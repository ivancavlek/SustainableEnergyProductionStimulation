using Acme.Seps.Domain.Subsidy.Test.Unit.Factory;
using Acme.Seps.UseCases.Subsidy.Command.DomainService;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.DomainService
{
    public class CogenerationParameterServiceTests
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;
        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly IEnumerable<NaturalGasSellingPrice> _yearsNaturalGasSellingPrices;

        public CogenerationParameterServiceTests()
        {
            IEconometricIndexFactory<NaturalGasSellingPrice> ngspFactory =
                new EconometricIndexFactory<NaturalGasSellingPrice>(DateTime.Now.AddYears(-3));
            _naturalGasSellingPrice = ngspFactory.Create();

            _yearsNaturalGasSellingPrices = new List<NaturalGasSellingPrice> { _naturalGasSellingPrice };

            _cogenerationParameterService = new CogenerationParameterService();
        }

        public void RatesAreCorrectlyCalculated()
        {
            var result = _cogenerationParameterService.GetFrom(_yearsNaturalGasSellingPrices, _naturalGasSellingPrice);

            result.Should().Be(100M);
        }
    }
}