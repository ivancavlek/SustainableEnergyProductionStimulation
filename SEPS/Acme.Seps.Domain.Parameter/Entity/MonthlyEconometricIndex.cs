using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class MonthlyEconometricIndex<TMonthlyEconometricIndex> : EconometricIndex
        where TMonthlyEconometricIndex : MonthlyEconometricIndex<TMonthlyEconometricIndex>
    {
        protected MonthlyEconometricIndex() { }

        protected MonthlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            Period lastMonthlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(
                  amount,
                  decimalPlaces,
                  remark,
                  new MonthlyPeriodFactory(lastMonthlyPeriod.ValidTill.Value),
                  identityFactory)
        {
            Period.ValidTill.HasValue.MustBe(false, (_, __) =>
                new DomainException(Message.MonthlyParameterException));
            Period.ValidFrom.MustBeGreaterThanOrEqualTo(InitialPeriod, (_, __) =>
                new DomainException(Message.MonthlyParameterException));
            Period.ValidFrom.MustBeLessThan(SystemTime.CurrentMonth(), (_, __) =>
                new DomainException(Message.MonthlyParameterException));
        }

        public abstract TMonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory);
    }
}