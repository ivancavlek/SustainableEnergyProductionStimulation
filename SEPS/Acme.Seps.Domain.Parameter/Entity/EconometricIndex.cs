using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Entity;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class EconometricIndex : SepsBaseEntity, IAggregateRoot
    {
        public decimal Amount { get; }
        public string Remark { get; }
        public DateTime InitialPeriod { get { return new DateTime(2007, 07, 01); } }

        protected EconometricIndex()
        {
        }

        protected EconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            Period period,
            IIdentityFactory<Guid> identityFactory)
            : base(period, identityFactory)
        {
            if (amount <= 0)
                throw new DomainException(Infrastructure.Parameter.ParameterAmountBelowOrZeroException);
            if (decimalPlaces < 0)
                throw new DomainException(Infrastructure.Parameter.ParameterDecimalPlacesBelowZeroException);
            if (string.IsNullOrWhiteSpace(remark))
                throw new DomainException(Infrastructure.Parameter.RemarkNotSetException);

            Amount = Math.Round(amount, decimalPlaces, MidpointRounding.AwayFromZero);
            Remark = remark;
        }
    }
}