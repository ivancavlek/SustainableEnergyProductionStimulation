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

namespace Acme.Seps.Presentation.Web.Test.Integration.Parameter;

[TestCaseOrderer(
    "Acme.Seps.Presentation.Web.Test.Integration.TestUtility.TestCollectionOrderer",
    "Acme.Seps.Presentation.Web.Test.Integration")]
public class AverageElectricEnergyProductionPriceControllerTests
    : ParameterBaseControllerTests, IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
{
    public AverageElectricEnergyProductionPriceControllerTests(IntegrationTestingWebApplicationFactory<Startup> factory)
        : base(factory) =>
        _baseUri += "AverageElectricEnergyProductionPrice/";

    [Fact, TestPriority(1)]
    public async Task GetAverageElectricEnergyProductionPrices()
    {
        var allAeepps = await GetAllAverageElectricEnergyProductionPrices().ConfigureAwait(false);

        allAeepps.Count.Should().Be(1);
        foreach (var cpi in allAeepps)
        {
            cpi.Amount.Should().Be(0.2625M);
            cpi.Remark.Should().Be("Initial value");
            cpi.Since.Should().Be(new DateTime(2007, 7, 1));
            cpi.Until.Should().BeNull();
        }
    }

    [Fact, TestPriority(2)]
    public async Task CalculateAverageElectricEnergyProductionPrice()
    {
        var command = new CalculateNewAverageElectricEnergyProductionPriceCommand
        {
            Amount = 0.5M,
            Month = _lastMonth.Month,
            Year = _lastMonth.Year,
            Remark = "Integration test calculate AEEPP remark"
        };

        var response = await _client
            .PostAsync(
                _baseUri + "CalculateAverageElectricEnergyProductionPrice",
                new StringContent(JsonConvert.SerializeObject(command), _encoding, _contentType))
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        await CheckNewAndOldAeeppAndCgnValues(
            _lastMonth.Year, _lastMonth.Month, command.Amount, command.Remark).ConfigureAwait(false);
    }

    [Fact, TestPriority(3)]
    public async Task CorrectActiveAeepp()
    {
        var correctCommand = new CorrectActiveAverageElectricEnergyProductionPriceCommand
        {
            Amount = 0.75M,
            Month = _lastMonth.Month,
            Year = _lastMonth.Year,
            Remark = "Integration test correct AEEPP remark"
        };

        var correctResponse = await _client
            .PutAsync(
                _baseUri + "CorrectActiveAverageElectricEnergyProductionPrice",
                new StringContent(JsonConvert.SerializeObject(correctCommand), _encoding, _contentType))
            .ConfigureAwait(false);

        correctResponse.EnsureSuccessStatusCode();

        await CheckNewAndOldAeeppAndCgnValues(
            _lastMonth.Year, _lastMonth.Month, correctCommand.Amount, correctCommand.Remark).ConfigureAwait(false);
    }

    private async Task CheckNewAndOldAeeppAndCgnValues(int year, int month, decimal newAmount, string newRemark)
    {
        var allAeepps = await GetAllAverageElectricEnergyProductionPrices().ConfigureAwait(false);

        var oldAeepp = allAeepps.Single(ngs => ngs.Until.HasValue);
        var newAeepp = allAeepps.Single(ngs => !ngs.Until.HasValue);
        const decimal oldAmount = 0.2625M;
        var oldDate = new DateTime(2007, 7, 1);
        var newDate = new DateTime(year, month, 1);

        oldAeepp.Amount.Should().Be(oldAmount);
        oldAeepp.Remark.Should().Be("Initial value");
        oldAeepp.Since.Should().Be(oldDate);
        oldAeepp.Until.Should().Be(newDate);
        newAeepp.Amount.Should().Be(newAmount);
        newAeepp.Remark.Should().Be(newRemark);
        newAeepp.Since.Should().Be(newDate);
        newAeepp.Until.Should().BeNull();

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
            res.AverageElectricEnergyProductionPriceAmount.Should().Be(newAmount);
        }
    }

    private async Task<List<EconometricIndexQueryResult>> GetAllAverageElectricEnergyProductionPrices()
    {
        var aeeppQueryResponse = await _client
            .GetAsync(_baseUri + "GetAverageElectricEnergyProductionPrices").ConfigureAwait(false);
        var aeeppJsonResponse = await aeeppQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonConvert.DeserializeObject<List<EconometricIndexQueryResult>>(aeeppJsonResponse);
    }
}
