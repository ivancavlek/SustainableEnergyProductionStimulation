using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Subsidy.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Subsidy.Repository
{
    public sealed class GspCogenerationTariffSpecification : BaseSpecification<CogenerationTariff>
    {
        private readonly Guid _gspId;

        public GspCogenerationTariffSpecification(Guid gspId)
        {
            _gspId = gspId;
        }

        public override Expression<Func<CogenerationTariff, bool>> ToExpression() =>
            ctf => ctf.NaturalGasSellingPrice.Id == _gspId;
    }
}