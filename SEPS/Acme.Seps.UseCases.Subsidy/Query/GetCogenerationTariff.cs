using Acme.Domain.Base.Query;
using Acme.Domain.Base.QueryHandler;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Acme.Seps.UseCases.Subsidy.Query
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
                .AppendLine("trf.Since,")
                .AppendLine("trf.Until,")
                .AppendLine("trf.LowerProductionLimit,")
                .AppendLine("trf.UpperProductionLimit,")
                .AppendLine("trf.LowerRate,")
                .AppendLine("trf.HigherRate,")
                .AppendLine("NaturalGasSellingPriceAmount = nsp.Amount,")
                .AppendLine("AverageElectricEnergyProductionPriceAmount = aep.Amount,")
                .AppendLine("pte.ConsumesFuel")
                .AppendLine("FROM parameter.Tariffs AS trf")
                .AppendLine("INNER JOIN parameter.ProjectType AS pte")
                .AppendLine("ON trf.ProjectTypeId = pte.Id")
                .AppendLine("INNER JOIN parameter.EconometricIndexes AS nsp")
                .AppendLine("ON trf.NaturalGasSellingPriceId = nsp.Id")
                .AppendLine("INNER JOIN parameter.EconometricIndexes AS aep")
                .AppendLine("ON trf.AverageElectricEnergyProductionPriceId = aep.Id")
                .AppendLine("WHERE trf.TariffType = 'CogenerationTariff'")
                .AppendLine("ORDER BY trf.Since DESC, pte.Code, trf.LowerProductionLimit")
                .ToString()).AsList();
    }

    public class GetCogenerationTariffQuery : IQuery<IReadOnlyList<CogenerationTariffQueryResult>>
    {
    }

    public class CogenerationTariffQueryResult
    {
        public string ContractLabel { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTimeOffset Since { get; set; }
        public DateTimeOffset? Until { get; set; }
        public int? LowerProductionLimit { get; set; }
        public int? UpperProductionLimit { get; set; }
        public decimal LowerRate { get; set; }
        public decimal HigherRate { get; set; }
        public decimal NaturalGasSellingPriceAmount { get; set; }
        public decimal AverageElectricEnergyProductionPriceAmount { get; set; }
        public decimal MonthlyAverageElectricEnergyProductionPriceAmount { get; set; }
        public bool ConsumesFuel { get; set; }
    }
}