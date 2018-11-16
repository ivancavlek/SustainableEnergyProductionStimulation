//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Acme.Seps.Bootstrap
//{
//    internal sealed class FluentValidationAbstractionsWithImplementation
//        : AbstractionsWithImplementation, IAbstractionsWithImplementation
//    {
//        List<(List<Type> Abstractions, Type Implementation)>
//            IAbstractionsWithImplementation.InterfaceAbstractionsWithImplementation(IEnumerable<Type> assemblyTypes) =>
//            assemblyTypes
//                .Where(TypeIsForInjection)
//                .Select(InterfaceAbstractionsWithImplementation)
//                .ToList();

//        private bool TypeIsForInjection(Type type) =>
//            type.BaseType?.IsGenericType is true &&
//            type.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>);

//        private (List<Type> Abstractions, Type Implementation) InterfaceAbstractionsWithImplementation(Type type) =>
//            (type.BaseType.GetInterfaces()
//                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>)).ToList(), type);
//    }
//}