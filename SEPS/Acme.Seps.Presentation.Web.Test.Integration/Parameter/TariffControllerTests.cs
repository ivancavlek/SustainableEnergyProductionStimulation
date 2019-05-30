using Acme.Seps.Presentation.Web.Test.Integration.TestUtility;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Acme.Seps.Presentation.Web.Test.Integration.Parameter
{
    [TestCaseOrderer(
        "Acme.Seps.Presentation.Web.Test.Integration.TestUtility.TestCollectionOrderer",
        "Acme.Seps.Presentation.Web.Test.Integration")]
    public class TariffControllerTests
        : ParameterBaseControllerTests, IClassFixture<IntegrationTestingWebApplicationFactory<Startup>>
    {
        public TariffControllerTests(IntegrationTestingWebApplicationFactory<Startup> factory) : base(factory) =>
            _baseUri = "Tariff/";

        [Fact, TestPriority(1)]
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

        [Fact, TestPriority(2)]
        public async Task GetCogenerationTariffs()
        {
            var allCgns = await GetAllCogenerationTariffs().ConfigureAwait(false);

            allCgns.Count.Should().Be(2);
            foreach (var cgn in allCgns)
            {
                cgn.NaturalGasSellingPriceAmount.Should().Be(1.07M);
                cgn.AverageElectricEnergyProductionPriceAmount.Should().Be(0.2625M);
                cgn.Since.Should().Be(new DateTime(2007, 7, 1));
                cgn.Until.Should().BeNull();
            }
        }
    }
}