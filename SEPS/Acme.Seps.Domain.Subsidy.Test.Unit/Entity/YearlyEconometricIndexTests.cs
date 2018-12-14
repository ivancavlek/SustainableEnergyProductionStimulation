﻿using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Utility;
using Acme.Seps.Domain.Subsidy.Entity;
using Acme.Seps.Domain.Subsidy.Infrastructure;
using FluentAssertions;
using NSubstitute;
using System;

namespace Acme.Seps.Domain.Subsidy.Test.Unit.Entity
{
    public class YearlyEconometricIndexTests
    {
        private readonly decimal _amount;
        private readonly string _remark;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public YearlyEconometricIndexTests()
        {
            _amount = 1M;
            _remark = nameof(_remark);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void DateCannotBeBeforeInitialDate()
        {
            var yearBeforeInitialDate = SepsVersion.InitialDate().AddYears(-1);

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _remark, yearBeforeInitialDate, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.YearlyParameterException);
        }

        public void DateCannotBeFromCurrentYear()
        {
            var currentYear = DateTime.Now.Date;

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _remark, currentYear, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.YearlyParameterException);
        }

        public void DateIsCorrectlySet()
        {
            DateTimeOffset yearBeforeCurrentYear = DateTime.Now.Date.AddYears(-1);

            var econometricIndex = new DummyYearlyEconometricIndex(
                _amount, _remark, yearBeforeCurrentYear, _identityFactory);

            econometricIndex.Period.ActiveFrom.Should().Be(yearBeforeCurrentYear.ToFirstMonthOfTheYear());
        }

        private class DummyYearlyEconometricIndex : YearlyEconometricIndex<DummyYearlyEconometricIndex>
        {
            protected override int DecimalPlaces => 2;

            public DummyYearlyEconometricIndex(
                decimal amount, string remark, DateTimeOffset activeFrom, IIdentityFactory<Guid> identityFactory)
                : base(amount, remark, activeFrom, identityFactory) { }
        }
    }
}