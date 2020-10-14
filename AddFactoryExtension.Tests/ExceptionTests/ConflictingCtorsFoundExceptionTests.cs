using Microsoft.Extensions.DependencyInjection;
using Xunit;

/// <summary>
/// Tests that verify exception throw when multiple ctors with 
/// same signature (accounting for additional services) are detected 
/// for a single factory method
/// </summary>
namespace AddFactoryExtension.Tests
{
    public class ConflictingCtorsFoundExceptionTest1
    {
        public interface IBar { }
        public class Bar : IBar { }
        public class Bar2 : IBar { }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }

    public class ConflictingCtorsFoundExceptionTest2
    {
        public interface IBar { }
        public class Bar : IBar { public Bar() { } }
        public class Bar2 : IBar { public Bar2() { } }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }

    public class ConflictingCtorsFoundExceptionTest3
    {
        public interface IBar { }
        public class Bar : IBar { }
        public class Bar2 : IBar { public Bar2() { } }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }

    public class ConflictingCtorsFoundExceptionTest4
    {
        public interface IBar { }
        public class Bar : IBar { public Bar() { } }
        public class Bar2 : IBar {  }
        public interface IBarFactory { IBar Factory(); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }

    public class ConflictingCtorsFoundExceptionTest5
    {
        public interface IBar { }
        public class Bar : IBar { public Bar(int num) { } }
        public class Bar2 : IBar { public Bar2(int num) { } }
        public interface IBarFactory { IBar Factory(int num); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }

    public class ConflictingCtorsFoundExceptionTest6
    {
        public interface IBar { }
        public class Bar : IBar { public Bar(int num) { } }
        public class Bar2 : IBar { public Bar2(int differentName) { } }
        public interface IBarFactory { IBar Factory(int num); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }

    public class ConflictingCtorsFoundExceptionTest7
    {
        public interface ISomeOtherService { }
        public interface IBar { }
        public class Bar : IBar { public Bar(int num, string str, ISomeOtherService someOtherService) { } }
        public class Bar2 : IBar { 
            public Bar2() { }
            public Bar2(int num, string str, ISomeOtherService someOtherService) { } 
        }
        public interface IBarFactory
        {
            IBar Factory(int num, string str);
        }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }

    public class ConflictingCtorsFoundExceptionTest8
    {
        public interface ISomeOtherService { }
        public class SomeOtherService : ISomeOtherService { }
        public interface IBar { }
        public class Bar : IBar { public Bar(int num) { } }
        public class Bar2 : IBar { public Bar2(int num, ISomeOtherService someOtherService) { } }
        public interface IBarFactory { IBar Factory(int num); }

        [Fact]
        public void Verify_AddFactory_Fails()
        {
            Assert.Throws<ConflictingCtorsFoundException>(() =>
            {
                ServiceCollection sc = new ServiceCollection();
                sc.AddTransient<IBar, Bar>();
                sc.AddFactory<IBarFactory>();
            });
        }
    }
}