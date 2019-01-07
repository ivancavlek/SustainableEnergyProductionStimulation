using Acme.Domain.Base.Factory;
using Acme.Domain.Base.Repository;
using Acme.Seps.Text;
using System;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public abstract class BaseCommandHandler
    {
        protected readonly IRepository _repository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IIdentityFactory<Guid> _identityFactory;

        public event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing = delegate { };

        protected BaseCommandHandler(
            IRepository repository, IUnitOfWork unitOfWork, IIdentityFactory<Guid> identityFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        protected void Commit() =>
            _unitOfWork.Commit();

        protected void Log(EntityExecutionLoggingEventArgs useCaseExecutionProcessingLog) =>
            UseCaseExecutionProcessing(this, useCaseExecutionProcessingLog);

        protected void LogSuccessfulCommit() =>
            Log(new EntityExecutionLoggingEventArgs(SepsMessage.SuccessfulSave()));
    }
}