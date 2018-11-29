using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class RenewableEnergySourceTariffConfiguration
        : BaseParameterConfiguration<RenewableEnergySourceTariff>,
        IEntityTypeConfiguration<RenewableEnergySourceTariff>
    {
        private readonly Guid _cpiId;
        private readonly IEnumerable<Guid> _projectIds;
        private readonly IIdentityFactory<Guid> _identityFactory;

        internal RenewableEnergySourceTariffConfiguration(
            Guid cpiId, IEnumerable<Guid> projectIds, IIdentityFactory<Guid> identityFactory)
        {
            _cpiId = cpiId;
            _projectIds = projectIds ?? throw new ArgumentNullException(nameof(projectIds));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        public override void Configure(EntityTypeBuilder<RenewableEnergySourceTariff> builder)
        {
            base.Configure(builder);

            foreach (var projectId in _projectIds)
            {
                var id = _identityFactory.CreateIdentity();

                builder.HasData(new
                {
                    Id = id,
                    LowerProductionLimit = 100,
                    UpperProductionLimit = 200,
                    LowerRate = 10M,
                    HigherRate = 20M,
                    ProjectTypeId = projectId,
                    TarrifType = nameof(RenewableEnergySourceTariff),
                    ConsumerPriceIndexId = _cpiId
                });
                builder.OwnsOne(vte => vte.Period, vte =>
                {
                    vte.HasData(new
                    {
                        TariffId = id,
                        ValidFrom = new DateTimeOffset(new DateTime(2016, 01, 01)),
                        ValidTill = new DateTimeOffset(new DateTime(2017, 01, 01))
                    });
                });
            }
        }
    }
}