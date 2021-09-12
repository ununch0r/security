using Lab1.RandomNumberGenerator.Concrete;
using Lab1.RandomNumberGenerator.Models;
using Lab1UI.Managers;
using Lab1UI.Models;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lab1UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ulong _outputBufferInterval = 1000;
        private readonly StringBuilder _outputBuffer;
        private readonly HashManager _hashManager;

        private ulong _numbersCountToGenerate = 0;

        public MainWindow()
        {
            InitializeComponent();

            _hashManager = new HashManager();
            _hashManager.LogMessageToUi += HashManager_LogMessageToUI;
            _hashManager.NumbersGenerated += HashManager_NumbersGenerated;
            _hashManager.NumberGeneratedOutput += HashManager_NumberGeneratedOutput;

            _outputBuffer = new StringBuilder();

            AInput.Text = PseudoRandomNumberGeneratorOptions.CurrentValues.Multiplier.ToString();
            CInput.Text = PseudoRandomNumberGeneratorOptions.CurrentValues.Cummulative.ToString();
            ModInput.Text = PseudoRandomNumberGeneratorOptions.CurrentValues.Mod.ToString();
            StartValueInput.Text = PseudoRandomNumberGeneratorOptions.CurrentValues.StartValue.ToString();
            OpenFileButton.IsEnabled = false;
        }

        private void HashManager_LogMessageToUI(string message)
        {
            Logs.Text = message;
        }

        private void HashManager_NumberGeneratedOutput(string message)
        {
            Output(message);
        }

        private void HashManager_NumbersGenerated(ulong count)
        {
            CountLogs.Text = $"Numbers generated: {count}";
        }

        private async void Generate_Click(object sender, EventArgs e)
        {
            SetEnableUiElements(true);
            ClearOutputs();
            
            var stopWatch = new Stopwatch();

            _numbersCountToGenerate = ulong.Parse(NumbersCount.Text);

            stopWatch.Start();

            await _hashManager.ExecuteAsync(
                new LabExecutionOptions
                {
                    MaxOutputCountToUI = _numbersCountToGenerate
                },
                new PseudoRandomNumberGenerator(new PseudoRandomNumberGeneratorOptions
                {
                    Cummulative = ulong.Parse(CInput.Text),
                    Mod = ulong.Parse(ModInput.Text),
                    Multiplier = ulong.Parse(AInput.Text),
                    StartValue = ulong.Parse(StartValueInput.Text)
                }));

            stopWatch.Stop();

            ForceOutput();
            ShowTimeSpentMessageBox(stopWatch);
            SetEnableUiElements(false);
            OpenFileButton.IsEnabled = true;
        }

        private void SetEnableUiElements(bool isExecutionStarted)
        {
            GenerateButton.IsEnabled = !isExecutionStarted;

            if (isExecutionStarted)
            {
                Cursor = Cursors.Wait;
            }
            else
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void ClearOutputs()
        {
            CountLogs.Text = string.Empty;
            Logs.Text = string.Empty;
            TextLogs.Text = string.Empty;
        }

        private void ShowTimeSpentMessageBox(Stopwatch stopWatch)
        {
            TimeSpan ts = stopWatch.Elapsed;

            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            MessageBox.Show(this, elapsedTime, "Time spent");
        }

        private void Output(string message)
        {
            _outputBuffer.Append(message);

            if ((ulong)_outputBuffer.Length > _outputBufferInterval)
            {
                Task.Factory.StartNew(ForceOutput);
            }
        }

        private void ForceOutput()
        {
            TextLogs.Dispatcher.Invoke(() => TextLogs.Text += _outputBuffer.ToString());
            _outputBuffer.Clear();
        }

        private void OpenFileBtn_Click(object sender, EventArgs e)
        {
            var startInfo = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + _hashManager.OutputFileName)
            {
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
