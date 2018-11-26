using Acme.Seps.Domain.Subsidy.Entity;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.DomainService
{
    public interface ICogenerationParameterService
    {
        decimal GetFrom(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice naturalGasSellingPrice);
    }
}