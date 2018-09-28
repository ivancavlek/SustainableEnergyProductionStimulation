using System.Collections.Generic;
using System.Linq;
using Acme.Domain.Base.Repository;
using Microsoft.EntityFrameworkCore;

namespace Acme.Repository.Base
{
    public abstract class BaseContext : DbContext, IRepository, IUnitOfWork
    {
        IReadOnlyList<TAggregateRoot> IRepository.GetAll<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Set<TAggregateRoot>().AsNoTracking(), (current, include) => current.Include(include));

            return queryableResultWithIncludes.Where(specification.ToExpression()).ToList();
        }

        TAggregateRoot IRepository.GetSingle<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Set<TAggregateRoot>().AsNoTracking(), (current, include) => current.Include(include));

            return queryableResultWithIncludes.SingleOrDefault(specification.ToExpression());
        }

        void IUnitOfWork.Commit()
        {
            SaveChanges();

            // vidjeti ima li smisla i upariti sa pravim Lifetimeom te paziti na razne vrste app (web, desk)
            foreach (var entry in ChangeTracker.Entries())
            {
                Entry(entry.Entity).State = EntityState.Detached;
            }
        }

        void IUnitOfWork.Delete<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Deleted;

        void IUnitOfWork.Insert<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Added;

        void IUnitOfWork.Update<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Modified;
    }
}