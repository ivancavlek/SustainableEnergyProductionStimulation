using Acme.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Entity;
using Acme.Seps.Domain.Base.Factory;
using Acme.Seps.Domain.Base.Repository;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Seps.Domain.Base.Test.Unit.Repository
{
    public class CurrentActiveMonthlySpecificationTests
    {
        public void AggregateRootIsCorrectlyFiltered()
        {
            const string correct = "Correct";
            var identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            var monthlySpecification = new CurrentActiveMonthlySpecification<DummySepsBaseAggregate>();

            var aggregates = new List<DummySepsBaseAggregate>
            {
                new DummySepsBaseAggregate(
                    correct, new MonthlyPeriodFactory(DateTime.Now.Date.AddMonths(-2)), identityFactory),
                new DummySepsBaseAggregate(
                    "False",
                    new MonthlyPeriodFactory(DateTime.Now.Date.AddMonths(-3), DateTime.Now.Date.AddMonths(-2)),
                    identityFactory)
            };

            var filteredAggregates = aggregates.Where(monthlySpecification.ToExpression().Compile());

            filteredAggregates.Should().HaveCount(1).And.Contain(x => x.Status.Equals(correct));
        }

        public void AggregateRootIsCorrectlySatisfiedAsConfirmation()
        {
            var identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            var monthlySpecification = new CurrentActiveMonthlySpecification<DummySepsBaseAggregate>();

            var aggregate = new DummySepsBaseAggregate(
                "Correct", new MonthlyPeriodFactory(DateTime.Now.Date.AddMonths(-2)), identityFactory);

            monthlySpecification.IsSatisfiedBy(aggregate).Should().BeTrue();
        }

        public void AggregateRootIsCorrectlySatisfiedAsRejection()
        {
            var identityFactory = Substitute.For<IIdentityFactory<Guid>>();
            var monthlySpecification = new CurrentActiveMonthlySpecification<DummySepsBaseAggregate>();

            var aggregate = new DummySepsBaseAggregate(
                "False",
                new MonthlyPeriodFactory(DateTime.Now.Date.AddMonths(-3), DateTime.Now.Date.AddMonths(-2)),
                identityFactory);

            monthlySpecification.IsSatisfiedBy(aggregate).Should().BeFalse();
        }

        internal class DummySepsBaseAggregate : SepsBaseAggregate
        {
            public string Status { get; private set; }

            public DummySepsBaseAggregate(
                string status, IPeriodFactory periodFactory, IIdentityFactory<Guid> identityFactory)
                : base(periodFactory, identityFactory)
            {
                Status = status;
            }
        }
    }
}