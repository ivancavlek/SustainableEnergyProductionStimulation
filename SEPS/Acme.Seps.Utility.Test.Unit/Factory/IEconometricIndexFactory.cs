using Acme.Seps.Domain.Subsidy.Entity;

namespace Acme.Seps.Test.Unit.Utility.Factory
{
    public interface IEconometricIndexFactory<TEconometricIndex> where TEconometricIndex : EconometricIndex
    {
        TEconometricIndex Create();
    }
}