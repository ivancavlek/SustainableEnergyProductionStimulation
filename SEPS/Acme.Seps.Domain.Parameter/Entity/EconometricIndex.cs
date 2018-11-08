using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class EconometricIndex : SepsBaseAggregate
    {
        public decimal Amount { get; private set; }
        public string Remark { get; private set; }
        public DateTime InitialPeriod { get { return new DateTime(2007, 07, 01); } }

        protected EconometricIndex() { }

        protected EconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            IIdentityFactory<Guid> identityFactory)
            : base(identityFactory)
        {
            amount.MustBeGreaterThan(0m, (_, __) =>
                new DomainException(Message.ParameterAmountBelowOrZeroException));
            decimalPlaces.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                new DomainException(Message.ParameterDecimalPlacesBelowZeroException));
            remark.MustNotBeNullOrWhiteSpace((_) => new DomainException(Message.RemarkNotSetException));

            Amount = Math.Round(amount, decimalPlaces, MidpointRounding.AwayFromZero);
            Remark = remark;
        }
    }
}