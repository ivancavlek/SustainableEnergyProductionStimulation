using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Base.ValueType;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public abstract class MonthlyEconometricIndex<TMonthlyEconometricIndex> : EconometricIndex
        where TMonthlyEconometricIndex : MonthlyEconometricIndex<TMonthlyEconometricIndex>
    {
        protected MonthlyEconometricIndex() { }

        protected MonthlyEconometricIndex(
            decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
            : base(amount, remark, activeFrom.ToFirstDayOfTheMonth(), identityFactory)
        {
            Period.ActiveFrom.MustBeGreaterThanOrEqualTo(SepsVersion.InitialDate(), (_, __) =>
                new DomainException(SubsidyMessages.MonthlyParameterException));
            Period.ActiveFrom.MustBeLessThan(SystemTime.CurrentMonth(), (_, __) =>
                new DomainException(SubsidyMessages.MonthlyParameterException));
        }

        public TMonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory)

        {
            var activeTill = new DateTime(year, month, 1);

            Archive(activeTill);

            switch (this)
            {
                case MonthlyAverageElectricEnergyProductionPrice _:
                    return new MonthlyAverageElectricEnergyProductionPrice(amount, remark, activeTill, identityFactory)
                        as TMonthlyEconometricIndex;
                case NaturalGasSellingPrice _:
                    return new NaturalGasSellingPrice(amount, remark, activeTill, identityFactory)
                        as TMonthlyEconometricIndex;
                default:
                    throw new ArgumentException();
            }
        }

        public void Correct(
            decimal amount, string remark, int year, int month, NaturalGasSellingPrice previousActiveNgsp)
        {
            var correctedDate = new DateTime(year, month, 1);

            AmountCorrection(amount, remark);
            CorrectActiveFrom(correctedDate);
            previousActiveNgsp.CorrectActiveTill(correctedDate);
        }
    }
}