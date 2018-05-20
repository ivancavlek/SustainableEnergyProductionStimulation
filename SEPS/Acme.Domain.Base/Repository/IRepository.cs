using Acme.Domain.Base.Entity;
using System.Collections.Generic;

namespace Acme.Domain.Base.Repository
{
    public interface IRepository
    {
        IReadOnlyList<TAggregateRoot> GetAll<TAggregateRoot>(ISpecification<TAggregateRoot> specification)
            where TAggregateRoot : BaseEntity, IAggregateRoot;

        TAggregateRoot GetSingle<TAggregateRoot>(ISpecification<TAggregateRoot> specification)
            where TAggregateRoot : BaseEntity, IAggregateRoot;
    }
}