using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Repository.Base.Repository
{
    public class SepsRepository<TAggregateRoot> : SepsBaseRepository, IRepository<TAggregateRoot>
        where TAggregateRoot : BaseEntity, IAggregateRoot
    {
        public SepsRepository(IContext context)
            : base(context) { }

        IReadOnlyList<TAggregateRoot> IRepository<TAggregateRoot>.GetAll(ISpecification<TAggregateRoot> specification) =>
            Context.GetContext<TAggregateRoot>().Where(specification.ToExpression()).ToList();

        TAggregateRoot IRepository<TAggregateRoot>.GetSingle(ISpecification<TAggregateRoot> specification) =>
            Context.GetContext<TAggregateRoot>().SingleOrDefault(specification.ToExpression());
    }
}