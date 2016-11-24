using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.ValueType;

namespace Acme.Seps.Domain.Parameter.DomainService
{
    public interface ICogenerationNewPeriodService
    {
        Period GetFrom(MonthlyPeriod naturalGasSellingPricePeriod, YearlyPeriod yaepPeriod);
    }
}