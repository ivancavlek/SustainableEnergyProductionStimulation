using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CurrentActiveCogenerationTariffSpecification
        : CurrentActiveMonthlySpecification<CogenerationTariff>
    { }
}