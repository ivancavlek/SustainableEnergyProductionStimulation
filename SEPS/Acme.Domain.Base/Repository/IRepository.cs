using Acme.Domain.Base.Entity;
using System.Collections.Generic;

namespace Acme.Domain.Base.Repository
{
    public interface IRepository<TAggregateRoot> where TAggregateRoot : BaseEntity, IAggregateRoot
    {
        IReadOnlyList<TAggregateRoot> Get(ISpecification<TAggregateRoot> specification);
    }
}