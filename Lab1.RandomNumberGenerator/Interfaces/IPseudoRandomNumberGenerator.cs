namespace Lab1.RandomNumberGenerator.Interfaces
{
    public interface IPseudoRandomNumberGenerator
    {
        ulong NextNumber();
        ulong NextNumber(ulong currentNumber);
        void Reset();
    }
}
