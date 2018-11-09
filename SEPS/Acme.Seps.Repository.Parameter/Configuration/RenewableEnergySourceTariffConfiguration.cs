using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class RenewableEnergySourceTariffConfiguration
        : BaseParameterConfiguration<RenewableEnergySourceTariff>,
        IEntityTypeConfiguration<RenewableEnergySourceTariff>
    {
    }
}