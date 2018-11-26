using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Base.ValueType;

namespace Acme.Seps.Domain.Subsidy.Entity
{
    public class ProjectType : SepsAggregate
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string ContractLabel { get; private set; }
        public bool ConsumesFuel { get; private set; }
        public ProjectType SuperiorProjectType { get; private set; }

        protected ProjectType() { }

        public void PermanentlyDisable()
        {
            Period = new Period(new MonthlyPeriodFactory(Period.ValidFrom, SystemTime.CurrentMonth()));
        }
    }
}