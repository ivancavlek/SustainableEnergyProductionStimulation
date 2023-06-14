namespace Acme.Domain.Base.CommandHandler;

/// <summary>
/// <see href="https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91">Command credits</see>
/// </summary>
public interface ICommandHandler<TCommand>
{
    void Handle(TCommand command);
}