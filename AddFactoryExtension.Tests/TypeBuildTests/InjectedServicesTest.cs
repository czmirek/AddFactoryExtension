using Microsoft.Extensions.DependencyInjection;
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
}