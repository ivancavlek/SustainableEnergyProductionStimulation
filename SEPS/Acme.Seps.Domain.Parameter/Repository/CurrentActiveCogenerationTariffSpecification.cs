using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CurrentActiveCogenerationTariffSpecification
        : BaseSpecification<CogenerationTariff>
    {
        public override Expression<Func<CogenerationTariff, bool>> ToExpression() =>
            mei => !mei.MonthlyPeriod.ValidTill.HasValue;
    }
}