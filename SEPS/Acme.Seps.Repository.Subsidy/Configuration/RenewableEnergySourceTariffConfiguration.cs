using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

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
            SeedData(builder);
        }

        private void SeedData(EntityTypeBuilder<RenewableEnergySourceTariff> builder)
        {
            var id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerRate = 0M,
                HigherRate = 3.40M,
                ProjectTypeId = _projectIds.ElementAt(1),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerRate = 0M,
                HigherRate = 3.00M,
                ProjectTypeId = _projectIds.ElementAt(2),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerRate = 0M,
                HigherRate = 0.69M,
                ProjectTypeId = _projectIds.ElementAt(4),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerRate = 0M,
                HigherRate = 0.64M,
                ProjectTypeId = _projectIds.ElementAt(5),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerRate = 0M,
                HigherRate = 1.20M,
                ProjectTypeId = _projectIds.ElementAt(7),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerRate = 0M,
                HigherRate = 0.95M,
                ProjectTypeId = _projectIds.ElementAt(8),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerProductionLimit = 0M,
                UpperProductionLimit = 5000M,
                LowerRate = 0M,
                HigherRate = 0.69M,
                ProjectTypeId = _projectIds.ElementAt(9),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerProductionLimit = 5000M,
                UpperProductionLimit = 15000M,
                LowerRate = 0M,
                HigherRate = 0.55M,
                ProjectTypeId = _projectIds.ElementAt(9),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });

            id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerProductionLimit = 15000M,
                LowerRate = 0M,
                HigherRate = 0.42M,
                ProjectTypeId = _projectIds.ElementAt(9),
                TarrifType = nameof(RenewableEnergySourceTariff),
                ConsumerPriceIndexId = _cpiId
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    TariffId = id,
                    Since = SepsVersion.InitialDate()
                });
            });
        }
    }
}