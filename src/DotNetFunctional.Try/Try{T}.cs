// <copyright file="Try{T}.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try
{
    using System;

    /// <summary>
    /// A wrapper for either an exception or a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public abstract class Try<T>
    {
        /// <summary>
        /// Gets a value indicating whether this wrapps an exception or not.
        /// </summary>
        public abstract bool IsFailure { get; }

        /// <summary>
        /// Gets a value indicating whether this wrapps a value or not.
        /// </summary>
        public abstract bool IsSuccess { get; }

        /// <summary>
        /// Gets the wrapped exception, if any. Returns <c>null</c> if this instance is not an exception.
        /// </summary>
        public abstract Exception Exception { get; }

        /// <summary>
        /// Gets the wrapped value. If this instance is an exception, then that exception is (re)thrown.
        /// </summary>
        public abstract T Value { get; }

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
        public abstract override string ToString();

        /// <inheritdoc/>
        public abstract override bool Equals(object obj);

        /// <inheritdoc/>
        public abstract override int GetHashCode();

        /// <summary>
        /// Deconstructs this wrapper into two variables.
        /// </summary>
        /// <param name="value">The wrapped value, or <c>default</c> if this instance is an exception.</param>
        /// <param name="exception">The wrapped exception, or <c>null</c> if this instance is a value.</param>
        public abstract void Deconstruct(out T value, out Exception exception);

        /// <summary>
        /// Gets the wrapped exception casted as <typeparamref name="TException"/>. If no exception is wrapped, null is returned.
        /// </summary>
        /// <typeparam name="TException">The type to cast the wrapped exception into.</typeparam>
        /// <returns>The wrapped exception casted into <typeparamref name="T"/>.</returns>
        public TException ExceptionAs<TException>()
            where TException : Exception => this.Exception as TException;

        /// <summary>
        /// Applies a given function to this in case of a failure. Similar to `map' for the exception.
        /// </summary>
        /// <param name="recoverFn">The mapping function. Exceptions from this method are captured and wrapped.</param>
        /// <returns>
        /// The wrapped result of invoking <paramref name="recoverFn"/> on <see cref="Exception"/> if this is a failure; this otherwise.
        /// </returns>
        public abstract Try<T> Recover(Func<Exception, T> recoverFn);

        /// <summary>
        /// Applies a given function to this in case of a failure. Similar to `flatMap` for the expection.
        /// </summary>
        /// <param name="recoverFn">The mapping function. Exceptions from this method are captured and wrapped.</param>
        /// <returns>
        /// The result of invoking <paramref name="recoverFn"/> on <see cref="Exception"/> if this is a failure; this otherwise.
        /// </returns>
        public abstract Try<T> RecoverWith(Func<Exception, Try<T>> recoverFn);

        /// <summary>
        /// Invokes a mapping function depending on wheter a value or exception is wrapped.
        /// </summary>
        /// <typeparam name="TResult">The type of the mapping result.</typeparam>
        /// <param name="whenValue">The action to execute if this instance is a value.</param>
        /// <param name="whenException">The action to execute if this instance is an exception.</param>
        /// <returns>The projection result.</returns>
        public abstract TResult Match<TResult>(Func<T, TResult> whenValue, Func<Exception, TResult> whenException);

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
        internal abstract Try<TResult> Bind<TResult>(Func<T, Try<TResult>> bind);
    }
}
