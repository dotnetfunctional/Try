# DotNetFunctional.Try

[![Build status](https://ci.appveyor.com/api/projects/status/sqaenp7jnbg5v029/branch/master?svg=true)](https://ci.appveyor.com/project/jotatoledo/try/branch/master)
[![NuGet](http://img.shields.io/nuget/v/DotNetFunctional.Try.svg?logo=nuget)](https://www.nuget.org/packages/DotNetFunctional.Try/)

The Try monad (Error/Exceptional monad) for C#

## Installation

`DotNetFunctional.Try` can be installed using the NuGet command line or the NuGet Package Manager in Visual Studio.

```bash
PM> Install-Package DotNetFunctional.Try
```

## Example

First, you will need to add the following using statement:

```csharp
using DotNetFunctional.Try;
```

### Lift

The standard way to wrap a value or exception is by using the `Try.Create` static method. It takes a delegate, which is **executed
immediately**. If the delegate throws an exception, the resulting `Try<T>` is a wrapper for that exception; if the delegate
successfully returns a value, the resulting `Try<T>` is a wrapper for that value.

```csharp
Try<int> tryInt = Try.Create(() => 10);
Try<string> tryString = Try.Create(() => "hello");
Try<int> tryException = Try.Create<int>(() => throw new InvalidArgumentException("Invalid int"));
Try<string> mayBeException = Try.Create(() =>
{
   MayThrow();
   return "success";
});
```

Both reference type and value type objects can be directly lifted by using the `Try.LiftValue` static method:

```csharp
Try<int> tryInt = Try.LiftValue(10);
Try<string> tryString = Try.LiftValue("hello");
```

Exceptions can be directly lifted as well by using the generic `Try.LiftException<T>` static method:

```csharp
Try<int> invalidInt = Try.LiftException<int>(new InvalidArgumentException("Invalid int"));
```

### Query expressions

`Try<T>` exposes `Select`, `SelectMany` and `Where` methods, which allow LINQ query syntax:

```csharp
Try<int> t1 = ...;
Try<int> t2 = ...;
Try<int> addition = from v1 in t1
                    from v2 in t2
                    select v1 + v2;
```

If any of the accessed `Try<T>` objects in the expression is an exception wrapper, the expression will short-circuit and
the resulting `Try<T>` will wrapp the first encountered wrapped exception:

```csharp
Try<int> t1 = Try.FromException<int>(new InvalidArgumentException());
Try<int> t2 = ...;
Try<int> addition = from v1 in t1
                    from v2 in t2
                    select v1 + v2;

if(addition.IsException)
{
    Console.Write($"Exception is: {addition.Exception}");
}
```