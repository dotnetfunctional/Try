// <copyright file="Utils.cs" company="DotNetFunctional">
// Copyright (c) DotNetFunctional. All rights reserved.
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace DotNetFunctional.Try.Test
{
    using System;

    internal static class Utils
    {
        public static (T, Try<T>) Wrap<T>(T val = default) => (val, Try.LiftValue(val));

        public static (Exception, Try<T>) WrapException<T>(Exception ex) => (ex, Try.LiftException<T>(ex));
    }
}
