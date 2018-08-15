using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;

using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class YearlyEconometricIndex<TYearlyEconometricIndex> : EconometricIndex
        where TYearlyEconometricIndex : YearlyEconometricIndex<TYearlyEconometricIndex>
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
            Period.ValidFrom.Year.MustBeGreaterThanOrEqualTo(InitialPeriod.Year, (x, y) => new DomainException(Message.YearlyParameterException));
            Period.ValidTill.MustHaveValue(() => new DomainException(Message.YearlyParameterException));
            Period.ValidTill.Value.Year.MustBeLessThan(SystemTime.CurrentYear().Year, (x, y) => new DomainException(Message.YearlyParameterException));
        }

        public abstract TYearlyEconometricIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory);
    }
}