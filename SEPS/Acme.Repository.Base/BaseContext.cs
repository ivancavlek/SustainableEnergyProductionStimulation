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
                .Aggregate(Set<TAggregateRoot>().AsQueryable(), (current, include) => current.Include(include));

            return queryableResultWithIncludes.Where(specification.ToExpression()).ToList();
        }

        TAggregateRoot IRepository.GetSingle<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Set<TAggregateRoot>().AsQueryable(), (current, include) => current.Include(include));

            return queryableResultWithIncludes.SingleOrDefault(specification.ToExpression());
        }

        void IUnitOfWork.Commit()
        {
            SaveChanges();
        }

        void IUnitOfWork.Delete<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Set<TAggregateRoot>().Remove(aggregateRoot);

        void IUnitOfWork.Insert<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Set<TAggregateRoot>().Add(aggregateRoot);

        void IUnitOfWork.Update<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Set<TAggregateRoot>().Update(aggregateRoot);
    }
}