namespace AddFactoryExtension.Tests
{
    public class FactoryImplementation : IBarFactory
    {
        private readonly ISomeDependency someDependency;
        private readonly ISomeOtherDependency someOtherDependency;
        private readonly ISomeOtherDependency2 someOtherDependency2;
        private readonly ISomeOtherDependency2 someOtherDependency3;
        private readonly ISomeOtherDependency2 someOtherDependency4;
        private readonly ISomeOtherDependency2 someOtherDependency5;
        private readonly ISomeOtherDependency2 someOtherDependency6;
        private readonly ISomeOtherDependency2 someOtherDependency7;

        public FactoryImplementation(ISomeDependency someDependency,
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