using Lab1.RandomNumberGenerator.Interfaces;
using Lab1UI.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lab1UI.Managers
{
    public class HashManager
    {
        public event Action<string> LogMessageToUi;
        public event Action<string> NumberGeneratedOutput;
        public event Action<string> NumberGeneratedOutputUi;
        public event Action<ulong> NumbersGenerated;

        public HashManager()
        {
            OutputFileName = "@numbers.txt";
        }

        public string OutputFileName { get; set; }

        public async Task ExecuteAsync(
            LabExecutionOptions options,
            IPseudoRandomNumberGenerator numberGenerator)
        {
            await ExecuteDefaultComparingAsync(options, numberGenerator);
        }

        public async Task ExecuteDefaultComparingAsync(
                    LabExecutionOptions options,
                    IPseudoRandomNumberGenerator numberGenerator)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            DeleteOutputFile();

            var fs = new FileStream(OutputFileName, FileMode.CreateNew);
            using (var writer = new StreamWriter(fs, Encoding.Default, 8192))
            {
                bool isPeriodFound = false;
                bool isMaxCountOutputToUiReached = false;
                ulong outputCount = 0;

                ulong? firstNumber = null;
                ulong? secondNumber = null;
                ulong? previousNumber = null;
                ulong nextNumber = default;

                while (true)
                {
                    var tempPrevNumber = nextNumber;
                    nextNumber = numberGenerator.NextNumber();

                    if (outputCount == 0)
                    {
                        firstNumber = nextNumber;
                    }
                    else if (outputCount == 1)
                    {
                        secondNumber = nextNumber;
                    }
                    else
                    {
                        previousNumber = tempPrevNumber;
                    }

                    isMaxCountOutputToUiReached = outputCount >= options.MaxOutputCountToUI;

                    await OutputAsync(
                        writer,
                        $"{nextNumber} ",
                        isPeriodFound,
                        isMaxCountOutputToUiReached
                        );

                    ++outputCount;

                    if (outputCount % 1_000_000 == 0)
                    {
                        await writer.WriteAsync("Stelmashchuk ");
                        await writer.FlushAsync();
                    }

                    if (!isPeriodFound
                        && IsDuplicateFound(nextNumber, previousNumber, firstNumber, secondNumber, outputCount))
                    {
                        LogMessageToUi?.Invoke(
                            $"Sequence interval period found, unique count: {outputCount - 1}");

                        isPeriodFound = true;
                    }

                    if (isPeriodFound && isMaxCountOutputToUiReached)
                    {
                        break;
                    }
                }

                NumbersGenerated?.Invoke(outputCount - 1);
            }
        }

        private bool IsDuplicateFound(
            ulong currentNumber,
            ulong? previousNumber,
            ulong? firstNumber,
            ulong? secondNumber,
            ulong outputCount)
        {
            return (firstNumber.HasValue && firstNumber == currentNumber && outputCount > 1)
                || (secondNumber.HasValue && secondNumber == currentNumber && outputCount > 2)
                || (previousNumber.HasValue && previousNumber == currentNumber);
        }

        private void DeleteOutputFile()
        {
            if (File.Exists(OutputFileName))
            {
                File.Delete(OutputFileName);
            }
        }

        private async Task OutputAsync(
            StreamWriter writer,
            string message,
            bool isPeriodFound,
            bool isMaxOutPutToUiReached
            )
        {
            await writer.WriteAsync(message);
            if (!isMaxOutPutToUiReached)
            {
                NumberGeneratedOutput?.Invoke(message);
            }
        }
    }
}
