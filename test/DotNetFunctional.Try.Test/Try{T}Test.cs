// <copyright file="Try{T}Test.cs" company="DotNetFunctional">
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
        public void Map_Should_ProjectValue_When_ValueWrapped()
        {
            var tryString = Try.LiftValue(string.Empty);
            var tryInt = Try.LiftValue<int>(default);
            int MapInt(int val) => val + 1;
            string MapString(string val) => val + "bla";

            var intMap = tryInt.Map(MapInt);
            var stringMap = tryString.Map(MapString);

            intMap.Should().Be(Try.LiftValue(MapInt(tryInt.Value)));
            stringMap.Should().Be(Try.LiftValue(MapString(tryString.Value)));
        }

        [Fact]
        public void Map_Should_ReWrappException_When_ExceptionWrapped()
        {
            (var ex, var tryEx) = Utils.WrapException<int>(new ArgumentException("hello"));
            int MapInt(int val) => val + 1;

            var bindResult = tryEx.Map(MapInt);

            bindResult.Should().Be(tryEx);
            bindResult.IsException.Should().BeTrue();
            bindResult.Exception.Should().Be(ex);
        }

        [Fact]
        public void Bind_Should_ReWrappException_When_ExceptionWrapped()
        {
            (var ex, var tryEx) = Utils.WrapException<int>(new ArgumentException("hello"));
            Try<int> MapInt(int val) => Try.LiftValue(val + 1);

            var bindResult = tryEx.Bind(MapInt);

            bindResult.Should().Be(tryEx);
            bindResult.IsException.Should().BeTrue();
            bindResult.Exception.Should().Be(ex);
        }

        [Fact]
        public void Bind_Should_ProjectValue_When_ValueWrapped()
        {
            var tryString = Try.LiftValue(string.Empty);
            var tryInt = Try.LiftValue<int>(default);
            Try<int> MapInt(int val) => Try.LiftValue(val + 1);
            Try<string> MapString(string val) => Try.LiftValue(val + "bla");

            var intBind = tryInt.Bind(MapInt);
            var stringBind = tryString.Bind(MapString);

            intBind.Should().Be(MapInt(tryInt.Value));
            stringBind.Should().Be(MapString(tryString.Value));
        }
    }
}
