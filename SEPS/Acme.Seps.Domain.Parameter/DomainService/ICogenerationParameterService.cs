using Acme.Seps.Domain.Parameter.Entity;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Parameter.DomainService
{
    public interface ICogenerationParameterService
    {
        decimal GetFrom(
            IEnumerable<NaturalGasSellingPrice> yearsNaturalGasSellingPrices,
            NaturalGasSellingPrice naturalGasSellingPrice);
    }
}