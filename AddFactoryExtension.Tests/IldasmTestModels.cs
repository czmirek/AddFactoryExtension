using System;

namespace AddFactoryExtension.Tests
{
    public class Bar : IBar
    {
        private int intParam1;
        private int intParam2;
        private string strParam1;
        private double param3;
        private decimal param4;
        private long param5;
        private string param6;
        private string strParam2;
        private ISomeDependency someDependency;

        public Bar()
        {

        }

        public Bar(int intParam1, int intParam2)
        {
            this.intParam1 = intParam1;
            this.intParam2 = intParam2;
        }

        public Bar(string strParam1, string strParam2, ISomeDependency someDependency)
        {
            this.strParam1 = strParam1;
            this.strParam2 = strParam2;
            this.someDependency = someDependency;
        }

        public Bar(string strParam1, int intParam2, double param3, decimal param4, long param5, string param6)
        {
            this.strParam1 = strParam1;
            this.intParam2 = intParam2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.param6 = param6;
        }

        public void DoSomething()
        {
            Console.WriteLine("doing something");
        }
    }

    public interface IBar
    {
        void DoSomething();
    }

    public interface IBarFactory
    {
        IBar Factory(int intParam1, int intParam2);
        IBar Factory(string strParam1, int intParam2, double param3, decimal param4, long param5, string param6);
        IBar Factory(string strParam1, string strParam2);
        IBar Factory();
    }

    public interface ISomeDependency { }
    public interface ISomeOtherDependency { }
    public interface ISomeOtherDependency2 { }
    public interface ISomeOtherDependency3 { }
    public interface ISomeOtherDependency4 { }
    public interface ISomeOtherDependency5 { }
    public interface ISomeOtherDependency6 { }
    public interface ISomeOtherDependency7 { }

    public class FactoryImpl : IBarFactory
    {
        private readonly ISomeDependency someDependency;
        private readonly ISomeOtherDependency someOtherDependency;
        private readonly ISomeOtherDependency2 someOtherDependency2;
        private readonly ISomeOtherDependency2 someOtherDependency3;
        private readonly ISomeOtherDependency2 someOtherDependency4;
        private readonly ISomeOtherDependency2 someOtherDependency5;
        private readonly ISomeOtherDependency2 someOtherDependency6;
        private readonly ISomeOtherDependency2 someOtherDependency7;

        public FactoryImpl(ISomeDependency someDependency,
            ISomeOtherDependency someOtherDependency,
            ISomeOtherDependency2 someOtherDependency2,
            ISomeOtherDependency2 someOtherDependency3,
            ISomeOtherDependency2 someOtherDependency4,
            ISomeOtherDependency2 someOtherDependency5,
            ISomeOtherDependency2 someOtherDependency6,
            ISomeOtherDependency2 someOtherDependency7)
        {
            this.someDependency = someDependency;
            this.someOtherDependency = someOtherDependency;
            this.someOtherDependency2 = someOtherDependency2;
            this.someOtherDependency3 = someOtherDependency3;
            this.someOtherDependency4 = someOtherDependency4;
            this.someOtherDependency5 = someOtherDependency5;
            this.someOtherDependency6 = someOtherDependency6;
            this.someOtherDependency7 = someOtherDependency7;
        }

        public IBar Factory(int intParam1, int intParam2)
        {
            return new Bar(intParam1, intParam2);
        }

        public IBar Factory(string strParam1, int intParam2, double param3, decimal param4, long param5, string param6)
        {
            return new Bar(strParam1, intParam2, param3, param4, param5, param6);
        }

        public IBar Factory(string strParam1, string strParam2)
        {
            return new Bar(strParam1, strParam2, someDependency);
        }

        public IBar Factory()
        {
            return new Bar();
        }
    }
}
