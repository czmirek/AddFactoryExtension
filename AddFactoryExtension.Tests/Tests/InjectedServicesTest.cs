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
        public void Verify_Factory_Fails()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar, Bar>();
            sc.AddTransient<ISomeOtherService, SomeOtherService>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            Assert.NotNull(barFactory);
            
        }
    }
}