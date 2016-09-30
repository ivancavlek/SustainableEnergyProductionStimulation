namespace Acme.Seps.Domain.Base.Factory
{
    public interface IIdentityFactory<out TKey>
    {
        TKey CreateIdentity();
    }
}