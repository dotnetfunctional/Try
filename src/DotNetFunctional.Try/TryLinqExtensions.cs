// <copyright file="TryLinqExtensions.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try
{
    using System;

    /// <summary>
    /// Provides extension methods for <see cref="Try{T}"/> that enable LINQ support.
    /// </summary>
    public static class TryLinqExtensions
    {
        /// <summary>
        /// Projects the wrapped value into a new one and wraps it.
        /// </summary>
        /// <typeparam name="TSource">The type of the wrapped value.</typeparam>
        /// <typeparam name="TResult">The type to project the value into.</typeparam>
        /// <param name="source">The wrapper.</param>
        /// <param name="project">A transform function to apply on the wrapped value.</param>
        /// <returns>The projection of the wrapped value in a wrapper.</returns>
        public static Try<TResult> Select<TSource, TResult>(this Try<TSource> source, Func<TSource, TResult> project)
            => source.Bind(val => Try.LiftValue(project(val)));

        /// <summary>
        /// Projects the wrapped value into a new wrapper.
        /// </summary>
        /// <typeparam name="TSource">The type of the wrapped value.</typeparam>
        /// <typeparam name="TResult">The type to project the value into.</typeparam>
        /// <param name="source">The wrapper.</param>
        /// <param name="project">A transform function to apply on the wrapped value.</param>
        /// <returns>The projection of the wrapped value.</returns>
        public static Try<TResult> SelectMany<TSource, TResult>(this Try<TSource> source, Func<TSource, Try<TResult>> project)
            => source.Bind(val => project(val));

        /// <summary>
        /// Projects the wrapped value into a new wrapper and invokes a result selector function on it.
        /// </summary>
        /// <typeparam name="TSource">The type of the wrapped value.</typeparam>
        /// <typeparam name="TIntermediate">The type to project the value into.</typeparam>
        /// <typeparam name="TResult">The type of the selected value.</typeparam>
        /// <param name="source">The wrapper.</param>
        /// <param name="project">A transform function to apply on the wrapped value.</param>
        /// <param name="select">A result selector function to apply on the projected value.</param>
        /// <returns>The selected value wrapped.</returns>
        public static Try<TResult> SelectMany<TSource, TIntermediate, TResult>(
            this Try<TSource> source,
            Func<TSource, Try<TIntermediate>> project,
            Func<TSource, TIntermediate, TResult> select) => source.Bind(sourceVal => project(sourceVal).Select(interVal => select(sourceVal, interVal)));

        /// <summary>
        /// Evaluates a condition on the wrapped value.
        /// </summary>
        /// <typeparam name="TSource">The type of the wrapped value.</typeparam>
        /// <param name="source">The wrapper.</param>
        /// <param name="predicate">A function to test the wrapped value for a condition.</param>
        /// <returns>
        /// <paramref name="source"/> if <paramref name="predicate"/> yields true when evaluated on the wrapped value; a wrapped <see cref="InvalidOperationException"/> otherwise.
        /// </returns>
        public static Try<TSource> Where<TSource>(this Try<TSource> source, Func<TSource, bool> predicate)
            => source.Bind(val => predicate(val) ? source : Try.LiftException<TSource>(new InvalidOperationException("Predicate not satisfied")));
    }
}
