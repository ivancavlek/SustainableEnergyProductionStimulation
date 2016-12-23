using Acme.Seps.Domain.Parameter.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Parameter.DomainService
{
    public sealed class CogenerationParameterService : ICogenerationParameterService
    {
        private const decimal _factor = 0.25M;
        private const decimal _lastQuarterGasPriceFor2006 = 1;
        private const decimal _maepPriceFor2006 = 1;

        decimal ICogenerationParameterService.GetFrom(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice naturalGasSellingPrice) =>
            GetYaepRate(yearsNaturalGasSellingPrices.Sum(y => y.Amount)) +
            GetNaturalGasSellingPriceRate(naturalGasSellingPrice.Amount);

        private static decimal GetYaepRate(decimal yaepAmount) =>
            _factor * (yaepAmount / _maepPriceFor2006);

        private static decimal GetNaturalGasSellingPriceRate(decimal naturalGasSellingPriceAmount) =>
            (1 - _factor) * (naturalGasSellingPriceAmount / _lastQuarterGasPriceFor2006);
    }
}