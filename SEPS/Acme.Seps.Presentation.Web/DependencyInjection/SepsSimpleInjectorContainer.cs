﻿using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Repository.Base;
using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Repository.Subsidy;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Acme.Seps.Presentation.Web.DependencyInjection;

public sealed class SepsSimpleInjectorContainer : Container
{
    private readonly DbContextOptionsBuilder<BaseContext> _options;
    private readonly char[] _onDot;
    private readonly string _projectName;
    private readonly string _executingProjectName;
    private readonly string _connectionString;

    public static SepsSimpleInjectorContainer Container { get; } = new SepsSimpleInjectorContainer();

    private SepsSimpleInjectorContainer()
    {
        _options = new DbContextOptionsBuilder<BaseContext>();
        _onDot = ".".ToCharArray();
        var currentAssemblyPartedFullName = Assembly.GetExecutingAssembly().GetName().Name.Split(_onDot);
        _projectName = currentAssemblyPartedFullName[0];
        _executingProjectName = currentAssemblyPartedFullName.Last();
        _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=IntegrationTesting;Trusted_Connection=True;";

        Options.DefaultLifestyle = new AsyncScopedLifestyle();
        Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
    }

    public void RegisterForTest()
    {
        _options.UseSqlServer(_connectionString);
        var bla = new ParameterContext(_options.Options, new GuidIdentityFactory(SequentialGuidType.SequentialAtEnd));
        bla.Database.EnsureCreated();
        RegisterAbstractionsWithImplementation();
    }

    public void RegisterForProduction() =>
        RegisterAbstractionsWithImplementation();

    private void RegisterAbstractionsWithImplementation()
    {
        var typesFromAssemblies = GetTypesFromAssemblies();

        RegisterSepsAbstractionsWithImplementationsWith(typesFromAssemblies);
        RegisterFluentValidationAbstractionsWithImplementationsWith(typesFromAssemblies);
        Register(typeof(IDbConnection), () => new SqlConnection(_connectionString));
        Register(typeof(ICqrsMediator), () => new CqrsMediator(this));
    }

    private IEnumerable<Type> GetTypesFromAssemblies()
    {
        return DependencyContext.Default.RuntimeLibraries
            .Where(RuntimeLibraryIsFromProject)
            .Select(AssemblyFromRuntimeLibrary)
            .SelectMany(TypesFromAssembly);

        bool RuntimeLibraryIsFromProject(RuntimeLibrary runtimeLibrary) =>
            runtimeLibrary.Name.Contains(_projectName) &&
            !runtimeLibrary.Name.Contains(_executingProjectName);

        Assembly AssemblyFromRuntimeLibrary(RuntimeLibrary runtimeLibrary) =>
            Assembly.Load(new AssemblyName(runtimeLibrary.Name));

        IEnumerable<Type> TypesFromAssembly(Assembly assembly) =>
            assembly.GetExportedTypes();
    }

    private void RegisterSepsAbstractionsWithImplementationsWith(IEnumerable<Type> types)
    {
        var identityFactory = new GuidIdentityFactory(SequentialGuidType.SequentialAtEnd);

        types
            .Where(TypeIsForInjection)
            .Select(InterfaceAbstractionsWithImplementation)
            .ToList()
            .ForEach(RegisterAbstractionsWithImplementation);

        bool TypeIsForInjection(Type type) =>
            type.GetInterfaces().Any(NamespaceIsFromProject) &&
            !type.IsAbstract &&
            !type.GetInterfaces().Any(ite => ite == typeof(IAggregateRoot) || ite == typeof(ISepsCommand));

        (List<Type> Abstractions, Type Implementation) InterfaceAbstractionsWithImplementation(Type type) =>
            (type.GetInterfaces().Where(NamespaceIsFromProject).ToList(), type);

        bool NamespaceIsFromProject(Type type) =>
            type.Namespace.Split(_onDot).First().Equals(_projectName);

        void RegisterAbstractionsWithImplementation((List<Type> Abstractions, Type Implementation) registration) =>
            registration.Abstractions.ForEach(asn =>
            {
                switch (registration.Implementation.Name)
                {
                    case nameof(GuidIdentityFactory):
                        Register(asn, () => identityFactory);
                        break;
                    case nameof(ParameterContext):
                        Register(asn, () => new ParameterContext(_options.Options, identityFactory));
                        break;
                    default:
                        Register(asn, registration.Implementation);
                        break;
                }
            });
    }

    private void RegisterFluentValidationAbstractionsWithImplementationsWith(IEnumerable<Type> types)
    {
        types
            .Where(TypeIsForInjection)
            .Select(InterfaceAbstractionsWithImplementation)
            .ToList()
            .ForEach(RegisterAbstractionsWithImplementation);

        bool TypeIsForInjection(Type type) =>
            type.BaseType?.IsGenericType is true &&
            type.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>);

        (Type Abstraction, Type Implementation) InterfaceAbstractionsWithImplementation(Type type) =>
            (type.BaseType.GetInterfaces()
                .Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>)), type);

        void RegisterAbstractionsWithImplementation((Type Abstraction, Type Implementation) registration) =>
            Register(registration.Abstraction, registration.Implementation);
    }
}