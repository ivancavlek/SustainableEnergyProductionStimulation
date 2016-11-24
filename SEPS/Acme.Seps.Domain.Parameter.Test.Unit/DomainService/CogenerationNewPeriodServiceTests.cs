using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Parameter.DomainService;
using FluentAssertions;
using System;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class CogenerationNewPeriodServiceTests
    {
        private readonly ICogenerationNewPeriodService _cogenerationNewPeriodService;

        private MonthlyPeriod _naturalGasSellingPricePeriod;
        private YearlyPeriod _yaepPeriod;

        public CogenerationNewPeriodServiceTests()
        {
            _cogenerationNewPeriodService = new CogenerationNewPeriodService();
        }

        public void YaepPeriodIsUsedIfModifiedAfterNaturalGasPeriod()
        {
            _naturalGasSellingPricePeriod =
                new MonthlyPeriod(DateTimeOffset.Now.AddYears(-4), DateTimeOffset.Now.AddYears(-3));
            _yaepPeriod =
                new YearlyPeriod(DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(-2));

            var result = _cogenerationNewPeriodService.GetFrom(_naturalGasSellingPricePeriod, _yaepPeriod);

            result.Should().BeSameAs(_yaepPeriod);
        }

        public void NaturalGasPeriodIsUsedIfModifiedAfterYeap()
        {
            _naturalGasSellingPricePeriod =
                new MonthlyPeriod(DateTimeOffset.Now.AddMonths(-4), DateTimeOffset.Now.AddMonths(-3));
            _yaepPeriod =
                new YearlyPeriod(DateTimeOffset.Now.AddYears(-3), DateTimeOffset.Now.AddYears(-2));

            var result = _cogenerationNewPeriodService.GetFrom(_naturalGasSellingPricePeriod, _yaepPeriod);

            result.Should().BeSameAs(_naturalGasSellingPricePeriod);
        }
    }
}