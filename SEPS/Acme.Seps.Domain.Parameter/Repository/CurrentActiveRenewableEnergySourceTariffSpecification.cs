using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CurrentActiveRenewableEnergySourceTariffSpecification
        : BaseSpecification<RenewableEnergySourceTariff>
    {
        private readonly DateTimeOffset _previousYear;

        public CurrentActiveRenewableEnergySourceTariffSpecification()
        {
            _previousYear = SystemTime.CurrentYear().AddYears(-1);
        }

        public override Expression<Func<RenewableEnergySourceTariff, bool>> ToExpression() =>
            yei => yei.YearlyPeriod.ValidFrom <= _previousYear && _previousYear < yei.YearlyPeriod.ValidTill;
    }
}