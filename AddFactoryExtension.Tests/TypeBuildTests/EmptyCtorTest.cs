using Microsoft.Extensions.DependencyInjection;
using Xunit;

/// <summary>
/// Tests that simple empty ctors are working
/// </summary>
namespace AddFactoryExtension.Tests
{
    public class EmptyCtorTest
    {
        public interface IBar { }
        public class Bar : IBar
        {
            public Bar() { }
        }
        public interface IBarFactory { IBar Factory(); }
        public class TestBarFactory : IBarFactory
        {
            public IBar Factory()
            {
                return new Bar();
            }
        }

        [Fact]
        public void Test_Factory_Without_Ctor()
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

    public class EmptyCtorTest2
    {
        public interface IBar { }
        public class Bar : IBar { }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Test_Factory_Without_Ctor()
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