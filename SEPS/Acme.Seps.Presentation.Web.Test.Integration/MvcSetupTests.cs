using Acme.Seps.Presentation.Web.DependencyInjection;
using Acme.Seps.Presentation.Web.Test.Integration.TestUtility;
using Acme.Seps.UseCases.Subsidy.Command;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Acme.Seps.Presentation.Web.Test.Integration;

[TestCaseOrderer(
    "Acme.Seps.Presentation.Web.Test.Integration.TestUtility.TestCollectionOrderer",
    "Acme.Seps.Presentation.Web.Test.Integration")]
public class MvcSetupTests : IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public MvcSetupTests(IntegrationTestingWebApplicationFactory<Startup> factory) =>
        _client = factory.CreateClient();

    [Fact, TestPriority(1)]
    public void SepsSimpleInjectorContainerIsVerified()
    {
        var sepsContainer = SepsSimpleInjectorContainer.Container;

        sepsContainer.Verify();
    }

    [Fact, TestPriority(2)]
    public async Task ModelValidationSendsBadRequestOnErroneousModel()
    {
        var command = new CalculateNewConsumerPriceIndexCommand { Amount = -2, Remark = null };

        var response = await _client
            .PostAsync(
                "/api/Parameter/AverageElectricEnergyProductionPrice/CalculateCpi",
                new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"))
            .ConfigureAwait(false);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}