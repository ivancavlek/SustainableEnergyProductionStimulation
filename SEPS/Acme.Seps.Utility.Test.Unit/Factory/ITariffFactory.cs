using Acme.Seps.Domain.Subsidy.Entity;

namespace Acme.Seps.Test.Unit.Utility.Factory
{
    public interface ITariffFactory<TTariff> where TTariff : Tariff
    {
        TTariff Create();
    }
}