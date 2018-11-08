using Acme.Domain.Base.Repository;
using NHibernate.Cfg;
using System.Collections.Generic;

namespace Acme.Repository.Base.NHibernate
{
    public abstract class BaseContext : IRepository, IUnitOfWork
    {
        public BaseContext(Configuration configuration)
        {

        }

        void IUnitOfWork.Commit()
        {
            throw new System.NotImplementedException();
        }

        void IUnitOfWork.Delete<TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            throw new System.NotImplementedException();
        }

        IReadOnlyList<TAggregateRoot> IRepository.GetAll<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
        {
            throw new System.NotImplementedException();
        }

        TAggregateRoot IRepository.GetSingle<TAggregateRoot>(BaseSpecification<TAggregateRoot> specification)
        {
            throw new System.NotImplementedException();
        }

        void IUnitOfWork.Insert<TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            throw new System.NotImplementedException();
        }

        void IUnitOfWork.Update<TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            throw new System.NotImplementedException();
        }
    }
}