// <copyright file="Try.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try
{
    using System;

    /// <summary>
    /// Companion class for <see cref="Try{T}"/> that provides static factory methods.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Invokes the given function and wrapps either the result or the exception.
        /// </summary>
        /// <typeparam name="T">The type of wraped value.</typeparam>
        /// <param name="createFn">The function to be run.</param>
        /// <returns>Either a wrapped value or exception.</returns>
        public static Try<T> Create<T>(Func<T> createFn)
        {
            try
            {
                return LiftValue(createFn());
            }
            catch (Exception ex)
            {
                return LiftException<T>(ex);
            }
        }

        /// <summary>
        /// Lifts a value.
        /// </summary>
        /// <typeparam name="T">The type of wraped value.</typeparam>
        /// <param name="value">The value to lift.</param>
        /// <returns>The wrapped value.</returns>
        public static Try<T> LiftValue<T>(T value) => new Try<T>(value, default, false);

        /// <summary>
        /// Lifts an exception.
        /// </summary>
        /// <typeparam name="T">The type of wrapped value.</typeparam>
        /// <param name="ex">The exception to lift.</param>
        /// <returns>The wrapped exception.</returns>
        public static Try<T> LiftException<T>(Exception ex) => new Try<T>(default, ex, true);
    }
}
