using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Text;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public abstract class MonthlyEconometricIndex<TMonthlyEconometricIndex> : EconometricIndex
        where TMonthlyEconometricIndex : MonthlyEconometricIndex<TMonthlyEconometricIndex>
    {
        protected MonthlyEconometricIndex()
        {
        }

        protected MonthlyEconometricIndex(
            decimal amount, string remark, DateTimeOffset since, IIdentityFactory<Guid> identityFactory)
            : base(amount, remark, since.ToFirstDayOfTheMonth(), identityFactory)
        {
            Active.Since.MustBeGreaterThan(SepsVersion.InitialDate(), (_, __) =>
                new DomainException(SepsMessage.ValueHigherThanTheOther(Active.Since.Date.ToShortDateString(), SepsVersion.InitialDate().Date.ToShortDateString())));
            Active.Since.MustBeLessThan(SystemTime.CurrentMonth(), (_, __) =>
                new DomainException(SepsMessage.ValueHigherThanTheOther(Active.Since.Date.ToShortDateString(), SystemTime.CurrentMonth().Date.ToShortDateString())));
        }

        public TMonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory)

        {
            var until = new DateTime(year, month, 1);

            SetInactive(until);

            switch (this)
            {
                case AverageElectricEnergyProductionPrice _:
                    return new AverageElectricEnergyProductionPrice(amount, remark, until, identityFactory)
                        as TMonthlyEconometricIndex;

                case NaturalGasSellingPrice _:
                    return new NaturalGasSellingPrice(amount, remark, until, identityFactory)
                        as TMonthlyEconometricIndex;

                default:
                    throw new ArgumentException();
            }
        }

        public void Correct(
            decimal amount,
            string remark,
            int year,
            int month,
            TMonthlyEconometricIndex previousActiveMonthlyEconometricIndex)
        {
            var correctedDate = new DateTime(year, month, 1);

            AmountCorrection(amount, remark);
            CorrectActiveSince(correctedDate);
            previousActiveMonthlyEconometricIndex.CorrectActiveUntil(correctedDate);
        }
    }
}