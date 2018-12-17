using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.UseCases.Subsidy.Command.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Repository.Subsidy.Configuration
{
    internal sealed class ProjectTypeConfiguration
        : BaseParameterConfiguration<ProjectType>, IEntityTypeConfiguration<ProjectType>
    {
        private readonly IEnumerable<Guid> _ids;

        internal ProjectTypeConfiguration(IEnumerable<Guid> ids)
        {
            _ids = ids ?? throw new ArgumentNullException(nameof(ids));
        }

        public override void Configure(EntityTypeBuilder<ProjectType> builder)
        {
            base.Configure(builder);
            ConfigureProperties(builder);
            ConfigureRelationships(builder);
            SeedData(builder);
        }

        private static void ConfigureProperties(EntityTypeBuilder<ProjectType> builder)
        {
            builder.Property(ppy => ppy.Code).IsRequired().HasMaxLength(10);
            builder.Property(ppy => ppy.ContractLabel).IsRequired().HasMaxLength(10);
            builder.Property(ppy => ppy.Name).IsRequired().HasMaxLength(100);
            builder.Property(ppy => ppy.ProjectTypeGroup).IsRequired().HasMaxLength(25);
            builder.Property(ppy => ppy.ProjectTypeGroup)
                .HasConversion(
                    dbe => dbe.ToString(),
                    apn => (ProjectTypeGroup)Enum.Parse(typeof(ProjectTypeGroup), apn));
            builder.Property<Guid?>("SuperiorProjectTypeId");
        }

        private static void ConfigureRelationships(EntityTypeBuilder<ProjectType> builder)
        {
            builder
                .HasMany(ppy => ppy.SubordinateProjectTypes)
                .WithOne()
                .HasForeignKey("SuperiorProjectTypeId");
            //.OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(ppy => ppy.Tariffs)
                .WithOne()
                .HasForeignKey(ppy => ppy.ProjectTypeId);
        }

        private void SeedData(EntityTypeBuilder<ProjectType> builder)
        {
            builder.HasData(new
            {
                Id = _ids.ElementAt(0),
                Name = "Solar power plant",
                Code = "1.",
                ContractLabel = "SE",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(0),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(1),
                Name = "Solar power plant > 0kW i <= 10kW",
                Code = "1.1.",
                ContractLabel = "SPP",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy,
                SuperiorProjectTypeId = _ids.ElementAt(0)
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(1),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(2),
                Name = "Solar power plant > 10kW i <= 30kW",
                Code = "1.2.",
                ContractLabel = "SPP",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy,
                SuperiorProjectTypeId = _ids.ElementAt(0)
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(2),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(3),
                Name = "Solar power plant > 30kW",
                Code = "1.3.",
                ContractLabel = "SPP",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy,
                SuperiorProjectTypeId = _ids.ElementAt(0)
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(3),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(4),
                Name = "Hydroelectric power plant",
                Code = "2.",
                ContractLabel = "HPP",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(4),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(5),
                Name = "Wind power plant",
                Code = "3.",
                ContractLabel = "WPP",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(5),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(6),
                Name = "Biomass power plant",
                Code = "4.",
                ContractLabel = "BPP",
                ConsumesFuel = true,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(6),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(7),
                Name = "Biomass power plant from forest",
                Code = "4.1.",
                ContractLabel = "BPP",
                ConsumesFuel = true,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy,
                SuperiorProjectTypeId = _ids.ElementAt(6)
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(7),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(8),
                Name = "Biomass power plant from forest leftovers",
                Code = "4.2.",
                ContractLabel = "BPP",
                ConsumesFuel = true,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy,
                SuperiorProjectTypeId = _ids.ElementAt(6)
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(8),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(9),
                Name = "Hydroelectric power plant",
                Code = "6.",
                ContractLabel = "HE",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.RenewableEnergy
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(9),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(10),
                Name = "Small cogeneration",
                Code = "7.",
                ContractLabel = "COGEN",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.Cogeneration
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(10),
                    Since = SepsVersion.InitialDate()
                });
            });

            builder.HasData(new
            {
                Id = _ids.ElementAt(11),
                Name = "Large cogeneration",
                Code = "8.",
                ContractLabel = "COGEN",
                ConsumesFuel = false,
                ProjectTypeGroup = ProjectTypeGroup.Cogeneration
            });
            builder.OwnsOne(vte => vte.Active, vte =>
            {
                vte.HasData(new
                {
                    ProjectTypeId = _ids.ElementAt(11),
                    Since = SepsVersion.InitialDate()
                });
            });
        }
    }
}