using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Repository.Base;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Repository.Parameter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acme.Seps.Presentation.Web.DependencyInjection
{
    public sealed class SepsSimpleInjectorContainer : Container
    {
        private readonly DbContextOptionsBuilder<BaseContext> _options;
        private readonly char[] _onDot;
        private readonly string _projectName;
        private readonly string _executingProjectName;

        public static SepsSimpleInjectorContainer Container { get; } = new SepsSimpleInjectorContainer();

        private SepsSimpleInjectorContainer()
        {
            _options = new DbContextOptionsBuilder<BaseContext>();
            _onDot = ".".ToCharArray();
            var currentAssemblyPartedFullName = Assembly.GetExecutingAssembly().GetName().Name.Split(_onDot);
            _projectName = currentAssemblyPartedFullName[0];
            _executingProjectName = currentAssemblyPartedFullName.Last();

            Options.DefaultLifestyle = new AsyncScopedLifestyle();
            Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public void RegisterForTest()
        {
            _options.UseSqlServer(@"Server=DL006132\IVAN;Database=IntegrationTesting;Trusted_Connection=True;");
            var bla = new ParameterContext(_options.Options);
            bla.Database.EnsureCreated();
            RegisterAbstractionsWithImplementation();
        }

        public void RegisterForProduction()
        {
            RegisterAbstractionsWithImplementation();
        }

        private void RegisterAbstractionsWithImplementation()
        {
            var typesFromAssemblies = GetTypesFromAssemblies();

            RegisterSepsAbstractionsWithImplementationsWith(typesFromAssemblies);
            RegisterFluentValidationAbstractionsWithImplementationsWith(typesFromAssemblies);
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
            types
                .Where(TypeIsForInjection)
                .Select(InterfaceAbstractionsWithImplementation)
                .ToList()
                .ForEach(RegisterAbstractionsWithImplementation);

            bool TypeIsForInjection(Type type) =>
                type.GetInterfaces().Any(NamespaceIsFromProject) &&
                !type.IsAbstract &&
                !type.GetInterfaces().Any(ite => ite == typeof(IPeriodFactory)) &&
                !type.GetInterfaces().Any(ite => ite == typeof(IAggregateRoot));

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
                            Register(asn, () => new GuidIdentityFactory(SequentialGuidType.SequentialAtEnd));
                            break;
                        case nameof(ParameterContext):
                            Register(asn, () => new ParameterContext(_options.Options));
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
}