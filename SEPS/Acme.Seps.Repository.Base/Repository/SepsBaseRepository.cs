using System;

namespace Acme.Seps.Repository.Base.Repository
{
    public abstract class SepsBaseRepository
    {
        public readonly IContext Context;

        protected SepsBaseRepository(IContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Context = context;
        }
    }
}