using Acme.Domain.Base.CommandHandler;
using System;

namespace Acme.Seps.Domain.Base.CommandHandler;

public interface ISepsCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ISepsCommand
{
    event EventHandler<EntityExecutionLoggingEventArgs> UseCaseExecutionProcessing;
}