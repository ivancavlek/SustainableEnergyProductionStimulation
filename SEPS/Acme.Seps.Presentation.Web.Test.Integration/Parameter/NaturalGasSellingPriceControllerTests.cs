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
    public class NaturalGasSellingPriceControllerTests
        : ParameterBaseControllerTests, IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
    {
        public NaturalGasSellingPriceControllerTests(IntegrationTestingWebApplicationFactory<Startup> factory)
            : base(factory) =>
            _baseUri += "NaturalGasSellingPrice/";

        [Fact, TestPriority(1)]
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

        [Fact, TestPriority(2)]
        public async Task CalculateNaturalGas()
        {
            var command = new CalculateNewNaturalGasSellingPriceCommand
            {
                Amount = 1.32M,
                Month = _lastMonth.Month,
                Year = _lastMonth.Year,
                Remark = "Integration test calculate NGSP remark"
            };

            var response = await _client
                .PostAsync(
                    _baseUri + "CalculateNaturalGas",
                    new StringContent(JsonConvert.SerializeObject(command), _encoding, _contentType))
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            await CheckNewAndOldNgspAndCgnValues(
                _lastMonth.Year, _lastMonth.Month, command.Amount, command.Remark).ConfigureAwait(false);
        }

        [Fact, TestPriority(3)]
        public async Task CorrectActiveNgsp()
        {
            var correctCommand = new CorrectActiveNaturalGasSellingPriceCommand
            {
                Amount = 1.5M,
                Month = _lastMonth.Month,
                Year = _lastMonth.Year,
                Remark = "Integration test correct NGSP remark"
            };

            var correctResponse = await _client
                .PutAsync(
                    _baseUri + "CorrectActiveNaturalGas",
                    new StringContent(JsonConvert.SerializeObject(correctCommand), _encoding, _contentType))
                .ConfigureAwait(false);

            correctResponse.EnsureSuccessStatusCode();

            await CheckNewAndOldNgspAndCgnValues(
                _lastMonth.Year, _lastMonth.Month, correctCommand.Amount, correctCommand.Remark).ConfigureAwait(false);
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

            newRes.Count.Should().Be(2);

            foreach (var res in oldRes)
            {
                res.Until.Should().Be(newDate);
            }

            foreach (var res in newRes)
            {
                res.Since.Should().Be(newDate);
                res.Until.Should().BeNull();
                res.NaturalGasSellingPriceAmount.Should().Be(newAmount);
            }
        }

        private async Task<List<EconometricIndexQueryResult>> GetAllNaturalGasSellingPrices()
        {
            var ngspQueryResponse = await _client
                .GetAsync(_baseUri + "GetNaturalGasSellingPrices").ConfigureAwait(false);
            var ngspJsonResponse = await ngspQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<EconometricIndexQueryResult>>(ngspJsonResponse);
        }
    }
}
