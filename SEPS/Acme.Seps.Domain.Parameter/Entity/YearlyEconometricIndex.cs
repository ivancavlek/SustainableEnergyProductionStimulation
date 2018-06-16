using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;

using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class YearlyEconometricIndex : EconometricIndex
    {
        protected YearlyEconometricIndex()
        {
        }

        protected YearlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            YearlyPeriod lastYearlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(
                  amount,
                  decimalPlaces,
                  remark,
                  new YearlyPeriod(lastYearlyPeriod.ValidTill.Value, lastYearlyPeriod.ValidTill.Value.AddYears(1)),
                  identityFactory)
        {
            Period.ValidFrom.Year.MustBeGreaterThanOrEqualTo(InitialPeriod.Year, exception: () => new DomainException(Message.YearlyParameterException));
            Period.ValidTill.MustHaveValue(exception: () => new DomainException(Message.YearlyParameterException));
            Period.ValidTill.Value.Year.MustBeLessThan(SystemTime.CurrentYear().Year, exception: () => new DomainException(Message.YearlyParameterException));
        }

        public abstract YearlyEconometricIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory);
    }
}