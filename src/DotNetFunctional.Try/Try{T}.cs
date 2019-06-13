// <copyright file="Try{T}.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.ExceptionServices;

    /// <summary>
    /// A wrapper for either an exception or a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public readonly struct Try<T> : IEquatable<Try<T>>
    {
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Try{T}"/> struct.
        /// </summary>
        /// <param name="value">The value to wrapp.</param>
        /// <param name="exception">The exception to wrapp.</param>
        /// <param name="isException">A flag value; true if this will wrapp an exception; false otherwise.</param>
        internal Try(T value, Exception exception, bool isException)
        {
            this.Exception = isException ? exception ?? throw new ArgumentNullException(nameof(exception)) : default;
            this.value = value;
        }

        /// <summary>
        /// Gets a value indicating whether this wrapps an exception or not.
        /// </summary>
        public bool IsException => this.Exception != null;

        /// <summary>
        /// Gets a value indicating whether this wrapps a value or not.
        /// </summary>
        public bool IsValue => !this.IsException;

        /// <summary>
        /// Gets the wrapped exception, if any. Returns <c>null</c> if this instance is not an exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the wrapped value. If this instance is an exception, then that exception is (re)thrown.
        /// </summary>
        public T Value => this.IsException ? throw this.Rethrow() : this.value;

        /// <summary>
        /// Implementation of the equality operator.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if left and right pass the equality check; false otherwise.</returns>
        public static bool operator ==(Try<T> left, Try<T> right) => left.Equals(right);

        /// <summary>
        /// Implementation of the inequality operator.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if left and right arent equal; false otherwise.</returns>
        public static bool operator !=(Try<T> left, Try<T> right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString() => this.IsException ? $"Exception<{this.Exception}>" : $"Value<{this.value}>";

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Try<T> @try && this.Equals(@try);

        /// <inheritdoc/>
        public override int GetHashCode() => this.IsException ? this.Exception.GetHashCode() : EqualityComparer<T>.Default.GetHashCode(this.value);

        /// <inheritdoc/>
        public bool Equals(Try<T> other) => this.IsException.Equals(other.IsException)
            && this.IsException
            ? ReferenceEquals(this.Exception, other.Exception)
            : EqualityComparer<T>.Default.Equals(this.value, other.value);

        /// <summary>
        /// Gets the wrapped exception casted as <typeparamref name="TException"/>. If no exception is wrapped, null is returned.
        /// </summary>
        /// <typeparam name="TException">The type to cast the wrapped exception into.</typeparam>
        /// <returns>The wrapped exception casted into <typeparamref name="T"/>.</returns>
        public TException ExceptionAs<TException>()
            where TException : Exception => this.Exception as TException;

        /// <summary>
        /// Invokes a mapping function depending on wheter a value or exception is wrapped.
        /// </summary>
        /// <typeparam name="TResult">The type of the mapping result.</typeparam>
        /// <param name="whenValue">The action to execute if this instance is a value.</param>
        /// <param name="whenException">The action to execute if this instance is an exception.</param>
        /// <returns>The projection result.</returns>
        public TResult Match<TResult>(Func<T, TResult> whenValue, Func<Exception, TResult> whenException) => this.IsException ? whenException(this.Exception) : whenValue(this.value);

        /// <summary>
        /// Maps a wrapped value to another mapped value.
        /// If this instance is an exception, <paramref name="map"/> is not invoked, and this method returns a wrapper for that exception.
        /// If <paramref name="map"/> throws an exception, this method returns a wrapper for that exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the mapping.</typeparam>
        /// <param name="map">The mapping function. Exceptions from this method are captured and wrapped.</param>
        /// <returns>The wrapped mapped value.</returns>
        public Try<TResult> Map<TResult>(Func<T, TResult> map) => this.Bind(value => Try.Create(() => map(value)));

        /// <summary>
        /// Binds the wrapped value.
        /// If this instance is an exception, <paramref name="bind"/> is not invoked, and this method returns a wrapper for that exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the binding.</typeparam>
        /// <param name="bind">The binding function. Should not throw exceptions.</param>
        /// <returns>The binded wrapped value.</returns>
        public Try<TResult> Bind<TResult>(Func<T, Try<TResult>> bind) => this.IsException ? Try.LiftException<TResult>(this.Exception) : bind(this.value);

        private Exception Rethrow()
        {
            ExceptionDispatchInfo.Capture(this.Exception).Throw();
            return this.Exception;
        }
    }
}
