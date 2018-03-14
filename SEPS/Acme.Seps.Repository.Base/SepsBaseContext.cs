using Acme.Domain.Base.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Acme.Seps.Repository.Base
{
    public abstract class SepsBaseContext : DbContext, IContext, IUnitOfWork
    {
        protected SepsBaseContext()
        //: base("Seps")
        {
            // Hack: this line is needed to ensure that Entity Framework.SqlServer is referenced in every presentation BIN folder for Unity DI
            //var ensureDllIsCopied = SqlProviderServices.Instance;
        }

        IQueryable<TAggregateRoot> IContext.GetContext<TAggregateRoot>() => Set<TAggregateRoot>();

        void IUnitOfWork.Commit() => SaveChanges();

        void IUnitOfWork.Delete<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Deleted;

        void IUnitOfWork.Insert<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Added;

        void IUnitOfWork.Update<TAggregateRoot>(TAggregateRoot aggregateRoot) =>
            Entry(aggregateRoot).State = EntityState.Modified;
    }
}