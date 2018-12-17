using Acme.Seps.UseCases.Subsidy.Command.Entity;
using System.Collections.Generic;

namespace Acme.Seps.UseCases.Subsidy.Command.DomainService
{
    public interface ICogenerationParameterService
    {
        decimal GetFrom(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice naturalGasSellingPrice);
    }
}