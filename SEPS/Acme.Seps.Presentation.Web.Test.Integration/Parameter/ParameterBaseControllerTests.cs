using Acme.Seps.Presentation.Web.Test.Integration.TestUtility;
using Acme.Seps.UseCases.Subsidy.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Acme.Seps.Presentation.Web.Test.Integration.Parameter;

public class ParameterBaseControllerTests
{
    protected readonly Encoding _encoding;
    protected readonly string _contentType;
    protected readonly HttpClient _client;
    protected readonly DateTime _lastMonth;

    protected string _baseUri;

    protected ParameterBaseControllerTests(IntegrationTestingWebApplicationFactory<Startup> factory)
    {
        _baseUri = "/api/Parameter/";
        _encoding = Encoding.UTF8;
        _contentType = "application/json";
        _client = factory.CreateClient();
        _lastMonth = DateTime.Today.AddMonths(-3);
    }

    protected async Task<List<RenewableEnergySourceTariffQueryResult>> GetAllResTariffs()
    {
        var resQueryResponse = await _client
            .GetAsync(_baseUri + "GetRenewableEnergySourceTariffs").ConfigureAwait(false);
        var resJsonResponse = await resQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonConvert.DeserializeObject<List<RenewableEnergySourceTariffQueryResult>>(resJsonResponse);
    }

    protected async Task<List<CogenerationTariffQueryResult>> GetAllCogenerationTariffs()
    {
        var cgnQueryResponse = await _client.GetAsync(_baseUri + "GetCogenerationTariffs").ConfigureAwait(false);
        var cgnJsonResponse = await cgnQueryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonConvert.DeserializeObject<List<CogenerationTariffQueryResult>>(
            cgnJsonResponse);
    }
}