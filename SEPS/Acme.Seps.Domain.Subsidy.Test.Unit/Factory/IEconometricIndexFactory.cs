using Acme.Seps.Domain.Subsidy.Entity;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Factory
{
    internal interface IEconometricIndexFactory<TEconometricIndex> where TEconometricIndex : EconometricIndex
    {
        TEconometricIndex Create();
    }
}