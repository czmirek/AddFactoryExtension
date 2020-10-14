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
}
