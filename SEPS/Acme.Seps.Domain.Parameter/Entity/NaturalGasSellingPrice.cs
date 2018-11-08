﻿using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
using System;

namespace Acme.Seps.Domain.Parameter.Entity
{
    public class NaturalGasSellingPrice : MonthlyEconometricIndex<NaturalGasSellingPrice>
    {
        protected NaturalGasSellingPrice() { }

        protected NaturalGasSellingPrice(
            decimal amount,
            string remark,
            MonthlyPeriod lastMonthlyPeriod,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, 2, remark, lastMonthlyPeriod, identityFactory) { }

        public override NaturalGasSellingPrice CreateNew(
            decimal amount, string remark, int month, int year, IIdentityFactory<Guid> identityFactory)
        {
            MonthlyPeriod = MonthlyPeriod.SetValidTill(new DateTime(year, month, 1));

            return new NaturalGasSellingPrice(amount, remark, MonthlyPeriod, identityFactory);
        }
    }
}