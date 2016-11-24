using Acme.Seps.Domain.Parameter.DomainService;
using FluentAssertions;

namespace Acme.Seps.Domain.Parameter.Test.Unit.Entity
{
    public class CogenerationParameterServiceTests
    {
        private readonly ICogenerationParameterService _cogenerationParameterService;

        public CogenerationParameterServiceTests()
        {
            _cogenerationParameterService = new CogenerationParameterService();
        }

        public void RatesAreCorrectlyCalculated()
        {
            var result = _cogenerationParameterService.GetFrom(1M, 1M);

            result.Should().Be(1M);
        }
    }
}