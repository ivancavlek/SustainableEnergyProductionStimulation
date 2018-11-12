﻿using Acme.Domain.Base.Repository;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Parameter.Entity;
using System;
using System.Linq.Expressions;

namespace Acme.Seps.Domain.Parameter.Repository
{
    public sealed class NaturalGasSellingPricesInAYearSpecification : BaseSpecification<NaturalGasSellingPrice>
    {
        private readonly DateTimeOffset _previousYear;

        public NaturalGasSellingPricesInAYearSpecification()
        {
            _previousYear = SystemTime.CurrentYear().AddYears(-1);
        }

        public override Expression<Func<NaturalGasSellingPrice, bool>> ToExpression() =>
            nsp => nsp.Period.ValidFrom.Year.Equals(_previousYear.Year);
    }
}