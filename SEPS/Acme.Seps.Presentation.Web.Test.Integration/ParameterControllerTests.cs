using Acme.Seps.Domain.Parameter.Command;
using Acme.Seps.Presentation.Web.DependencyInjection;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Acme.Seps.Presentation.Web.Test.Integration
{
    public class ParameterControllerTests : IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
    {
        private readonly string _baseUri;
        private readonly HttpClient _client;

        public ParameterControllerTests(IntegrationTestingWebApplicationFactory<Startup> factory)
        {
            _baseUri = "/api/parameter/";
            _client = factory.CreateClient();
        }

        [Fact]
        public void SepsSimpleInjectorContainerIsVerified()
        {
            var sepsContainer = SepsSimpleInjectorContainer.Container;

            sepsContainer.Verify(); // ToDo: FluentValidation not complying
        }

        [Fact]
        public async Task ModelValidationSendsBadRequestOnErroneousModel()
        {
            var command = new CalculateCpiCommand { Amount = -2, Remark = null };

            var response = await _client
                .PostAsync(_baseUri + "CalculateCpi", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"));

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CalculateCpi()
        {
            var command = new CalculateCpiCommand { Amount = 2, Remark = "Some remark" };

            var response = await _client
                .PostAsync(_baseUri + "CalculateCpi", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CalculateNaturalGas()
        {
            var lastMonth = DateTime.Now.AddMonths(-1);

            var command = new CalculateNaturalGasCommand
            {
                Amount = 2,
                Month = lastMonth.Month,
                Year = lastMonth.Year,
                Remark = "Some remark"
            };

            var response = await _client
                .PostAsync(_baseUri + "CalculateNaturalGas", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
    }
}