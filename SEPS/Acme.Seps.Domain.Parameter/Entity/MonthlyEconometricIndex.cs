using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class MonthlyEconometricIndex<TMonthlyEconometricIndex> : EconometricIndex
        where TMonthlyEconometricIndex : MonthlyEconometricIndex<TMonthlyEconometricIndex>
    {
        public MonthlyPeriod MonthlyPeriod { get; protected set; }

        protected MonthlyEconometricIndex() { }

        protected MonthlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            MonthlyPeriod lastMonthlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, decimalPlaces, remark, identityFactory)
        {
            MonthlyPeriod.ValidTill.MustBe(null, (_, __) =>
                new DomainException(Message.MonthlyParameterException));
            MonthlyPeriod.ValidFrom.MustBeGreaterThanOrEqualTo(InitialPeriod, (_, __) =>
                new DomainException(Message.MonthlyParameterException));
            MonthlyPeriod.ValidFrom.MustBeLessThan(SystemTime.CurrentMonth(), (_, __) =>
                new DomainException(Message.MonthlyParameterException));

            MonthlyPeriod = new MonthlyPeriod(lastMonthlyPeriod.ValidTill.Value);
        }

        public abstract TMonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory);
    }
}