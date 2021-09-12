using System;

namespace Lab1.RandomNumberGenerator.Models
{
    public class PseudoRandomNumberGeneratorOptions
    {
        static PseudoRandomNumberGeneratorOptions()
        {
            var biggest = false;
            if(biggest)
            {    
                CurrentValues = new PseudoRandomNumberGeneratorOptions
                {
                    Mod = (ulong)Math.Pow(2, 31) -1,
                    Multiplier = (ulong)Math.Pow(7, 5),
                    Cummulative = 17711,
                    StartValue = 31
                };
            }
            else
            {
                CurrentValues = new PseudoRandomNumberGeneratorOptions
                {
                    Mod = (ulong)Math.Pow(2, 26) - 1,
                    Cummulative = 1597,
                    Multiplier = (ulong)Math.Pow(13, 3),
                    StartValue = 13
                };
            }
        }

        public static PseudoRandomNumberGeneratorOptions CurrentValues { get; }

        public ulong Mod { get; set; }

        public ulong Multiplier { get; set; }

        public ulong Cummulative { get; set; }

        public ulong StartValue { get; set; }
    }
}
