using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using Light.GuardClauses;
using System;
using Message = Acme.Seps.Domain.Parameter.Infrastructure.Parameter;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public abstract class EconometricIndex : SepsBaseAggregate
    {
        protected readonly int DecimalPlaces;

        public static DateTime InitialPeriod { get; } = new DateTime(2007, 07, 01);

        public decimal Amount { get; private set; }
        public string Remark { get; private set; }

        protected EconometricIndex() { }

        protected EconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            IPeriodFactory periodFactory,
            IIdentityFactory<Guid> identityFactory)
            : base(periodFactory, identityFactory)
        {
            amount.MustBeGreaterThan(0m, (_, __) =>
                new DomainException(Message.ParameterAmountBelowOrZeroException));
            decimalPlaces.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                new DomainException(Message.ParameterDecimalPlacesBelowZeroException));
            remark.MustNotBeNullOrWhiteSpace((_) => new DomainException(Message.RemarkNotSetException));

            DecimalPlaces = decimalPlaces;
            Amount = RoundAmount(amount);
            Remark = remark;
        }

        public void AmountCorrection(decimal amount, string remark)
        {
            Amount = RoundAmount(amount);
            Remark = remark;
        }

        private decimal RoundAmount(decimal amount) =>
            Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);

    }
}