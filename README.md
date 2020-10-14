# AddFactory extension method
This project simulates the Ninject's [`ToFactory`](https://github.com/ninject/Ninject.Extensions.Factory/wiki/Factory-interface) functionality
but on `IServiceCollection`.

[![NuGet](http://img.shields.io/nuget/v/AddFactoryExtension.svg)](https://www.nuget.org/packages/AutoMapper/)

## Example

Types.cs
```csharp
public interface IBar 
{ 
    void DoSomething(); 
}
public class Bar : IBar
{ 
    public void DoSomething() 
        => Console.WriteLine("It works!!!");
}
// Let's add implementation of IBarFactory dynamically!
public interface IBarFactory
{ 
    IBar Factory(); 
}
```

Program.cs
```csharp
using Microsoft.Extensions.DependencyInjection;

public static class Program 
{
    static void Main() 
    {
        ServiceCollection sc = new ServiceCollection();
        sc.AddTransient<IBar, Bar>();

        // dynamic factory service lifetime 
        // is singleton by default and can be changed
        sc.AddFactory<IBarFactory>();

        var sp = sc.BuildServiceProvider();
        var barFactory = sp.GetRequiredService<IBarFactory>();
        
        IBar bar = barFactory.Factory();
        
        // outputs 'It works!!!!'
        bar.DoSomething();
    }
}
```
## How it works

Magic. (`System.Reflection.Emit`)

## Assembly scan for implemented interfaces

The `AddFactory` method scans for implementations of the 
`Factory` method return types inside the assembly of 
the factory interface type.

E.g. from the example above, the `AddFactory` will look
for implementations of `IBar` inside the assembly with
`IBarFactory`.

It is possible to change that behvaiour and specify 
which assemblies to scan.

```csharp
sc.AddFactory<IBarFactory>(params Assembly[] assemblies)
```



## Automatic injection of other services

`AddFactory` supports automatic injection of other services
inside the dynamically created factory and then feeding those
services to the newly factored services.

Note that the dependent services must come **after** any 
simple types (ints/longs/other num types, enums, structs, strings).

Example:

```csharp
publi
public interface ISomeDependency { }
public interface IBar1 { }

public class Bar1 : IBar1 
{ 
    private int someParameter;
    private ISomeDependency someDependency;

    // ISomeDependency will be automatically fed by
    // the dynamic factory
    public Bar1(int someParameter, ISomeDependency someDependency) 
    {
        this.someParameter = someParameter;
        this.someDependency = someDependency;
    }
    public void DoSomething() 
    {
        Console.WriteLine("It works!!!");
    }
}

public interface IBar1Factory 
{ 
    // dynamic factory will inject ISomeDependency
    // and then invoke the ctor with int
    IBar1 Factory(int someParameter); 
}
```

## Limitations

- The project searches only for methods named `Factory` inside the factory interface.
- The return type of the `Factory` method must be an interface
- `AddFactory` will throw an exception if there are multiple constructors matching the factory method signature.
