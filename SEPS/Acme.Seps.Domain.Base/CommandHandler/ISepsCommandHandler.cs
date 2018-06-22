using Acme.Domain.Base.Command;
using Acme.Domain.Base.CommandHandler;

namespace Acme.Seps.Domain.Base.CommandHandler
{
    public interface ISepsCommandHandler<TCommand>
        : ISepsLogService, ICommandHandler<TCommand> where TCommand : BaseCommand
    {
    }
}