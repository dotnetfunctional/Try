// <copyright file="TryLinqExtensions.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try.Test
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class TryLinqExtensions
    {
        [Fact]
        public void Select_Should_ReWrappException_When_InvokedOnWrappedException()
        {
            var ex = new ArgumentNullException("test");
            var tryString = Try.LiftException<string>(ex);
            var tryInt = Try.LiftException<int>(ex);

            var stringResult = tryString.Select(val => $"Source val: {val} ");
            var intResult = tryInt.Select(val => val + 10);

            stringResult.Should()
                .Be(tryString, "the projection wont be applied on a wrapped exception.");
            intResult.Should()
                .Be(tryInt, "the projection wont be applied on a wrapped exception.");
        }

        [Fact]
        public void Select_Should_WrappProjectedValue_When_InvokedOnWrappedValue()
        {
            var tryString = Try.LiftValue("hello");
            var tryInt = Try.LiftValue(0);
            string SelectString(string val) => $"Source val: {val} ";
            int SelectInt(int val) => val + 10;

            var stringResult = tryString.Select(SelectString);
            var intResult = tryInt.Select(SelectInt);

            stringResult.Should()
                .Be(tryString.Map(SelectString), "the value was projected.");
            intResult.Should()
                .Be(tryInt.Map(SelectInt), "the value was projected.");
        }

        [Fact]
        public void Where_Should_YieldSource_When_PredicateFullfiled()
        {
            var tryString = Try.LiftValue("hello");
            var tryInt = Try.LiftValue(20);

            var stringResult = tryString.Where(val => val.StartsWith("h"));
            var intResult = tryInt.Where(val => val > 10);

            stringResult.Should()
                .Be(tryString, "the source wrapper is yielded");
            intResult.Should()
                .Be(tryInt, "the source wrapper is yielded");
        }

        [Fact]
        public void Where_Should_YieldWrappedException_When_PredicateUnmeet()
        {
            var tryString = Try.LiftValue("hello");
            var tryInt = Try.LiftValue(20);

            var stringResult = tryString.Where(val => val.StartsWith("x"));
            var intResult = tryInt.Where(val => val > 20);

            stringResult.IsException.Should()
                .BeTrue();
            stringResult.Exception.Should()
                .BeOfType<InvalidOperationException>("an exception is wrapped when the predicate is unmeet.");
            intResult.IsException.Should()
                .BeTrue();
            intResult.Exception.Should()
                .BeOfType<InvalidOperationException>("an exception is wrapped when the predicate is unmeet.");
        }

        [Fact]
        public void Where_Should_YieldReWrappedException_When_InvokedOnWrappedException()
        {
            var ex = new ArgumentNullException("test");
            var tryString = Try.LiftException<string>(ex);
            var tryInt = Try.LiftException<int>(ex);

            var stringResult = tryString.Where(val => val.StartsWith("h"));
            var intResult = tryInt.Where(val => val > 10);

            stringResult.Should()
                 .Be(tryString, "the predicate wont be applied on an exception.");
            intResult.Should()
                .Be(tryInt, "the predicate wont be applied on an exception.");
        }

        [Fact]
        public void QueryExpression_Should_CorrectlyYield_When_NoInvalidValuesAreAccessed()
        {
            var tryStringLeft = Try.LiftValue("he");
            var tryStringRight = Try.LiftValue("llo");
            var tryIntLeft = Try.LiftValue(10);
            var tryIntRight = Try.LiftValue(20);

            var stringResult = from left in tryStringLeft
                               from right in tryStringRight
                               select left + right;
            var intResult = from left in tryIntLeft
                            from right in tryIntRight
                            select left + right;

            stringResult.Should()
                .Be(Try.LiftValue(tryStringLeft.Value + tryStringRight.Value));
            intResult.Should()
                .Be(Try.LiftValue(tryIntLeft.Value + tryIntRight.Value));
        }

        [Fact]
        public void QueryExpression_Should_ShortCircuit_When_WrappedExceptionAccessed()
        {
            var ex = new InvalidOperationException("test");
            var tryStringLeft = Try.LiftValue("he");
            var tryStringRight = Try.LiftException<string>(ex);
            var tryIntLeft = Try.LiftValue(10);
            var tryIntRight = Try.LiftException<int>(ex);

            var stringResult = from left in tryStringLeft
                               from right in tryStringRight
                               select left + right;
            var intResult = from left in tryIntLeft
                            from right in tryIntRight
                            select left + right;

            stringResult.Should()
                .Be(Try.LiftException<string>(ex));
            intResult.Should()
                .Be(Try.LiftException<int>(ex));
        }

        [Fact]
        public void QueryExpression_Should_YieldNothing_When_WhereConditionUnmeet()
        {
            var tryString = Try.LiftValue("hello");
            var tryInt = Try.LiftValue(20);

            var stringResult = from stringVal in tryString
                               where stringVal.StartsWith("o")
                               select stringVal;
            var intResult = from intVal in tryInt
                            where intVal > 30
                            select intVal;

            stringResult.IsException.Should()
                .BeTrue();
            stringResult.Exception.Should()
                .BeOfType<InvalidOperationException>();
            intResult.IsException.Should()
                .BeTrue();
            intResult.Exception.Should()
                .BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void QueryExpression_Should_CorrectlyYield_When_WhereConditionMeet()
        {
            var tryString = Try.LiftValue("hello");
            var tryInt = Try.LiftValue(20);

            var stringResult = from stringVal in tryString
                               where stringVal.StartsWith(tryString.Value)
                               select stringVal;
            var intResult = from intVal in tryInt
                            where intVal >= tryInt.Value
                            select intVal;

            stringResult.Should()
                .Be(tryString, "the new wrapper has the same value.");
            tryInt.Should()
                .Be(tryInt, "the new wrapper has the same value.");
        }
    }
}
