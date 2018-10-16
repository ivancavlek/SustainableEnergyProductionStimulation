using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acme.Seps.Domain.Parameter.Command;
using Newtonsoft.Json;
using Xunit;

namespace Acme.Seps.Presentation.Web.Test.Integration
{
    public class ParameterControllerTest : IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
    {
        private readonly string _baseUri;
        private readonly HttpClient _client;

        public ParameterControllerTest(IntegrationTestingWebApplicationFactory<Startup> factory)
        {
            _baseUri = "/api/parameter/";
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalculateCpi()
        {
            var command = new CalculateCpiCommand { Amount = 2, Remark = "Some remark" };

            var response = await _client
                .PostAsync(_baseUri + "CalculateCpi", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
    }
}