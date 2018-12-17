using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Infrastructure;
using Light.GuardClauses;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Acme.Seps.Domain.Base.Test.Unit")]
namespace Acme.Seps.Domain.Base.ValueType
{
    public sealed class ActivePeriod : ValueObject
    {
        public DateTimeOffset Since { get; private set; }
        public DateTimeOffset? Until { get; private set; }

        private ActivePeriod() { }

        private ActivePeriod(DateTimeOffset since, DateTimeOffset until)
        {
            until.MustBeGreaterThanOrEqualTo(since, (_, __) =>
                new DomainException(SepsBaseMessage.UnilGreaterThanSincePeriodException));

            Since = since;
            Until = until;
        }

        internal ActivePeriod(DateTimeOffset since) =>
            Since = since;

        internal ActivePeriod SetActiveUntil(DateTimeOffset until) =>
            new ActivePeriod(Since, until);
    }
}