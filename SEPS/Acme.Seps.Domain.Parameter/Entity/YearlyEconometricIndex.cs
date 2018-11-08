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
        public YearlyPeriod YearlyPeriod { get; private set; }

        protected YearlyEconometricIndex() { }

        protected YearlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            YearlyPeriod lastYearlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, decimalPlaces, remark, identityFactory)
        {
            lastYearlyPeriod.ValidFrom.Year.MustBeGreaterThanOrEqualTo(InitialPeriod.Year, (_, __) =>
                new DomainException(Message.YearlyParameterException));
            lastYearlyPeriod.ValidTill.Year.MustBeLessThan(SystemTime.CurrentYear().Year, (_, __) =>
                new DomainException(Message.YearlyParameterException));

            YearlyPeriod = new YearlyPeriod(lastYearlyPeriod.ValidTill, lastYearlyPeriod.ValidTill.AddYears(1));
        }

        public abstract TYearlyEconometricIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory);
    }
}