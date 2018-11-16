using FluentValidation;
using System;

namespace Acme.Seps.Presentation.Web.Utility
{
    public class DependencyInjectionFluentValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionFluentValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _serviceProvider.GetService(validatorType) as IValidator;
        }
    }
}