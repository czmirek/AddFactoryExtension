namespace AddFactoryExtension.Tests
{
    public interface IBarFactory
    {
        IBar Factory(int intParam1, int intParam2);
        IBar Factory(string strParam1, int intParam2, double param3, decimal param4, long param5, string param6);
        IBar Factory(string strParam1, string strParam2);
        IBar Factory();
    }
}
