using Acme.Domain.Base.Entity;
using Acme.Seps.Text;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Acme.Seps.Domain.Base.Test.Unit")]
namespace Acme.Seps.Domain.Base.ValueType;

public sealed record ActivePeriod
{
    public DateTimeOffset Since { get; private set; }
    public DateTimeOffset? Until { get; private set; }

    private ActivePeriod() { }

    private ActivePeriod(DateTimeOffset since, DateTimeOffset until)
    {
        until.MustBeGreaterThanOrEqualTo(since, (_, __) =>
            new DomainException(SepsMessage.ValueHigherThanTheOther(until.Date.ToShortDateString(), since.Date.ToShortDateString())));

        Since = since;
        Until = until;
    }

    internal ActivePeriod(DateTimeOffset since) =>
        Since = since;

    internal ActivePeriod SetActiveUntil(DateTimeOffset until) =>
        new(Since, until);
}