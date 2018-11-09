using Acme.Domain.Base.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Repository.Base
{
    public abstract class BaseContext : DbContext, IRepository, IUnitOfWork
    {
        public BaseContext(DbContextOptions<BaseContext> options) : base(options) { }

        IReadOnlyList<TAggregateRoot> IRepository.GetAll<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Set<TAggregateRoot>().AsQueryable()/*AsNoTracking()*/, (current, include) => current.Include(include));

            return queryableResultWithIncludes.Where(specification.ToExpression()).ToList();
        }

        TAggregateRoot IRepository.GetSingle<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Set<TAggregateRoot>().AsQueryable()/*AsNoTracking()*/, (current, include) => current.Include(include));

            return queryableResultWithIncludes.SingleOrDefault(specification.ToExpression());
        }

        void IUnitOfWork.Commit()
        {
            SaveChanges();

            // vidjeti ima li smisla i upariti sa pravim Lifetimeom te paziti na razne vrste app (web, desk)
            //foreach (var entry in ChangeTracker.Entries())
            //{
            //    Entry(entry.Entity).State = EntityState.Detached;
            //}
        }

        void IUnitOfWork.Delete<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            //Entry(aggregateRoot).State = EntityState.Deleted;
            Set<TAggregateRoot>().Remove(aggregateRoot);

        void IUnitOfWork.Insert<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            //Entry(aggregateRoot).State = EntityState.Added;
            Set<TAggregateRoot>().Add(aggregateRoot);

        void IUnitOfWork.Update<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            //Entry(aggregateRoot).State = EntityState.Modified;
            Set<TAggregateRoot>().Update(aggregateRoot);
    }
}