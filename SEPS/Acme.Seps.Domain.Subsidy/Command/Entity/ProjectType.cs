using Acme.Seps.Domain.Base.Entity;
using System.Collections.Generic;

namespace Acme.Seps.Domain.Subsidy.Command.Entity
{
    public class ProjectType : SepsAggregateRoot
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string ContractLabel { get; private set; }
        public bool ConsumesFuel { get; private set; }
        public ProjectTypeGroup ProjectTypeGroup { get; private set; }
        public ICollection<Tariff> Tariffs { get; private set; }
        public ICollection<ProjectType> SubordinateProjectTypes { get; private set; }

        protected ProjectType() { }
    }
}