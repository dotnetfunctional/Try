// <copyright file="TryTest.cs" company="DotNetFunctional">
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

    public partial class TryTest
    {
        [Fact]
        public void LiftsException()
        {
            (var ex, var result) = Utils.WrapException<int>(new ArgumentException("Invalid int"));
            Func<int> access = () => result.Value;

            result.IsFailure.Should().BeTrue("an exception was wrapped.");
            result.Exception.Should().Be(ex);
            access.Should().Throw<ArgumentException>("there is no value.");
        }

        [Fact]
        public void LiftExceptionThrowsWhenGivenNull()
        {
            Action nullException = () => Try.LiftException<int>(null);

            nullException.Should().Throw<ArgumentNullException>("null was given as exception");
        }

        [Fact]
        public void LiftsValue()
        {
            (var rawInt, var tryInt) = Utils.Wrap(10);
            (var rawString, var tryStr) = Utils.Wrap("hello");

            tryInt.IsFailure.Should().BeFalse("a value was wrapped");
            tryInt.Value.Should().Be(rawInt);
            tryInt.Exception.Should().BeNull("no exception present");
            tryStr.IsFailure.Should().BeFalse("a value was wrapped");
            tryStr.Value.Should().Be(rawString);
            tryStr.Exception.Should().BeNull("no exception present");
        }

        [Fact]
        public void LiftsValueHandlesNullValues()
        {
            (var rawInt, var tryInt) = Utils.Wrap(default(int?));
            (var rawString, var tryStr) = Utils.Wrap(default(string));

            tryInt.IsFailure.Should().BeFalse("a value was wrapped");
            tryInt.Value.Should().Be(rawInt);
            tryInt.Exception.Should().BeNull("no exception present");
            tryStr.IsFailure.Should().BeFalse("a value was wrapped");
            tryStr.Value.Should().Be(rawString);
            tryStr.Exception.Should().BeNull("no exception present");
        }

        [Fact]
        public void CreateWrapsException()
        {
            var testEx = new InvalidOperationException("throw");
            int YieldInt() => throw testEx;

            var result = Try.Create(YieldInt);

            result.IsFailure.Should().BeTrue("the creation fn. throwed an exception.");
            result.Exception.Should().Be(testEx);
        }

        [Fact]
        public void CreateWrapsValue()
        {
            var testVal = default(int);
            int YieldInt() => testVal;

            var result = Try.Create(YieldInt);

            result.IsFailure.Should().BeFalse("the creation fn. throwed no exception.");
            result.Value.Should().Be(testVal);
            result.Exception.Should().BeNull("no exception present");
        }
    }
}
