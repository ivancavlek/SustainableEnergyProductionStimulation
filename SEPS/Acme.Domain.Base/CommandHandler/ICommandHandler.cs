using Acme.Domain.Base.Command;

namespace Acme.Domain.Base.CommandHandler
{
    public interface ICommandHandler<TCommand> where TCommand : BaseCommand
    {
        void Handle(TCommand command);
    }
}