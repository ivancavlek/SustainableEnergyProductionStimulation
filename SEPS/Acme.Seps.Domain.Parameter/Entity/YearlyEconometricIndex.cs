using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class YearlyEconometricIndex : EconometricIndex
    {
        protected YearlyEconometricIndex() { }

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
            if (!Period.ValidTill.HasValue ||
                Period.ValidFrom.Year < DateTime.Parse(Infrastructure.Parameter.InitialPeriod).Year ||
                SystemTime.CurrentYear().Year <= Period.ValidTill.Value.Year)
                throw new DomainException(Infrastructure.Parameter.YearlyParameterException);
        }
    }
}