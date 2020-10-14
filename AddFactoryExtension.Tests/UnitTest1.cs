using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace AddFactoryExtension.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Base_Test()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar, Bar>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            barFactory.Factory(1, 2);
        }
    }
}
