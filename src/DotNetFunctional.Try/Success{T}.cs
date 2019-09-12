// <copyright file="Success{T}.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a value wrapper.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public sealed class Success<T> : Try<T>, IEquatable<Success<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Success{T}"/> class.
        /// </summary>
        /// <param name="value">The value to wrapp.</param>
        internal Success(T value)
        {
            this.Value = value;
        }

        /// <inheritdoc/>
        public override bool IsFailure => false;

        /// <inheritdoc/>
        public override bool IsSuccess => true;

        /// <inheritdoc/>
        public override Exception Exception => null;

        /// <inheritdoc/>
        public override T Value { get; }

        /// <inheritdoc/>
        public override void Deconstruct(out T value, out Exception exception)
        {
            value = this.Value;
            exception = default;
        }

        /// <inheritdoc/>
        public bool Equals(Success<T> other)
        {
            return other is Success<T>
                && EqualityComparer<T>.Default.Equals(this.Value, other.Value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Success<T> success
                && this.Equals(success);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => EqualityComparer<T>.Default.GetHashCode(this.Value);

        /// <inheritdoc/>
        public override TResult Match<TResult>(Func<T, TResult> whenValue, Func<Exception, TResult> whenException)
            => whenValue(this.Value);

        /// <inheritdoc/>
        public override Try<T> Recover(Func<Exception, T> recoverFn)
            => this;

        /// <inheritdoc/>
        public override Try<T> RecoverWith(Func<Exception, Try<T>> recoverFn)
            => this;

        /// <inheritdoc/>
        public override Try<T> Tap(Action<T> successFn = null, Action<Exception> failureFn = null)
        {
            successFn?.Invoke(this.Value);
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"Success<{this.Value}>";

        /// <inheritdoc/>
        internal override Try<TResult> Bind<TResult>(Func<T, Try<TResult>> bind)
            => bind(this.Value);
    }
}
