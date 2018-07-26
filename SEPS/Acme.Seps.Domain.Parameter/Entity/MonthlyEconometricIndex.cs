using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class MonthlyEconometricIndex : EconometricIndex
    {
        protected MonthlyEconometricIndex()
        {
        }

        protected MonthlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            MonthlyPeriod lastMonthlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(
                  amount,
                  decimalPlaces,
                  remark,
                  new MonthlyPeriod(lastMonthlyPeriod.ValidTill.Value),
                  identityFactory)
        {
            Period.ValidTill.MustBe(null, (x, y) => new DomainException(Message.MonthlyParameterException));
            Period.ValidFrom.MustBeGreaterThanOrEqualTo(InitialPeriod, (x, y) => new DomainException(Message.MonthlyParameterException));
            Period.ValidFrom.MustBeLessThan(SystemTime.CurrentMonth(), (x, y) => new DomainException(Message.MonthlyParameterException));
        }

        public abstract MonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory);
    }
}