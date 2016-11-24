using Acme.Domain.Base.ValueType;
using Acme.Seps.Domain.Base.ValueType;

namespace Acme.Seps.Domain.Parameter.DomainService
{
    public class CogenerationNewPeriodService : ICogenerationNewPeriodService
    {
        public Period GetFrom(MonthlyPeriod naturalGasSellingPricePeriod, YearlyPeriod yaepPeriod) =>
            naturalGasSellingPricePeriod.ValidFrom < yaepPeriod.ValidTill.Value ?
                yaepPeriod as Period :
                naturalGasSellingPricePeriod as Period;
    }
}