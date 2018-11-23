namespace Acme.Domain.Base.CommandHandler
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}