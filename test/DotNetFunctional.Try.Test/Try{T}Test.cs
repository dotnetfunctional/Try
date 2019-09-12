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
        public void ExceptionAs_Should_ReturnNull_When_ValueWrapped()
        {
            var tryString = Try.LiftValue(string.Empty);

            var result = tryString.ExceptionAs<InvalidOperationException>();

            result.Should()
                .BeNull();
        }

        [Fact]
        public void ExceptionAs_Should_ReturnNull_When_CastingTypeIncompatible()
        {
            var tryEx = Try.LiftException<int>(new InvalidOperationException("test"));

            var result = tryEx.ExceptionAs<ArgumentException>();

            result.Should()
                .BeNull();
        }

        [Fact]
        public void ExceptionAs_Should_ReturnCastedException_When_CastingTypeCompatible()
        {
            var tryEx = Try.LiftException<int>(new InvalidOperationException("test"));

            var result = tryEx.ExceptionAs<InvalidOperationException>();

            result.Should()
                .NotBeNull("the exception could be casted")
                .And
                .BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Match_Should_ProjectValue_When_ValueWrapped()
        {
            var tryString = Try.LiftValue("he");
            string MatchString(string val) => val + "llo";

            var result = tryString.Match(MatchString, ex => ex.Message);

            result.Should()
                .Be(MatchString(tryString.Value), "the value mapping function was invoked on the wrapped value.");
        }

        [Fact]
        public void Match_Should_ProjectException_When_ExceptionWrapped()
        {
            var tryEx = Try.LiftException<string>(new InvalidOperationException("te"));
            string MapException(Exception ex) => ex.Message + "st";

            var result = tryEx.Match(val => val, MapException);

            result.Should()
                .Be(MapException(tryEx.Exception), "the exception mapping function was invoked on the wrapped exception.");
        }

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
            bindResult.IsFailure.Should().BeTrue();
            bindResult.Exception.Should().Be(ex);
        }

        [Fact]
        public void DesconstructsExceptionWrapper()
        {
            (var sourceEx, var sut) = Utils.WrapException<string>(new ArgumentException("hello"));

            var (val, ex) = sut;

            val.Should().BeNull();
            ex.Should().Be(sourceEx);
        }

        [Fact]
        public void DesconstructsValueWrapper()
        {
            (var sourceVal, var sut) = Utils.Wrap("hello");

            var (val, ex) = sut;

            val.Should().Be(sourceVal);
            ex.Should().BeNull();
        }

        [Fact]
        public void RecoverWithReturnsSameValueWrapper()
        {
            (var val, var sut) = Utils.Wrap("hello");

            var result = sut.RecoverWith(ex => Try.LiftValue("nothing"));

            result.Should().Be(sut);
        }

        [Fact]
        public void RecoverWithMapsExceptionWrapperIntoValueWrapper()
        {
            (var sourceEx, var sut) = Utils.WrapException<string>(new ArgumentException("hello"));

            var result = sut.RecoverWith(ex => Try.LiftValue(ex.Message));

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(sourceEx.Message);
        }

        [Fact]
        public void RecoverWithMapsExceptionWrapperIntoNewExceptionWrapper()
        {
            (var sourceEx, var sut) = Utils.WrapException<string>(new ArgumentException("hello"));
            Exception RecoverFn(Exception ex)
            {
                return new InvalidOperationException("test", ex);
            }

            var result = sut.RecoverWith(ex => Try.LiftException<string>(RecoverFn(ex)));

            result.Should().NotBe(sut);
            result.IsFailure.Should().BeTrue();
            result.Exception.Should().BeEquivalentTo(RecoverFn(sourceEx));
        }

        [Fact]
        public void Tap_Should_RunFailureSideEffectAndNotRunSuccessSideEffect_When_OnFailure()
        {
            var initial = "initial";
            string result = initial;
            var (ex, sut) = Utils.WrapException<string>(new InvalidOperationException("test"));

            sut.Tap(v => result = v, e => result = e.Message);

            result.Should().Be(ex.Message);
        }

        [Fact]
        public void Tap_Should_RunSuccessSideEffectAndNotFailureSideEffect_When_OnSuccess()
        {
            var initial = "initial";
            string result = initial;
            var sut = Try.LiftValue("next");

            sut.Tap(val => result = val, e => result = e.Message);

            result.Should().Be(sut.Value);
        }
    }
}
