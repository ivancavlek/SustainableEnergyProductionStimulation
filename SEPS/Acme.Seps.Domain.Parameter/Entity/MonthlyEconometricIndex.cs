using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

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
            if (Period.ValidTill.HasValue ||
                Period.ValidFrom < InitialPeriod ||
                SystemTime.CurrentMonth() <= Period.ValidFrom)
                throw new DomainException(Infrastructure.Parameter.MonthlyParameterException);
        }

        public abstract MonthlyEconometricIndex CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory);
    }
}