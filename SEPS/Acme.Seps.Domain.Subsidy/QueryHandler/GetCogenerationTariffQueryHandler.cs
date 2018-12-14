using Acme.Domain.Base.QueryHandler;
using Acme.Seps.Domain.Subsidy.Query;
using Acme.Seps.Domain.Subsidy.QueryResult;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Acme.Seps.Domain.Subsidy.QueryHandler
{
    public class GetCogenerationTariffQueryHandler
        : IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>>
    {
        private readonly IDbConnection _connection;

        public GetCogenerationTariffQueryHandler(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        IReadOnlyList<CogenerationTariffQueryResult>
            IQueryHandler<GetCogenerationTariffQuery, IReadOnlyList<CogenerationTariffQueryResult>>
            .Handle(GetCogenerationTariffQuery query) =>
            _connection.Query<CogenerationTariffQueryResult>(new StringBuilder()
                .AppendLine("SELECT")
                .AppendLine("pte.ContractLabel,")
                .AppendLine("pte.Name,")
                .AppendLine("pte.Code,")
                .AppendLine("trf.ActiveFrom,")
                .AppendLine("trf.ActiveTill,")
                .AppendLine("trf.LowerProductionLimit,")
                .AppendLine("trf.UpperProductionLimit,")
                .AppendLine("trf.LowerRate,")
                .AppendLine("trf.HigherRate,")
                .AppendLine("NaturalGasSellingPriceAmount = eix.Amount,")
                // ToDo: MonthlyAverageElectricEnergyProductionPrice
                .AppendLine("pte.ConsumesFuel")
                .AppendLine("FROM parameter.Tariffs AS trf")
                .AppendLine("INNER JOIN parameter.ProjectType AS pte")
                .AppendLine("ON trf.ProjectTypeId = pte.Id")
                .AppendLine("INNER JOIN parameter.EconometricIndexes AS eix")
                .AppendLine("ON trf.NaturalGasSellingPriceId = eix.Id")
                .AppendLine("WHERE trf.TariffType = 'CogenerationTariff'")
                .AppendLine("ORDER BY trf.ActiveFrom DESC, pte.Code, trf.LowerProductionLimit")
                .ToString()).AsList();
    }
}