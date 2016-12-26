using Acme.Domain.Base.Entity;
using System.Linq;

namespace Acme.Seps.Repository.Base
{
    public interface IContext
    {
        IQueryable<TAggregateRoot> GetContext<TAggregateRoot>() where TAggregateRoot : BaseEntity, IAggregateRoot;
    }
}