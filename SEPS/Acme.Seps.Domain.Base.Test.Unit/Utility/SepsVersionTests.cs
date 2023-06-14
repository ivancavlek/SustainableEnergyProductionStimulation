using Acme.Seps.Domain.Base.Utility;
using System;

namespace Acme.Seps.Domain.Base.Test.Unit.Utility;

public class SepsVersionTests
{
    public void InitialPeriodIsCorrectlySet() =>
        SepsVersion.InitialDate().Should().Be(new DateTimeOffset(new DateTime(2007, 7, 1)));
}