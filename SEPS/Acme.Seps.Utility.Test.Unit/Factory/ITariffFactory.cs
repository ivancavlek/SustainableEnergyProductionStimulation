using Acme.Seps.Domain.Subsidy.Entity;

namespace Acme.Seps.Utility.Test.Unit.Factory
{
    public interface ITariffFactory<TTariff> where TTariff : Tariff
    {
        TTariff Create();
    }
}