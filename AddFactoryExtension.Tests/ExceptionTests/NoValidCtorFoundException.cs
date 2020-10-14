using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AddFactoryExtension.Tests
{
    public class NoValidCtorFoundExceptionTest1
    {
        public interface IBar { }
        public class Bar : IBar { public Bar(int num1) { } }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<NoValidCtorFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }
}