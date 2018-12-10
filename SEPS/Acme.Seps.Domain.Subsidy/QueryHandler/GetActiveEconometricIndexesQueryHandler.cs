using Acme.Domain.Base.QueryHandler;
using Acme.Seps.Domain.Subsidy.Query;
using Acme.Seps.Domain.Subsidy.QueryResult;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Acme.Seps.Domain.Subsidy.QueryHandler
{
    public class GetActiveEconometricIndexesQueryHandler
        : IQueryHandler<GetActiveEconometricIndexesQuery, IReadOnlyList<ActiveEconometricIndexesQueryResult>>
    {
        private readonly IDbConnection _connection;

        public GetActiveEconometricIndexesQueryHandler(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        IReadOnlyList<ActiveEconometricIndexesQueryResult>
            IQueryHandler<GetActiveEconometricIndexesQuery, IReadOnlyList<ActiveEconometricIndexesQueryResult>>
            .Handle(GetActiveEconometricIndexesQuery query) =>
            _connection
                .Query<ActiveEconometricIndexesQueryResult>(
                    "SELECT " +
                        "FORMAT(eix.ActiveFrom, 'd') AS ActiveFrom, " +
                        "FORMAT(eix.ActiveTill, 'd') AS ActiveTill, " +
                        "eix.Amount, " +
                        "eix.Remark " +
                    "FROM parameter.EconometricIndexes AS eix " +
                    "WHERE eix.EconometricIndexType = @Type",
                    new { Type = query.EconometricIndexType.Name }).AsList();
    }
}
