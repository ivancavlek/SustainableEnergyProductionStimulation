using Acme.Domain.Base.Entity;
using System.Collections.Generic;

namespace Acme.Domain.Base.Repository
{
    public interface IRepository<TAggregateRoot> where TAggregateRoot : BaseEntity, IAggregateRoot
    {
        TAggregateRoot GetSingle(ISpecification<TAggregateRoot> specification);

        IReadOnlyList<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification);
    }
}