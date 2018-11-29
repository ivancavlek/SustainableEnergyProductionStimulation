using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Subsidy.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

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

            var random = new Random();

            foreach (var projectId in _projectIds)
            {
                var id = _identityFactory.CreateIdentity();

                builder.HasData(new
                {
                    Id = id,
                    LowerProductionLimit = random.Next(250, 500),
                    UpperProductionLimit = random.Next(650, 1000),
                    LowerRate = new decimal(random.Next(250, 500)),
                    HigherRate = new decimal(random.Next(750, 900)),
                    TarrifType = nameof(CogenerationTariff),
                    ProjectTypeId = projectId,
                    NaturalGasSellingPriceId = _naturalGasSellingPriceId
                });
                builder.OwnsOne(vte => vte.Period, vte =>
                {
                    vte.HasData(new
                    {
                        TariffId = id,
                        ValidFrom = new DateTimeOffset(new DateTime(2007, 07, 01))
                    });
                });
            }
        }
    }
}