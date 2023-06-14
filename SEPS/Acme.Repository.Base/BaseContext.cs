using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Repository.Base;

public abstract class BaseContext : DbContext, IRepository, IUnitOfWork
{
    protected BaseContext(DbContextOptions<BaseContext> options) : base(options) { }

    IReadOnlyList<TAggregateRoot> IRepository.GetAll<TAggregateRoot>(
        BaseSpecification<TAggregateRoot> specification) =>
        QueryableResultWithIncludes(specification).Where(specification.ToExpression()).ToList();

    TAggregateRoot IRepository.GetSingle<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification) =>
        QueryableResultWithIncludes(specification).SingleOrDefault(specification.ToExpression());

    protected IQueryable<TAggregateRoot> QueryableResultWithIncludes<TAggregateRoot>(
        BaseSpecification<TAggregateRoot> specification) where TAggregateRoot : BaseEntity, IAggregateRoot =>
        specification.Includes
            .Aggregate(Set<TAggregateRoot>().AsQueryable(), (current, include) => current.Include(include));

    void IUnitOfWork.Commit() =>
        SaveChanges();

    void IUnitOfWork.Delete<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
        Set<TAggregateRoot>().Remove(aggregateRoot);

    void IUnitOfWork.Insert<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
        Set<TAggregateRoot>().Add(aggregateRoot);

    void IUnitOfWork.Update<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
        Set<TAggregateRoot>().Update(aggregateRoot);
}