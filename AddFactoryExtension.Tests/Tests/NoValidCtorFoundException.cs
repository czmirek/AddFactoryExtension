using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.AddFactoryExtension;
using Xunit;

/// <summary>
/// Tests that verify exception throw when multiple ctors with 
/// same signature (accounting for additional services) are detected 
/// for a single factory method
/// </summary>
namespace AddFactoryExtension.Tests
{
    public class NoValidCtorFoundExceptionTest1
    {
        public interface IBar { }
        public class Bar : IBar
        {
            public Bar(int num1) { }
        }
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