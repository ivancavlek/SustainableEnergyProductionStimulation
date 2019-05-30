using Acme.Domain.Base.Query;
using Acme.Domain.Base.QueryHandler;
using Acme.Seps.Domain.Base.CommandHandler;

namespace Acme.Seps.Presentation.Web.DependencyInjection
{
    public class CqrsMediator : ICqrsMediator
    {
        private readonly SepsSimpleInjectorContainer _container;

        public CqrsMediator(SepsSimpleInjectorContainer container) =>
            _container = container;

        void ICqrsMediator.Send<TCommand>(TCommand command)
        {
            var handler = _container.GetInstance<ISepsCommandHandler<TCommand>>();
            handler.UseCaseExecutionProcessing += Handler_UseCaseExecutionProcessing;
            handler.Handle(command);
        }

        TQueryResult ICqrsMediator.Handle<TQuery, TQueryResult>(TQuery query)
        {
            var handler = _container.GetInstance<IQueryHandler<TQuery, TQueryResult>>();
            return handler.Handle(query);
        }

        private void Handler_UseCaseExecutionProcessing(object sender, EntityExecutionLoggingEventArgs e) =>
            throw new System.NotImplementedException();
    }

    public interface ICqrsMediator
    {
        void Send<TCommand>(TCommand command) where TCommand : ISepsCommand;

        TQueryResult Handle<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult>;
    }
}