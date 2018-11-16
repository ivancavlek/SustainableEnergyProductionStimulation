//using Acme.Domain.Base.Entity;
//using Acme.Seps.Domain.Base.Factory;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Acme.Seps.Bootstrap
//{
//    internal sealed class SepsAbstractionsWithImplementation
//        : AbstractionsWithImplementation, IAbstractionsWithImplementation
//    {
//        List<(List<Type> Abstractions, Type Implementation)>
//            IAbstractionsWithImplementation.InterfaceAbstractionsWithImplementation(IEnumerable<Type> assemblyTypes) =>
//            assemblyTypes
//                .Where(TypeIsForInjection)
//                .Select(InterfaceAbstractionsWithImplementation)
//                .ToList();

//        private bool TypeIsForInjection(Type type) =>
//            type.GetInterfaces().Any(NamespaceIsFromProject) &&
//            !type.IsAbstract &&
//            !type.GetInterfaces().Any(ite => ite == typeof(IPeriodFactory)) &&
//            !type.GetInterfaces().Any(ite => ite == typeof(IAggregateRoot));

//        private (List<Type> Abstractions, Type Implementation) InterfaceAbstractionsWithImplementation(Type type) =>
//            (type.GetInterfaces().Where(NamespaceIsFromProject).ToList(), type);

//        private bool NamespaceIsFromProject(Type type) =>
//            type.Namespace.Split(_onDot).First().Equals(_projectName);
//    }
//}