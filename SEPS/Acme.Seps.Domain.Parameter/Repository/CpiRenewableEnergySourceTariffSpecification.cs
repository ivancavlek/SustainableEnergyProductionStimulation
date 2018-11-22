using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CpiRenewableEnergySourceTariffSpecification : BaseSpecification<RenewableEnergySourceTariff>
    {
        private readonly Guid _cpiId;

        public CpiRenewableEnergySourceTariffSpecification(Guid cpiId)
        {
            _cpiId = cpiId;
        }

        public override Expression<Func<RenewableEnergySourceTariff, bool>> ToExpression() =>
            res => res.ConsumerPriceIndex.Id == _cpiId;
    }
}