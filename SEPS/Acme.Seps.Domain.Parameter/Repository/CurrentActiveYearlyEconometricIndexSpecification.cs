using Acme.Seps.Domain.Base.Repository;
using Acme.Seps.Domain.Parameter.Entity;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class CurrentActiveYearlyEconometricIndexSpecification<TYearlyEconometicIndex>
        : CurrentActiveYearlySpecification<TYearlyEconometicIndex>
        where TYearlyEconometicIndex : YearlyEconometricIndex<TYearlyEconometicIndex> { }
}