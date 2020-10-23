using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AddFactoryExtension.Tests
{
    public class ReinjectedFactoryTest
    {
        public interface IBar { }
        public class Bar : IBar
        {
            private readonly IBarFactory barFactory;

            public Bar(IBarFactory barFactory)
            {
                this.barFactory = barFactory;
            }
        }

        public interface IBarFactory { IBar Factory(); }
        public class TestBarFactory : IBarFactory
        {
            public IBar Factory()
            {
                return new Bar(this);
            }
        }

        [Fact]
        public void Test_Factory_Reinjections()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar, Bar>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            IBar bar = barFactory.Factory();
            Assert.NotNull(bar);
        }
    }
}
