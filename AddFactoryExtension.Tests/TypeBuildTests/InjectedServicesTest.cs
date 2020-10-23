using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace AddFactoryExtension.Tests
{
    public class InjectedServicesTest
    {
        public interface ISomeOtherService { }
        public class SomeOtherService : ISomeOtherService { }
        public interface IBar { }
        public class Bar : IBar { public Bar(int num) { } }
        public class Bar2 : IBar { public Bar2(ISomeOtherService someOtherService) { } }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_Parameterless_Factory_Method_Match_With_Service_Ctor()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar, Bar>();
            sc.AddTransient<ISomeOtherService, SomeOtherService>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            Assert.NotNull(barFactory);

            var bar = barFactory.Factory();
            Assert.NotNull(bar);
            Assert.IsType<Bar2>(bar);
        }
    }

    public class InjectedServicesTest2
    {
        public interface ISomeOtherService { }
        public class SomeOtherService : ISomeOtherService { }
        public interface IBar { }
        public class Bar : IBar { public Bar(int num) { } }
        public class Bar2 : IBar { public Bar2(int num1, int num2, ISomeOtherService someOtherService) { } }
        public interface IBarFactory { IBar Factory(int num1, int num2); }

        [Fact]
        public void Verify_Parameter_Factory_Method_Match()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar, Bar>();
            sc.AddTransient<ISomeOtherService, SomeOtherService>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            Assert.NotNull(barFactory);

            var bar = barFactory.Factory(1, 2);
            Assert.NotNull(bar);
            Assert.IsType<Bar2>(bar);
        }
    }

    public class InjectedServicesTest3
    {
        public interface IBar1 { }
        public interface IBar2 { }
        public interface IBar3 { }
        public interface IBar4 { }

        public class SomeDependency : ISomeDependency { }
        public class Bar1 : IBar1 { public Bar1(Obj1 obj1, ISomeDependency someDependency) { } }
        public class Bar2 : IBar2 { public Bar2(Obj2 obj2, ISomeDependency someDependency, IBarFactory barFactory) { } }
        public class Bar3 : IBar3 { public Bar3(Obj3 obj3, ISomeDependency someDependency) { } }
        public class Bar4 : IBar4 { public Bar4(Obj4 obj4, ISomeDependency someDependency) { } }

        public class Obj1 { }
        public class Obj2 { }
        public class Obj3 { }
        public class Obj4 { }
        public interface IBarFactory
        {
            IBar1 Factory(Obj1 obj);
            IBar2 Factory(Obj2 obj);
            IBar3 Factory(Obj3 obj3);
            IBar4 Factory(Obj4 obj4);
        }

        [Fact]
        public void Verify_One_Dependency_Is_Injected_In_Multiple_Services()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar1, Bar1>();
            sc.AddTransient<IBar2, Bar2>();
            sc.AddTransient<IBar3, Bar3>();
            sc.AddTransient<IBar4, Bar4>();

            sc.AddTransient<ISomeDependency, SomeDependency>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            Assert.NotNull(barFactory);

            var bar1 = barFactory.Factory(new Obj1{ });
            Assert.NotNull(bar1);
            Assert.IsType<Bar1>(bar1);

        }
    }
}