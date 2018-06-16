using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.ValueType;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class EconometricIndex : SepsBaseAggregate
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
            amount.MustBeGreaterThan(0m, exception: () => new DomainException(Message.ParameterAmountBelowOrZeroException));
            decimalPlaces.MustBeGreaterThanOrEqualTo(0, exception: () => new DomainException(Message.ParameterDecimalPlacesBelowZeroException));
            remark.MustNotBeNullOrWhiteSpace(exception: () => new DomainException(Message.RemarkNotSetException));

            Amount = Math.Round(amount, decimalPlaces, MidpointRounding.AwayFromZero);
            Remark = remark;
        }
    }
}