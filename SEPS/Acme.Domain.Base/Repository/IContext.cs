using Acme.Domain.Base.Entity;
using System.Linq;

namespace Acme.Domain.Base.Repository
{
    public interface IContext
    {
        IQueryable<TAggregateRoot> GetContext<TAggregateRoot>() where TAggregateRoot : BaseEntity, IAggregateRoot;
    }
}