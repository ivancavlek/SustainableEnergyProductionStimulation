namespace Acme.Seps.Domain.Parameter.DomainService
{
    public class CogenerationParameterService : ICogenerationParameterService
    {
        private const decimal _factor = 0.25M;

        decimal ICogenerationParameterService.GetFrom(
            decimal yaepAmount, decimal naturalGasSellingPriceAmount) =>
            GetYaepRate(yaepAmount, decimal.Parse(Infrastructure.Parameter.MaepPriceFor2006)) +
                GetNaturalGasSellingPriceRate(
                    naturalGasSellingPriceAmount, decimal.Parse(Infrastructure.Parameter.LastQuarterGasPriceFor2006));

        private static decimal GetYaepRate(decimal yaepAmount, decimal maepPriceFor2006) =>
            _factor * (yaepAmount / maepPriceFor2006);

        private static decimal GetNaturalGasSellingPriceRate(
            decimal naturalGasSellingPriceAmount, decimal lastQuarterGasPriceFor2006) =>
            (1 - _factor) * (naturalGasSellingPriceAmount / lastQuarterGasPriceFor2006);
    }
}