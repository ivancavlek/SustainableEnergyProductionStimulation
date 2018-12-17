using Acme.Seps.Domain.Subsidy.Command;
using Acme.Seps.Domain.Subsidy.Query;
using Acme.Seps.Presentation.Web.DependencyInjection;
using Acme.Seps.Presentation.Web.Test.Integration.TestUtility;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Acme.Seps.Presentation.Web.Test.Integration
{
    [TestCaseOrderer("Acme.Seps.Presentation.Web.Test.Integration.TestUtility.TestCollectionOrderer", "Acme.Seps.Presentation.Web.Test.Integration")]
    public class ParameterControllerTests : IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
    {
        private readonly string _baseUri;
        private readonly Encoding _encoding;
        private readonly string _contentType;
        private readonly HttpClient _client;

        public ParameterControllerTests(IntegrationTestingWebApplicationFactory<Startup> factory)
        {
            _baseUri = "/api/parameter/";
            _encoding = Encoding.UTF8;
            _contentType = "application/json";
            _client = factory.CreateClient();
        }

        [Fact, TestPriority(1)]
        public void SepsSimpleInjectorContainerIsVerified()
        {
            var sepsContainer = SepsSimpleInjectorContainer.Container;

            sepsContainer.Verify();
        }

        [Fact, TestPriority(2)]
        public async Task ModelValidationSendsBadRequestOnErroneousModel()
        {
            var command = new CalculateConsumerPriceIndexCommand { Amount = -2, Remark = null };

            var response = await _client
                .PostAsync(
                    _baseUri + "CalculateCpi",
                    new StringContent(JsonConvert.SerializeObject(command), _encoding, _contentType))
                .ConfigureAwait(false);

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact, TestPriority(3)]
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

        [Fact, TestPriority(4)]
        public async Task GetNaturalGasSellingPrices()
        {
            var allNgsps = await GetAllNaturalGasSellingPrices().ConfigureAwait(false);

            allNgsps.Count.Should().Be(1);
            foreach (var cpi in allNgsps)
            {
                cpi.Amount.Should().Be(1.07M);
                cpi.Remark.Should().Be("Initial value");
                cpi.Since.Should().Be(new DateTime(2007, 7, 1));
                cpi.Until.Should().BeNull();
            }
        }

        [Fact, TestPriority(5)]
        public async Task GetRenewableEnergySourceTariffs()
        {
            var allRess = await GetAllResTariffs().ConfigureAwait(false);

            allRess.Count.Should().Be(9);
            foreach (var res in allRess)
            {
                res.ConsumerPriceIndexAmount.Should().Be(105.8M);
                res.Since.Should().Be(new DateTime(2007, 7, 1));
                res.Until.Should().BeNull();
            }
        }

        [Fact, TestPriority(6)]
        public async Task GetCogenerationTariffs()
        {
            var allCgns = await GetAllCogenerationTariffs().ConfigureAwait(false);

            allCgns.Count.Should().Be(2);
            foreach (var cgn in allCgns)
            {
                cgn.NaturalGasSellingPriceAmount.Should().Be(1.07M);
                cgn.Since.Should().Be(new DateTime(2007, 7, 1));
                cgn.Until.Should().BeNull();
            }
        }

        [Fact, TestPriority(7)]
        public async Task CalculateCpi()
        {
            var command = new CalculateConsumerPriceIndexCommand
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

        [Fact, TestPriority(8)]
        public async Task CalculateNaturalGas()
        {
            var lastMonth = DateTime.Now.AddMonths(-3);

            var command = new CalculateNaturalGasSellingPriceCommand
            {
                Amount = 1.32M,
                Month = lastMonth.Month,
                Year = lastMonth.Year,
                Remark = "Integration test calculate NGSP remark"
            };

            var response = await _client
                .PostAsync(
                    _baseUri + "CalculateNaturalGas",
                    new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            await CheckNewAndOldNgspAndCgnValues(
                lastMonth.Year, lastMonth.Month, command.Amount, command.Remark).ConfigureAwait(false);
        }

        [Fact, TestPriority(9)]
        public async Task CorrectActiveCpi()
        {
            var allCpis = await GetAllConsumerPriceIndexes().ConfigureAwait(false);

            if (allCpis.Count.Equals(1))
            {
                var command = new CalculateConsumerPriceIndexCommand
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

        [Fact, TestPriority(10)]
        public async Task CorrectActiveNgsp()
        {
            var allNgsps = await GetAllNaturalGasSellingPrices().ConfigureAwait(false);

            var lastMonth = DateTime.Now.AddMonths(-3);

            if (allNgsps.Count.Equals(1))
            {
                var command = new CalculateNaturalGasSellingPriceCommand
                {
                    Amount = 1.32M,
                    Month = lastMonth.Month,
                    Year = lastMonth.Year,
                    Remark = "Integration test correct NGSP remark"
                };

                var response = await _client
                    .PostAsync(
                        _baseUri + "CalculateNaturalGas",
                        new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json"))
                    .ConfigureAwait(false);
            }

            lastMonth = DateTime.Now.AddMonths(-1);

            var correctCommand = new CalculateNaturalGasSellingPriceCommand
            {
                Amount = 1.32M,
                Month = lastMonth.Month,
                Year = lastMonth.Year,
                Remark = "Integration test correct NGSP remark"
            };

            var correctResponse = await _client
                .PutAsync(
                    _baseUri + "CorrectActiveNaturalGas",
                    new StringContent(JsonConvert.SerializeObject(correctCommand), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            correctResponse.EnsureSuccessStatusCode();

            await CheckNewAndOldNgspAndCgnValues(
                lastMonth.Year, lastMonth.Month, correctCommand.Amount, correctCommand.Remark).ConfigureAwait(false);
        }

        private async Task<List<EconometricIndexQueryResult>> GetAllConsumerPriceIndexes()
        {
            var cpiQueryResponse = await _client.GetAsync(_baseUri + "GetConsumerPriceIndexes").ConfigureAwait(false);
            var cpiJsonResponse = await cpiQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<EconometricIndexQueryResult>>(cpiJsonResponse);
        }

        private async Task<List<EconometricIndexQueryResult>> GetAllNaturalGasSellingPrices()
        {
            var ngspQueryResponse = await _client
                .GetAsync(_baseUri + "GetNaturalGasSellingPrices").ConfigureAwait(false);
            var ngspJsonResponse = await ngspQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<EconometricIndexQueryResult>>(ngspJsonResponse);
        }

        private async Task<List<RenewableEnergySourceTariffQueryResult>> GetAllResTariffs()
        {
            var resQueryResponse = await _client
                .GetAsync(_baseUri + "GetRenewableEnergySourceTariffs").ConfigureAwait(false);
            var resJsonResponse = await resQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<RenewableEnergySourceTariffQueryResult>>(resJsonResponse);
        }

        private async Task<List<CogenerationTariffQueryResult>> GetAllCogenerationTariffs()
        {
            var cgnQueryResponse = await _client.GetAsync(_baseUri + "GetCogenerationTariffs").ConfigureAwait(false);
            var cgnJsonResponse = await cgnQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<CogenerationTariffQueryResult>>(
                cgnJsonResponse);
        }

        private async Task CheckNewAndOldNgspAndCgnValues(int year, int month, decimal newAmount, string newRemark)
        {
            var allNgsps = await GetAllNaturalGasSellingPrices().ConfigureAwait(false);

            var oldNgsp = allNgsps.Single(ngs => ngs.Until.HasValue);
            var newNgsp = allNgsps.Single(ngs => !ngs.Until.HasValue);
            const decimal oldAmount = 1.07M;
            var oldDate = new DateTime(2007, 7, 1);
            var newDate = new DateTime(year, month, 1);

            oldNgsp.Amount.Should().Be(oldAmount);
            oldNgsp.Remark.Should().Be("Initial value");
            oldNgsp.Since.Should().Be(oldDate);
            oldNgsp.Until.Should().Be(newDate);
            newNgsp.Amount.Should().Be(newAmount);
            newNgsp.Remark.Should().Be(newRemark);
            newNgsp.Since.Should().Be(newDate);
            newNgsp.Until.Should().BeNull();

            var allCgns = await GetAllCogenerationTariffs().ConfigureAwait(false);

            var oldRes = allCgns.Where(cgn => cgn.Until.HasValue).ToList();
            var newRes = allCgns.Where(cgn => !cgn.Until.HasValue).ToList();

            oldRes.Count.Should().Be(2);
            newRes.Count.Should().Be(2);

            foreach (var res in oldRes)
            {
                res.Since.Should().Be(oldDate);
                res.Until.Should().Be(newDate);
                res.NaturalGasSellingPriceAmount.Should().Be(oldAmount);
            }

            foreach (var res in newRes)
            {
                res.Since.Should().Be(newDate);
                res.Until.Should().BeNull();
                res.NaturalGasSellingPriceAmount.Should().Be(newAmount);
            }
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