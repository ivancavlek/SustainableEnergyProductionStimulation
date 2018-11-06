using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CurrentActiveMonthlyEconometricIndexSpecification<TMonthlyEconometicIndex>
        : CurrentActiveMonthlySpecification<TMonthlyEconometicIndex>
        where TMonthlyEconometicIndex : MonthlyEconometricIndex<TMonthlyEconometicIndex> { }
}