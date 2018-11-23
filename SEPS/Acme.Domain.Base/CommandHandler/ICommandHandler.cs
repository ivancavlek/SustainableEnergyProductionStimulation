using Acme.Domain.Base.Command;

namespace Acme.Domain.Base.CommandHandler
{
    public interface ICommandHandler<TCommand> where TCommand : IBaseCommand
    {
        void Handle(TCommand command);
    }
}