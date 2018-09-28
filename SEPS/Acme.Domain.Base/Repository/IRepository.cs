using System.Collections.Generic;
using Acme.Domain.Base.Entity;

namespace Acme.Domain.Base.Repository
{
    public interface IRepository
    {
        IReadOnlyList<TAggregateRoot> GetAll<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
            where TAggregateRoot : BaseEntity, IAggregateRoot;

        TAggregateRoot GetSingle<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
            where TAggregateRoot : BaseEntity, IAggregateRoot;
    }
}