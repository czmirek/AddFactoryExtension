# AddFactory extension method
This project simulates the Ninject's [`ToFactory`](https://github.com/ninject/Ninject.Extensions.Factory/wiki/Factory-interface) functionality
but with `IServiceCollection`.

**Example code:**

Types.cs
```csharp
public interface IBar1 
{ 
    void DoSomething();
}

public class Bar1 : IBar1 
{ 
    public void DoSomething() 
    {
        Console.WriteLine("It works!!!");
    }
}

public interface IBar1Factory 
{ 
    IBar1 Factory(); 
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
        sc.AddFactory<IBarFactory>();

        var sp = sc.BuildServiceProvider();
        var barFactory = sp.GetRequiredService<IBarFactory>();
        
        IBar bar = barFactory.Factory();
        
        // outputs 'It works!!!!'
        bar.DoSomething();
    }
}
```