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
    public class GetEconometricIndexesQueryHandler
        : IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>>
    {
        private readonly IDbConnection _connection;

        public GetEconometricIndexesQueryHandler(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        IReadOnlyList<EconometricIndexQueryResult>
            IQueryHandler<GetEconometricIndexQuery, IReadOnlyList<EconometricIndexQueryResult>>
            .Handle(GetEconometricIndexQuery query) =>
            _connection
                .Query<EconometricIndexQueryResult>(new StringBuilder()
                    .AppendLine("SELECT ")
                    .AppendLine("eix.Since,")
                    .AppendLine("eix.Until,")
                    .AppendLine("eix.Amount,")
                    .AppendLine("eix.Remark")
                    .AppendLine("FROM parameter.EconometricIndexes AS eix")
                    .AppendLine("WHERE eix.EconometricIndexType = @Type")
                    .AppendLine("ORDER BY eix.Since DESC")
                    .ToString(),
                    new { Type = query.EconometricIndexType.Name }).AsList();
    }
}
