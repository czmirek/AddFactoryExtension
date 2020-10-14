using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.AddFactoryExtension;
using Xunit;

namespace AddFactoryExtension.Tests
{
    public class FactoryMethodNotInterfaceExceptionTest
    {
        public class NotInterface { }
        public interface IBarFactory { NotInterface Factory(); }

        [Fact]
        public void Verify_NotInterface_Throws()
        {
            Assert.Throws<FactoryMethodNotInterfaceException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddFactory<IBarFactory>();

                var sp = sc.BuildServiceProvider();
                var barFactory = sp.GetRequiredService<IBarFactory>();
            });
        }
    }
}
