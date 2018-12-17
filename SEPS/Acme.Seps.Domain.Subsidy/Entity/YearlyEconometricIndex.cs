using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public abstract class YearlyEconometricIndex<TYearlyEconometricIndex> : EconometricIndex
        where TYearlyEconometricIndex : YearlyEconometricIndex<TYearlyEconometricIndex>
    {
        protected YearlyEconometricIndex() { }

        protected YearlyEconometricIndex(
            decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
            : base(amount, remark, since.ToFirstMonthOfTheYear(), identityFactory)
        {
            Active.Since.Year.MustBeGreaterThanOrEqualTo(SepsVersion.InitialDate().Year, (_, __) =>
                new DomainException(SubsidyMessages.YearlyParameterException));
            Active.Since.MustBeLessThan(SystemTime.CurrentYear(), (_, __) =>
                new DomainException(SubsidyMessages.YearlyParameterException));
        }

        public TYearlyEconometricIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory)
        {
            SetInactive(Active.Since.ToFirstMonthOfTheYear().AddYears(1));

            switch (this)
            {
                case ConsumerPriceIndex cpi:
                    return new ConsumerPriceIndex(amount, remark, Active.Until.Value, identityFactory)
                        as TYearlyEconometricIndex;
                default:
                    throw new ArgumentException();
            }
        }

        public void Correct(decimal amount, string remark)
        {
            base.AmountCorrection(amount, remark);
        }
    }
}