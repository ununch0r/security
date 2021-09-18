using System;

namespace Lab3.RC5Cryptography.WordModels
{
    internal interface IWord
    {
        void CreateFromBytes(byte[] bytes, Int32 startFromIndex);
        Byte[] FillBytesArray(Byte[] bytesToFill, Int32 startFromIndex);
        IWord ROL(Int32 offset);
        IWord ROR(Int32 offset);
        IWord XorWith(IWord word);
        IWord Add(IWord word);
        IWord Add(byte value);
        IWord Sub(IWord word);
        IWord Clone();
        Int32 ToInt32();
    }
}
