using Acme.Domain.Base.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Repository.Base
{
    public abstract class BaseContext : DbContext, IRepository, IUnitOfWork
    {
        protected BaseContext()
        {
        }

        IReadOnlyList<TAggregateRoot> IRepository.GetAll<TAggregateRoot>(ISpecification<TAggregateRoot> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Set<TAggregateRoot>().AsNoTracking(), (current, include) => current.Include(include));

            return queryableResultWithIncludes.Where(specification.ToExpression()).ToList();
        }

        TAggregateRoot IRepository.GetSingle<TAggregateRoot>(ISpecification<TAggregateRoot> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Set<TAggregateRoot>().AsNoTracking(), (current, include) => current.Include(include));

            return queryableResultWithIncludes.SingleOrDefault(specification.ToExpression());
        }

        void IUnitOfWork.Commit() => SaveChanges();

        void IUnitOfWork.Delete<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Deleted;

        void IUnitOfWork.Insert<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Added;

        void IUnitOfWork.Update<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Modified;
    }
}