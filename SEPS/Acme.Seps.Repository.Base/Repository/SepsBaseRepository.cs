using Acme.Domain.Base.Repository;
using System;

namespace Acme.Seps.Repository.Base.Repository
{
    public abstract class SepsBaseRepository
    {
        public readonly IContext Context;

        protected SepsBaseRepository(IContext context) =>
            Context = context ?? throw new ArgumentNullException(nameof(context));
    }
}