using Acme.Seps.Domain.Parameter.Entity;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public interface INaturalGasSellingPriceRepository
    {
        IEnumerable<NaturalGasSellingPrice> GetAllWithin(int year);
    }
}