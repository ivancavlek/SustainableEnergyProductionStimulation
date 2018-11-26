//using Acme.Domain.Base.Factory;
//using Acme.Repository.Base;
//using Acme.Seps.Repository.Subsidy;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyModel;
//using SimpleInjector;
//using SimpleInjector.Lifestyles;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;

//namespace Acme.Seps.Bootstrap
//{
//    public sealed class SepsSimpleInjectorContainer : Container
//    {
//        private readonly DbContextOptionsBuilder<BaseContext> _options;

//        public static SepsSimpleInjectorContainer Container { get; } = new SepsSimpleInjectorContainer();

//        private SepsSimpleInjectorContainer()
//        {
//            _options = new DbContextOptionsBuilder<BaseContext>();

//            Options.DefaultLifestyle = new AsyncScopedLifestyle();
//            Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
//        }

//        public void RegisterForTest()
//        {
//            _options.UseSqlServer(@"Server=DL006132\IVAN;Database=IntegrationTesting;Trusted_Connection=True;");
//            var bla = new ParameterContext(_options.Options);
//            bla.Database.EnsureCreated();
//            RegisterAbstractionsWithImplementation();
//        }

//        public void RegisterForProduction()
//        {
//            RegisterAbstractionsWithImplementation();
//        }

//        private void RegisterAbstractionsWithImplementation()
//        {
//            var assembliesTypes = GetTypesFromAssemblies();

//            GetAbstractionsWithImplementation()
//                .ForEach(awi => awi.InterfaceAbstractionsWithImplementation(assembliesTypes)
//                    .ForEach(RegisterAbstractionsWithImplementation));

// problem with calling DI, maybe somehow setting registry in get property and initializing in ctor
//            List<IAbstractionsWithImplementation> GetAbstractionsWithImplementation() =>
//                new List<IAbstractionsWithImplementation>
//                {
//                    new FluentValidationAbstractionsWithImplementation(),
//                    new SepsAbstractionsWithImplementation()
//                };

//            void RegisterAbstractionsWithImplementation((List<Type> Abstractions, Type Implementation) registration) =>
//               registration.Abstractions.ForEach(asn =>
//               {
//                   switch (registration.Implementation.Name)
//                   {
//                       case nameof(GuidIdentityFactory):
//                           Register(asn, () => new GuidIdentityFactory(SequentialGuidType.SequentialAtEnd));
//                           break;
//                       case nameof(ParameterContext):
//                           Register(asn, () => new ParameterContext(_options.Options));
//                           break;
//                       default:
//                           Register(asn, registration.Implementation);
//                           break;
//                   }
//               });
//        }

//        private IEnumerable<Type> GetTypesFromAssemblies() // Lazy
//        {
//            return DependencyContext.Default.RuntimeLibraries
//                .Where(RuntimeLibraryIsFromProject)
//                .Select(AssemblyFromRuntimeLibrary)
//                .SelectMany(TypesFromAssembly)
//                .ToList();

//            bool RuntimeLibraryIsFromProject(RuntimeLibrary runtimeLibrary) =>
//                runtimeLibrary.Name.Contains(_projectName) &&
//                !runtimeLibrary.Name.Contains(_executingProjectName);

//            Assembly AssemblyFromRuntimeLibrary(RuntimeLibrary runtimeLibrary) =>
//                Assembly.Load(new AssemblyName(runtimeLibrary.Name));

//            IEnumerable<Type> TypesFromAssembly(Assembly assembly) =>
//                assembly.GetExportedTypes();
//        }
//    }
//}