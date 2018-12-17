using Acme.Domain.Base.Query;
using Acme.Domain.Base.QueryHandler;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Acme.Seps.UseCases.Subsidy.Query
{
    public class GetRenewableEnergySourceTariffQueryHandler
        : IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>>
    {
        private readonly IDbConnection _connection;

        public GetRenewableEnergySourceTariffQueryHandler(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        IReadOnlyList<RenewableEnergySourceTariffQueryResult>
            IQueryHandler<GetRenewableEnergySourceTariffQuery, IReadOnlyList<RenewableEnergySourceTariffQueryResult>>
            .Handle(GetRenewableEnergySourceTariffQuery query) =>
            _connection.Query<RenewableEnergySourceTariffQueryResult>(new StringBuilder()
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
                .AppendLine("ConsumerPriceIndexAmount = eix.Amount,")
                .AppendLine("pte.ConsumesFuel")
                .AppendLine("FROM parameter.Tariffs AS trf")
                .AppendLine("INNER JOIN parameter.ProjectType AS pte")
                .AppendLine("ON trf.ProjectTypeId = pte.Id")
                .AppendLine("INNER JOIN parameter.EconometricIndexes AS eix")
                .AppendLine("ON trf.ConsumerPriceIndexId = eix.Id")
                .AppendLine("WHERE trf.TariffType = 'RenewableEnergySourceTariff'")
                .AppendLine("ORDER BY trf.Since DESC, pte.Code, trf.LowerProductionLimit")
                .ToString()).AsList();
    }

    public class GetRenewableEnergySourceTariffQuery : IQuery<IReadOnlyList<RenewableEnergySourceTariffQueryResult>>
    {
    }

    public class RenewableEnergySourceTariffQueryResult
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
        public decimal ConsumerPriceIndexAmount { get; set; }
        public bool ConsumesFuel { get; set; }
    }
}