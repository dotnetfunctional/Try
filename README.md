# DotNetFunctional.Try

[![Build status](https://ci.appveyor.com/api/projects/status/sqaenp7jnbg5v029/branch/master?svg=true)](https://ci.appveyor.com/project/jotatoledo/try/branch/master)
[![NuGet](http://img.shields.io/nuget/v/DotNetFunctional.Try.svg?logo=nuget)](https://www.nuget.org/packages/DotNetFunctional.Try/)

The Try monad (Error/Exceptional monad) for C# with LINQ support and rich fluent syntax.

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

### Wrapping

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

### Unwrapping

There are a couple of options available to extract the value or exception wrapped in a `Try<T>` object.
One of them is `Try<T>.Value`, which raises the exception if no value but an exception is being wrapped:

```csharp
Try<string> tryString = ....;
// Throws if tryString wrapps an exception
string value = tryString.Value;
```

The `IsException` and `IsValue` properties can be used to determine if its safe to access the `Value` property:

```csharp
Try<string> tryString = ....;
if(tryString.IsValue)
{
    Console.WriteLine($"Wrapped value is: {tryString.Value}");
}

Try<string> tryString = ....;
if(tryString.IsException)
{
    Console.WriteLine($"Wrapped exception is: {tryString.Exception}");
}
```

Another option is to use deconstruction to get both the exception and value simultaneously:

```csharp
Try<string> tryString = ....;
// If a value is wrapped, exception is null
var (exception, value) = t1;
```

Finally, a more "functional" way would be using the `Try<T>.Match` method, which always run one of
two delegates:

```csharp
Try<int> t1 = ...;
string result = t1.Match(
    value => value,
    exception => exception.Message
);
```

## Recovering

`Try<T>.RecoverWith` allows you to map an exception wrapper into a new one. Futhermore, it alloes
you to access the wrapped exception. This can be useful in cases when you want to map an exception into
another one before moving along the pipeline. For example:

```csharp
Try<string> wrapper = Try.Create(()=> MayThrowArgumentNullException(null));
Try<string> invalid = wrapper.RecoverWith(ex => Try.LiftException<string>(new ArgumentException("Invalid", ex))):
```

## Side-Effects

To perform side effects, `Try<T>` exposes a `Tap<T>(succesFn: Action<T>, failureFn: Action<Exception>)` method. It
takes 2 delegates, invokes the according one depending on if the wrapper is a failure or success and returns the original wrapper.

````csharp
strig value = ...
Try<string> wrapper = Try.Create(()=> MayThrowArgumentNullException(value))
                         .Tap(val => Console.WriteLine(value), ex => Console.WriteLine(ex.Message));
```

## Other projects

Check out some of my other C# projects:

- [DotNetFunctional.Maybe](https://github.com/dotnetfunctional/Maybe): An Option type monad for C# with LINQ support and rich fluent syntax.