using Acme.Seps.Domain.Parameter.Entity;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public interface IEconometricIndexRepository
    {
        TEconometricIndex GetActive<TEconometricIndex>() where TEconometricIndex : EconometricIndex;
    }
}