namespace Acme.Domain.Base.Factory
{
    public interface IIdentityFactory<out TKey>
    {
        TKey CreateIdentity();
    }
}