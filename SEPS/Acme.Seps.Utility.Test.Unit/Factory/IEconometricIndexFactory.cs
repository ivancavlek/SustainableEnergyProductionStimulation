using Acme.Seps.Domain.Subsidy.Entity;

namespace Acme.Seps.Utility.Test.Unit.Factory
{
    public interface IEconometricIndexFactory<TEconometricIndex> where TEconometricIndex : EconometricIndex
    {
        TEconometricIndex Create();
    }
}