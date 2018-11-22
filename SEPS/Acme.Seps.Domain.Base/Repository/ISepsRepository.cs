using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Entity;

namespace Acme.Seps.Domain.Base.Repository
{
    public interface ISepsRepository : IRepository
    {
        TAggregateRoot GetLatest<TAggregateRoot>()
            where TAggregateRoot : SepsBaseAggregate;
    }
}