﻿using Acme.Domain.Base.Entity;
using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.ValueType;
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
        private readonly int _decimalPlaces;
        private readonly string _remark;
        private readonly IIdentityFactory<Guid> _identityFactory;

        public YearlyEconometricIndexTests()
        {
            _amount = 1M;
            _decimalPlaces = 2;
            _remark = nameof(_remark);
            _identityFactory = Substitute.For<IIdentityFactory<Guid>>();
        }

        public void PeriodCannotStartBeforeInitialPeriod()
        {
            var initialPeriodMinusYear = new DateTime(2007, 07, 01).AddYears(-1);
            var period = new Period(new YearlyPeriodFactory(initialPeriodMinusYear.AddYears(-1), initialPeriodMinusYear));

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.YearlyParameterException);
        }

        public void PeriodMustStartBeforeCurrentYear()
        {
            var currentDate = DateTime.UtcNow;
            var period = new Period(new YearlyPeriodFactory(currentDate.AddYears(-1), DateTime.UtcNow));

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory);

            action
                .Should()
                .ThrowExactly<DomainException>()
                .WithMessage(SubsidyMessages.YearlyParameterException);
        }

        public void PeriodIsCorrectlySet()
        {
            var correctDate = DateTime.UtcNow.AddYears(-2);
            var period = new Period(new YearlyPeriodFactory(correctDate.AddYears(-1), correctDate));

            Action action = () => new DummyYearlyEconometricIndex(
                _amount, _decimalPlaces, _remark, period, _identityFactory);

            action
                .Should()
                .NotThrow<Exception>();
        }
    }

    internal class DummyYearlyEconometricIndex : YearlyEconometricIndex<DummyYearlyEconometricIndex>
    {
        public DummyYearlyEconometricIndex(
            decimal amount,
            int decimalPlaces,
            string remark,
            Period period,
            IIdentityFactory<Guid> identityFactory)
            : base(amount, decimalPlaces, remark, period, identityFactory) { }

        public override DummyYearlyEconometricIndex CreateNew(
            decimal amount, string remark, IIdentityFactory<Guid> identityFactory) =>
            this;
    }
}