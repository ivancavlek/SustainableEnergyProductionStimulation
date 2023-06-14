using Acme.Domain.Base.Entity;

namespace Acme.Domain.Base.Repository;

public interface IUnitOfWork
{
    void Commit();

    void Delete<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : BaseEntity, IAggregateRoot;

    void Insert<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : BaseEntity, IAggregateRoot;

    void Update<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : BaseEntity, IAggregateRoot;
}