using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;

namespace Acme.Seps.Domain.Base.Repository
{
    public interface IActiveSpecification<TAggregateRoot> : ISpecification<TAggregateRoot>
        where TAggregateRoot : SepsBaseEntity, IAggregateRoot
    { }
}