using Acme.Seps.Domain.Subsidy.Command;
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

            sepsContainer.Verify();
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
            var command = new CalculateCpiCommand { Amount = 102, Remark = "Integration test calculate CPI remark" };

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
                Remark = "Integration test remark"
            };

            var response = await _client
                .PostAsync(_baseUri + "CalculateNaturalGas", new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CorrectActiveCpi()
        {
            var insertCommand =
                new CalculateCpiCommand { Amount = 102, Remark = "Integration test calculate CPI by update remark" };

            await _client
                .PostAsync(_baseUri + "CalculateCpi", new StringContent(JsonConvert.SerializeObject(insertCommand), Encoding.UTF8, "application/json"));

            var updateCommand = new CorrectActiveCpiCommand { Amount = 102, Remark = "Integration test correct active CPI remark" };

            var response = await _client
                .PutAsync(_baseUri + "CorrectActiveCpi", new StringContent(JsonConvert.SerializeObject(updateCommand), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetActiveConsumerPriceIndexes()
        {
            var response = await _client.GetAsync(_baseUri + "GetActiveConsumerPriceIndexes");
            var jsonResponse = await response.Content.ReadAsStringAsync();

            jsonResponse.Should().NotBeNullOrWhiteSpace();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetActiveNaturalGasSellingPrices()
        {
            var response = await _client.GetAsync(_baseUri + "GetActiveNaturalGasSellingPrices");
            var jsonResponse = await response.Content.ReadAsStringAsync();

            jsonResponse.Should().NotBeNullOrWhiteSpace();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetActiveRenewableEnergySourceTariffs()
        {
            var response = await _client.GetAsync(_baseUri + "GetActiveRenewableEnergySourceTariffs");
            var jsonResponse = await response.Content.ReadAsStringAsync();

            jsonResponse.Should().NotBeNullOrWhiteSpace();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetActiveCogenerationTariffs()
        {
            var response = await _client.GetAsync(_baseUri + "GetActiveCogenerationTariffs");
            var jsonResponse = await response.Content.ReadAsStringAsync();

            jsonResponse.Should().NotBeNullOrWhiteSpace();
            response.EnsureSuccessStatusCode();
        }
    }
}