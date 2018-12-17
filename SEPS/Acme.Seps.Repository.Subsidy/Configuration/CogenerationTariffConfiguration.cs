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
    internal sealed class CogenerationTariffConfiguration
        : BaseParameterConfiguration<CogenerationTariff>, IEntityTypeConfiguration<CogenerationTariff>
    {
        private readonly Guid _naturalGasSellingPriceId;
        private readonly IEnumerable<Guid> _projectIds;
        private readonly IIdentityFactory<Guid> _identityFactory;

        internal CogenerationTariffConfiguration(
            Guid naturalGasSellingPriceId, IEnumerable<Guid> projectIds, IIdentityFactory<Guid> identityFactory)
        {
            _naturalGasSellingPriceId = naturalGasSellingPriceId;
            _projectIds = projectIds ?? throw new ArgumentNullException(nameof(projectIds));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        public override void Configure(EntityTypeBuilder<CogenerationTariff> builder)
        {
            base.Configure(builder);
            SeedData(builder);
        }

        private void SeedData(EntityTypeBuilder<CogenerationTariff> builder)
        {
            var id = _identityFactory.CreateIdentity();

            builder.HasData(new
            {
                Id = id,
                LowerProductionLimit = 0M,
                UpperProductionLimit = 0.26M,
                LowerRate = 0.51M,
                HigherRate = 1M,
                TarrifType = nameof(CogenerationTariff),
                ProjectTypeId = _projectIds.ElementAt(10),
                NaturalGasSellingPriceId = _naturalGasSellingPriceId
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
                UpperProductionLimit = 0.15M,
                LowerRate = 0.30M,
                HigherRate = 1M,
                TarrifType = nameof(CogenerationTariff),
                ProjectTypeId = _projectIds.ElementAt(11),
                NaturalGasSellingPriceId = _naturalGasSellingPriceId
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