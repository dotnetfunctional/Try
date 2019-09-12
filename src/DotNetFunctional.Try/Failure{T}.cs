// <copyright file="Failure{T}.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try
{
    using System;
    using System.Runtime.ExceptionServices;

    /// <summary>
    /// Represents an exception wrapper.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public sealed class Failure<T> : Try<T>, IEquatable<Failure<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Failure{T}"/> class.
        /// </summary>
        /// <param name="exception">The exception to wrapp.</param>
        internal Failure(Exception exception)
        {
            this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        /// <inheritdoc/>
        public override bool IsFailure => true;

        /// <inheritdoc/>
        public override bool IsSuccess => false;

        /// <inheritdoc/>
        public override Exception Exception { get; }

        /// <inheritdoc/>
        public override T Value => throw this.Rethrow();

        /// <inheritdoc/>
        public override void Deconstruct(out T value, out Exception exception)
        {
            value = default;
            exception = this.Exception;
        }

        /// <inheritdoc/>
        public bool Equals(Failure<T> other)
        {
            return other is Failure<T>
                && ReferenceEquals(this.Exception, other.Exception);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Failure<T> failure
                && this.Equals(failure);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.Exception.GetHashCode();

        /// <inheritdoc/>
        public override TResult Match<TResult>(Func<T, TResult> whenValue, Func<Exception, TResult> whenException)
            => whenException(this.Exception);

        /// <inheritdoc/>
        public override Try<T> Recover(Func<Exception, T> recoverFn)
            => this.RecoverWith(ex => Try.LiftValue(recoverFn(ex)));

        /// <inheritdoc/>
        public override Try<T> RecoverWith(Func<Exception, Try<T>> recoverFn)
        {
            try
            {
                return recoverFn(this.Exception);
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }

        /// <inheritdoc/>
        public override Try<T> Tap(Action<T> successFn = null, Action<Exception> failureFn = null)
        {
            failureFn?.Invoke(this.Exception);
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"Failure<{this.Exception}>";

        /// <inheritdoc/>
        internal override Try<TResult> Bind<TResult>(Func<T, Try<TResult>> bind)
            => Try.LiftException<TResult>(this.Exception);

        private Exception Rethrow()
        {
            ExceptionDispatchInfo.Capture(this.Exception).Throw();
            return this.Exception;
        }
    }
}
