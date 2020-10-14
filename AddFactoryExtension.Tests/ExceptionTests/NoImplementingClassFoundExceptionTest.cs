using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AddFactoryExtension.Tests
{
    public class NoImplementingClassFoundExceptionTest
    {
        public interface IBar { }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_NotInterface_Throws()
        {
            Assert.Throws<NoImplementingClassFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddFactory<IBarFactory>();

                var sp = sc.BuildServiceProvider();
                var barFactory = sp.GetRequiredService<IBarFactory>();
            });
        }
    }
}
