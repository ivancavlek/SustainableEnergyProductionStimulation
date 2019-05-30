using Acme.Seps.Presentation.Web.Test.Integration.TestUtility;
using Acme.Seps.UseCases.Subsidy.Command;
using Acme.Seps.UseCases.Subsidy.Query;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Acme.Seps.Presentation.Web.Test.Integration.Parameter
{
    [TestCaseOrderer(
        "Acme.Seps.Presentation.Web.Test.Integration.TestUtility.TestCollectionOrderer",
        "Acme.Seps.Presentation.Web.Test.Integration")]
    public class ConsumerPriceIndexControllerTests
        : ParameterBaseControllerTests, IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
    {
        public ConsumerPriceIndexControllerTests(IntegrationTestingWebApplicationFactory<Startup> factory)
            : base(factory) =>
            _baseUri = "ConsumerPriceIndex/";

        [Fact, TestPriority(1)]
        public async Task GetConsumerPriceIndexes()
        {
            var allCpis = await GetAllConsumerPriceIndexes().ConfigureAwait(false);

            allCpis.Count.Should().Be(1);
            foreach (var cpi in allCpis)
            {
                cpi.Amount.Should().Be(105.8M);
                cpi.Remark.Should().Be("Initial value");
                cpi.Since.Should().Be(new DateTime(2007, 7, 1));
                cpi.Until.Should().BeNull();
            }
        }

        [Fact, TestPriority(2)]
        public async Task CalculateCpi()
        {
            var command = new CalculateNewConsumerPriceIndexCommand
            {
                Amount = 102.9M,
                Remark = "Integration test calculate CPI remark"
            };

            var response = await _client
                .PostAsync(
                    _baseUri + "CalculateCpi",
                    new StringContent(JsonConvert.SerializeObject(command), _encoding, _contentType))
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            await CheckNewAndOldCpiAndResValues(command.Amount, command.Remark).ConfigureAwait(false);
        }

        [Fact, TestPriority(3)]
        public async Task CorrectActiveCpi()
        {
            var allCpis = await GetAllConsumerPriceIndexes().ConfigureAwait(false);

            if (allCpis.Equals(1))
            {
                var command = new CalculateNewConsumerPriceIndexCommand
                {
                    Amount = 102.9M,
                    Remark = "Integration test correct CPI remark"
                };

                var response = await _client
                    .PostAsync(
                        _baseUri + "CalculateCpi",
                        new StringContent(JsonConvert.SerializeObject(command), _encoding, _contentType))
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }

            var correctCommand = new CorrectActiveConsumerPriceIndexCommand
            {
                Amount = 106.9M,
                Remark = "Integration test correct CPI remark"
            };

            var correctResponse = await _client
                .PutAsync(
                    _baseUri + "CorrectActiveCpi",
                    new StringContent(JsonConvert.SerializeObject(correctCommand), _encoding, _contentType))
                .ConfigureAwait(false);

            correctResponse.EnsureSuccessStatusCode();

            await CheckNewAndOldCpiAndResValues(correctCommand.Amount, correctCommand.Remark).ConfigureAwait(false);
        }

        private async Task<List<EconometricIndexQueryResult>> GetAllConsumerPriceIndexes()
        {
            var cpiQueryResponse = await _client.GetAsync(_baseUri + "GetConsumerPriceIndexes").ConfigureAwait(false);
            var cpiJsonResponse = await cpiQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<EconometricIndexQueryResult>>(cpiJsonResponse);
        }

        private async Task CheckNewAndOldCpiAndResValues(decimal newAmount, string newRemark)
        {
            var allCpis = await GetAllConsumerPriceIndexes().ConfigureAwait(false);

            var oldCpi = allCpis.Single(cpi => cpi.Until.HasValue);
            var newCpi = allCpis.Single(cpi => !cpi.Until.HasValue);
            const decimal oldAmount = 105.8M;
            var oldDate = new DateTime(2007, 7, 1);
            var newDate = new DateTime(2008, 1, 1);

            oldCpi.Amount.Should().Be(oldAmount);
            oldCpi.Remark.Should().Be("Initial value");
            oldCpi.Since.Should().Be(oldDate);
            oldCpi.Until.Should().Be(newDate);
            newCpi.Amount.Should().Be(newAmount);
            newCpi.Remark.Should().Be(newRemark);
            newCpi.Since.Should().Be(newDate);
            newCpi.Until.Should().BeNull();

            var allRes = await GetAllResTariffs().ConfigureAwait(false);

            var oldRes = allRes.Where(res => res.Until.HasValue).ToList();
            var newRes = allRes.Where(res => !res.Until.HasValue).ToList();

            oldRes.Count.Should().Be(9);
            newRes.Count.Should().Be(9);

            foreach (var res in oldRes)
            {
                res.Since.Should().Be(oldDate);
                res.Until.Should().Be(newDate);
                res.ConsumerPriceIndexAmount.Should().Be(oldAmount);
            }

            foreach (var res in newRes)
            {
                res.Since.Should().Be(newDate);
                res.Until.Should().BeNull();
                res.ConsumerPriceIndexAmount.Should().Be(newAmount);
            }
        }
    }
}
