using Acme.Domain.Base.Command;

namespace Acme.Domain.Base.CommandHandler
{
    /// <summary>
    /// <see href="https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91">CQRS decorator</see>
    /// </summary>
    public abstract class CommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : IBaseCommand
    {
        private readonly ICommandHandler<TCommand> _decorated;

        protected CommandHandlerDecorator(ICommandHandler<TCommand> decorated) =>
            _decorated = decorated;

        public abstract void Handle(TCommand command);
    }
}