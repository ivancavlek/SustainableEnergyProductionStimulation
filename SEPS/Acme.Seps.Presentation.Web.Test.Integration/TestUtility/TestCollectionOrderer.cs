﻿using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Acme.Seps.Presentation.Web.Test.Integration.TestUtility;

public class TestCollectionOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var sortedMethods = new SortedDictionary<int, TTestCase>();

        foreach (var testCase in testCases)
        {
            var attribute = testCase.TestMethod.Method
                .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName)
                .FirstOrDefault();

            var priority = attribute.GetNamedArgument<int>("Priority");
            sortedMethods.Add(priority, testCase);
        }

        return sortedMethods.Values;
    }
}