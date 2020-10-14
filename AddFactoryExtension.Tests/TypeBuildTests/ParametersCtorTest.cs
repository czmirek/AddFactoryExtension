using Microsoft.Extensions.DependencyInjection;
using Xunit;

/// <summary>
/// Tests that simple empty ctors are working
/// </summary>
namespace AddFactoryExtension.Tests
{
    public class ParametersCtorTest1
    {
        public interface ISomeOtherService { }
        public class SomeOtherService : ISomeOtherService { }
        public interface IBar { }
        public class Bar : IBar { public Bar(int num) { } }
        public interface IBarFactory { IBar Factory(int num); }

        [Fact]
        public void Verify_Parameter_Ctor()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar, Bar>();
            sc.AddTransient<ISomeOtherService, SomeOtherService>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            Assert.NotNull(barFactory);

            var bar = barFactory.Factory(123);
            Assert.NotNull(bar);
            Assert.IsType<Bar>(bar);
        }
    }

    public class ParametersCtorTest2
    {
        public interface ISomeOtherService { }
        public class SomeOtherService : ISomeOtherService { }
        public interface IBar 
        { 
            int? Num1 { get; }
            int? Num2 { get; }
            string Str1 { get; }
        }
        public class Bar : IBar
        {
            public int? Num1 { get; private set; }
            public int? Num2 { get; private set; }
            public string Str1 { get; private set; }

            public Bar(int num1) {
                Num1 = num1;    
            }
            public Bar(int num1, int num2) {
                Num1 = num1;
                Num2 = num2;
            }
            public Bar(int num1, string str1)
            {
                Num1 = num1;
                Str1 = str1;
            }
        }
        public interface IBarFactory 
        {
            IBar Factory(int num);
            IBar Factory(int num1, int num2);
            IBar Factory(int num1, string str1);
        }

        [Fact]
        public void Verify_Parameter_Ctor()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.AddTransient<IBar, Bar>();
            sc.AddTransient<ISomeOtherService, SomeOtherService>();
            sc.AddFactory<IBarFactory>();

            var sp = sc.BuildServiceProvider();
            var barFactory = sp.GetRequiredService<IBarFactory>();
            Assert.NotNull(barFactory);

            var bar = barFactory.Factory(123);
            Assert.NotNull(bar);
            Assert.IsType<Bar>(bar);
            Assert.Equal(123, bar.Num1);
            Assert.Null(bar.Num2);
            Assert.Null(bar.Str1);

            var bar2 = barFactory.Factory(123, 456);
            Assert.NotSame(bar, bar2);
            Assert.NotNull(bar2);
            Assert.IsType<Bar>(bar2);
            Assert.Equal(123, bar2.Num1);
            Assert.Equal(456, bar2.Num2);
            Assert.Null(bar2.Str1);

            var bar3 = barFactory.Factory(123, "some string");
            Assert.NotNull(bar3);
            Assert.NotSame(bar, bar3);
            Assert.NotSame(bar2, bar3);
            Assert.IsType<Bar>(bar3);
            Assert.Equal(123, bar3.Num1);
            Assert.Equal("some string", bar3.Str1);
            Assert.Null(bar3.Num2);
        }
    }
}