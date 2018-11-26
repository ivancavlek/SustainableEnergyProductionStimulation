using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.DomainService;
using Acme.Seps.Domain.Subsidy.Entity;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class CogenerationParameterServiceTests
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;

        private readonly NaturalGasSellingPrice _naturalGasSellingPrice;
        private readonly IEnumerable<NaturalGasSellingPrice> _yearsNaturalGasSellingPrices;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public CogenerationParameterServiceTests()
        {
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            _naturalGasSellingPrice = Activator.CreateInstance(
                typeof(NaturalGasSellingPrice),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] {
                    10M,
                    nameof(NaturalGasSellingPrice),
                    new Period(new MonthlyPeriodFactory(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(-2))),
                    _identityFactory },
                null) as NaturalGasSellingPrice;
            _yearsNaturalGasSellingPrices = new List<NaturalGasSellingPrice> { _naturalGasSellingPrice };

            _cogenerationParameterService = new CogenerationParameterService();
        }

        public void RatesAreCorrectlyCalculated()
        {
            var result = _cogenerationParameterService.GetFrom(_yearsNaturalGasSellingPrices, _naturalGasSellingPrice);

            result.Should().Be(10M);
        }
    }
}