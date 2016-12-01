﻿using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class ConsumerPriceIndex : YearlyEconometricIndex
    {
        protected ConsumerPriceIndex()
        {
        }

        protected ConsumerPriceIndex(
            decimal amount,
            string remark,
            YearlyPeriod lastYearlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, 4, remark, lastYearlyPeriod, identityFactory) { }

        public override YearlyEconometricIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory) =>
            new ConsumerPriceIndex(amount, remark, (YearlyPeriod)Period, identityFactory);
    }
}