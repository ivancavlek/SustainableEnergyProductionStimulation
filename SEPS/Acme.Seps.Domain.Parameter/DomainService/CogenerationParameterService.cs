using Acme.Seps.Domain.Parameter.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.DomainService
{
    public sealed class CogenerationParameterService : ICogenerationParameterService
    {
        private const decimal _factor = 0.25M;

        decimal ICogenerationParameterService.GetFrom(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice naturalGasSellingPrice) =>
            GetYaepRate(
                yearsNaturalGasSellingPrices.Sum(y => y.Amount),
                decimal.Parse(Infrastructure.Parameter.MaepPriceFor2006)) +
            GetNaturalGasSellingPriceRate(
                naturalGasSellingPrice.Amount,
                decimal.Parse(Infrastructure.Parameter.LastQuarterGasPriceFor2006));

        private static decimal GetYaepRate(decimal yaepAmount, decimal maepPriceFor2006) =>
            _factor * (yaepAmount / maepPriceFor2006);

        private static decimal GetNaturalGasSellingPriceRate(
            decimal naturalGasSellingPriceAmount, decimal lastQuarterGasPriceFor2006) =>
            (1 - _factor) * (naturalGasSellingPriceAmount / lastQuarterGasPriceFor2006);
    }
}