using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Repository.Base;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Parameter.CommandValidation;
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

namespace Acme.Seps.Presentation.Web
{
    public sealed class SepsSimpleInjectorContainer : Container
    {
        private readonly DbContextOptionsBuilder<BaseContext> _options;
        private readonly char[] _onDot;
        private readonly string[] _currentAssemblyPartedFullName;

        public static SepsSimpleInjectorContainer Container { get; } = new SepsSimpleInjectorContainer();

        private SepsSimpleInjectorContainer()
        {
            _options = new DbContextOptionsBuilder<BaseContext>();
            _onDot = ".".ToCharArray();
            _currentAssemblyPartedFullName = Assembly.GetExecutingAssembly().GetName().Name.Split(_onDot);

            Bootstrap();
        }

        private void Bootstrap() =>
            SetLifestyle();

        private void SetLifestyle()
        {
            Options.DefaultLifestyle = new AsyncScopedLifestyle();
            Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public void RegisterForTest()
        {
            _options.UseSqlServer(@"Server=DL006132\IVAN;Database=IntegrationTesting;Trusted_Connection=True;");
            var bla = new ParameterContext(_options.Options);
            bla.Database.EnsureCreated();
            Register();
        }

        public void RegisterForProduction()
        {
            Register();
        }

        private void Register()
        {
            var projectName = _currentAssemblyPartedFullName[0];
            var baseProjectName = _currentAssemblyPartedFullName.Last();

            Register(typeof(IValidator<>), typeof(CalculateCpiCommandValidator).Assembly); // ToDo: integrate, singleton?

            DependencyContext.Default.RuntimeLibraries
                .Where(RuntimeLibraryIsFromProject)
                .Select(AssemblyFromRuntimeLibrary)
                .SelectMany(TypesFromAssembly)
                .Where(TypeIsForInjection)
                .Select(InterfaceAbstractionsWithImplementation)
                .ToList()
                .ForEach(RegisterAbstractionsWithImplementation);

            bool RuntimeLibraryIsFromProject(RuntimeLibrary runtimeLibrary) =>
                runtimeLibrary.Name.Contains(projectName);

            Assembly AssemblyFromRuntimeLibrary(RuntimeLibrary runtimeLibrary) =>
                Assembly.Load(new AssemblyName(runtimeLibrary.Name));

            IEnumerable<Type> TypesFromAssembly(Assembly assembly) =>
                assembly.GetExportedTypes();

            bool TypeIsForInjection(Type type) =>
                type.Namespace.Split(_onDot).First().Equals(projectName) &&
                !type.Namespace.Split(_onDot).Last().Equals(baseProjectName) &&
                type.GetInterfaces().Any(ite => ite.Namespace.Split(_onDot).First().Equals(projectName)) &&
                !type.IsAbstract &&
                !type.GetInterfaces().Any(ite => ite == typeof(IPeriodFactory)) &&
                !type.GetInterfaces().Any(ite => ite == typeof(IAggregateRoot));

            (List<Type> Abstractions, Type Implementation) InterfaceAbstractionsWithImplementation(Type type) =>
                (type.GetInterfaces().Where(ite => ite.Namespace.Split(_onDot).First().Equals(projectName)).ToList(),
                type);

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
    }
}
