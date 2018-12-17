using Acme.Seps.Domain.Subsidy.Command.Entity;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Factory
{
    internal interface ITariffFactory<TTariff> where TTariff : Tariff
    {
        TTariff Create();
    }
}