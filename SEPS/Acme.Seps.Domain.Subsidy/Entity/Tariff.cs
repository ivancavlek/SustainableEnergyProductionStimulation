﻿using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using Light.GuardClauses;
using System;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public abstract class Tariff : SepsAggregateRoot
    {
        public decimal? LowerProductionLimit { get; private set; }
        public decimal? UpperProductionLimit { get; private set; }
        public decimal LowerRate { get; protected set; }
        public decimal HigherRate { get; protected set; }
        public Guid ProjectTypeId { get; private set; }

        protected Tariff() { }

        protected Tariff(
            decimal? lowerProductionLimit,
            decimal? upperProductionLimit,
            decimal lowerRate,
            decimal higherRate,
            Guid projectTypeId,
            DateTimeOffset activeFrom,
            IIdentityFactory<Guid> identityFactory) : base(activeFrom, identityFactory)
        {
            if (lowerProductionLimit.HasValue)
                lowerProductionLimit.Value.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                    new DomainException(SubsidyMessages.BelowZeroLowerProductionLimitException));
            if (upperProductionLimit.HasValue)
            {
                upperProductionLimit.Value.MustBeGreaterThanOrEqualTo(0, (_, __) =>
                   new DomainException(SubsidyMessages.BelowZeroUpperProductionLimitException));
                lowerProductionLimit.Value.MustBeLessThanOrEqualTo(upperProductionLimit.Value, (_, __) =>
                    new DomainException(SubsidyMessages.LowerProductionLimitAboveUpperProductionLimitException));
            }
            lowerRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
                new DomainException(SubsidyMessages.BelowZeroLowerRateException));
            higherRate.MustBeGreaterThanOrEqualTo(0m, (_, __) =>
                new DomainException(SubsidyMessages.BelowZeroUpperRateException));
            lowerRate.MustBeLessThanOrEqualTo(higherRate, (_, __) =>
                new DomainException(SubsidyMessages.LowerRateAboveUpperException));
            projectTypeId.MustNotBeEmpty(() =>
                new DomainException(SubsidyMessages.ProjectTypeIdentifierException));
            projectTypeId.MustNotBeDefault(() =>
                new DomainException(SubsidyMessages.ProjectTypeIdentifierException));

            LowerProductionLimit = lowerProductionLimit;
            UpperProductionLimit = upperProductionLimit;
            LowerRate = lowerRate;
            HigherRate = higherRate;
            ProjectTypeId = projectTypeId;
        }
    }
}