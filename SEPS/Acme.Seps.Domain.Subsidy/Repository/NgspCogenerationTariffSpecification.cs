using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Subsidy.Repository
{
    public sealed class NgspCogenerationTariffSpecification : BaseSpecification<CogenerationTariff>
    {
        private readonly Guid _ngspId;

        public NgspCogenerationTariffSpecification(Guid ngspId)
        {
            _ngspId = ngspId;
        }

        public override Expression<Func<CogenerationTariff, bool>> ToExpression() =>
            ctf => ctf.NaturalGasSellingPrice.Id == _ngspId;
    }
}