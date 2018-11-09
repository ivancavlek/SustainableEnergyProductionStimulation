using Acme.Seps.Domain.Parameter.Entity;
using Microsoft.EntityFrameworkCore;

namespace Acme.Seps.Repository.Parameter.Configuration
{
    public class CogenerationTariffConfiguration
        : BaseParameterConfiguration<CogenerationTariff>, IEntityTypeConfiguration<CogenerationTariff>
    {
    }
}