using Acme.Domain.Base.Entity;
using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.Infrastructure;
using Light.GuardClauses;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Acme.Seps.Domain.Base.Test.Unit")]
namespace Acme.Seps.Domain.Base.ValueType
{
    public sealed class Period : ValueObject
    {
        public DateTimeOffset ActiveFrom { get; private set; }
        public DateTimeOffset? ActiveTill { get; private set; }

        private Period() { }

        private Period(DateTimeOffset activeFrom, DateTimeOffset activeTill)
        {
            activeTill.MustBeGreaterThanOrEqualTo(activeFrom, (_, __) =>
                new DomainException(SepsBaseMessage.ActiveTillGreaterThanActiveFromException));

            ActiveFrom = activeFrom;
            ActiveTill = activeTill;
        }

        internal Period(DateTimeOffset activeFrom) =>
            ActiveFrom = activeFrom;

        internal Period SetActiveTill(DateTimeOffset activeTill) =>
            new Period(ActiveFrom, activeTill);
    }
}