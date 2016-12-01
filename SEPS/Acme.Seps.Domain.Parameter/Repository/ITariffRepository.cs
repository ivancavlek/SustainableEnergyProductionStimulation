using Acme.Seps.Domain.Parameter.Entity;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public interface ITariffRepository
    {
        IEnumerable<TTariff> GetActive<TTariff>() where TTariff : Tariff;
    }
}