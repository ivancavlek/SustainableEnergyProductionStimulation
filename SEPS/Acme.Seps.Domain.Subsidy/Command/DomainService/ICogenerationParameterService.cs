using Acme.Seps.Domain.Subsidy.Command.Entity;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Command.DomainService
{
    public interface ICogenerationParameterService
    {
        decimal GetFrom(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice naturalGasSellingPrice);
    }
}