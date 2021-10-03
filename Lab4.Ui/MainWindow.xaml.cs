using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Lab3.RC5Cryptography;
using Lab3.RC5Cryptography.Extensions;
using Lab3.RC5Cryptography.Options;
using Microsoft.Win32;

namespace Lab4.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _filepathRc5;
        private string _filepathRsa;

        private const KeyLengthInBytesEnum KeyLength = KeyLengthInBytesEnum.Bytes_16;

        private const int EncipherBlockSizeRSA = 64;
        private const int DecipherBlockSizeRSA = 128;

        private readonly RC5 _rc5;
        private readonly RSACryptoServiceProvider _rsa;

        public MainWindow()
        {
            InitializeComponent();
            InitializeKeyInput();

            _rc5 = new RC5(new AlgorithmSettings
            {
                RoundCount = RoundCountEnum.Rounds_16,
                WordLengthInBits = WordLengthInBitsEnum.Bit32
            });

            _rsa = new RSACryptoServiceProvider();
        }

        private void InitializeKeyInput()
        {
            KeyInput.Text = Constants.KeyPlaceholder;
            KeyInput.GotFocus += RemoveText;
            KeyInput.LostFocus += AddText;
        }


        public void RemoveText(object sender, EventArgs e)
        {
            if (KeyInput.Text == Constants.KeyPlaceholder)
            {
                KeyInput.Text = string.Empty;
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(KeyInput.Text))
                KeyInput.Text = Constants.KeyPlaceholder;
        }

        private bool TryOpenFile(string dlgTitle, out string filepath, out string filename)
        {
            bool isFilenameRetrieved = false;
            filepath = null;
            filename = null;

            var openFileDialog = new OpenFileDialog
            {
                Title = dlgTitle, RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.SafeFileName;
                filepath = openFileDialog.FileName;
                isFilenameRetrieved = true;
            }

            return isFilenameRetrieved;
        }

        private void ChooseRc5File_OnClick(object sender, RoutedEventArgs e)
        {
            if (TryOpenFile("Select file for RC5", out _filepathRc5, out var filename))
            {
                Rc5FileName.Content = "Filename: " + filename;
                Rc5FileName.Foreground = new SolidColorBrush(Colors.DarkGreen);

                LogMessage($"Selected file for RC5: '{_filepathRc5}'");
            }
        }

        private void LogMessage(string message, string logLevel = "INFO")
        {
            LogTextBlock.Dispatcher.BeginInvoke(new Action(() =>
            {
                LogTextBlock.Text += $"{DateTime.Now.ToLongTimeString()} [{logLevel}]: {message}" +
                                     Environment.NewLine;
            }));
        }

        private void ChooseRsaFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (TryOpenFile("Select file for RSA", out _filepathRsa, out var filename))
            {
                RsaFileName.Content = "Filename: " + filename;
                RsaFileName.Foreground = new SolidColorBrush(Colors.DarkGreen);

                LogMessage($"Selected file for RSA: '{_filepathRsa}'");
            }
        }

        private void ImportKeys_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryOpenFile("Import RSA keys", out var rsaKeysPath, out var filename)) //костиль)
                    return;

                _rsa.FromXmlString(File.ReadAllText(rsaKeysPath));

                LogMessage($"Successfully imported RSA keys from '{rsaKeysPath}'");
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message, "ERROR");
                LogMessage("Import RSA keys failed", "ERROR");
            }
        }

        private void ExportKeys_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDlg = new SaveFileDialog
                {
                    Title = "Export RSA keys",
                    FileName = "RSA_Keys.xml"
                };

                var dlgRes = saveFileDlg.ShowDialog();

                if (dlgRes == true)
                {
                    File.WriteAllText(
                        saveFileDlg.FileName,
                        _rsa.ToXmlString(includePrivateParameters: true));

                    LogMessage($"File '{saveFileDlg.FileName}' saved.");
                }
                else
                {
                    LogMessage($"File save cancelled.", "WARNING");
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message, "ERROR");
                LogMessage("Export RSA keys failed", "ERROR");
            }
        }

        private void ClearLogs_OnClick(object sender, RoutedEventArgs e)
        {
            LogTextBlock.Text = string.Empty;
        }

        private async void RsaEncipher_OnClick(object sender, RoutedEventArgs e)
        {
            string filePadding;
            var inputBytes = await File.ReadAllBytesAsync(_filepathRsa);
            LogMessage(
                $"{inputBytes.Length} bytes read from file: {_filepathRsa}.",
                "INFO");
            Byte[] encryptedBytes;

            if (PublicRadioButton.IsChecked == true)
            {
                encryptedBytes = await EncipherRSAAsync(inputBytes);
                filePadding = "-enc-rsa";

            }
            else if (PrivateRadioButton.IsChecked == true)
            {
                encryptedBytes= await PrivateRsaEncipher(inputBytes);
                filePadding = "-enc-pr-rsa";
            }
            else
            {
                return;
            }

            SaveBytesToFile(
                encryptedBytes,
                "Save RSA enciphered file",
                _filepathRsa,
                filePadding);
        }

        private async Task<Byte[]> PrivateRsaEncipher(byte[] inputBytes)
        {
            var stopWatch = new Stopwatch();
            var encipheredBytes = new List<byte>
            {
                Capacity = inputBytes.Length * 2
            };

            stopWatch.Start();

            await Task.Run(() =>
            {
                for (int i = 0; i < inputBytes.Length; i += EncipherBlockSizeRSA)
                {
                    var inputBlock = inputBytes
                        .Skip(i)
                        .Take(EncipherBlockSizeRSA)
                        .ToArray();

                    encipheredBytes.AddRange(_rsa.PrivateEncipher(inputBlock));
                }
            });

            stopWatch.Stop();

            LogMessage(
                $"Private RSA enciphered in {stopWatch.ElapsedMilliseconds} ms.",
                "INFO");

            return encipheredBytes.ToArray();
        }

        private async Task<Byte[]> EncipherRSAAsync(Byte[] inputBytes)
        {
            var stopWatch = new Stopwatch();
            var encipheredBytes = new List<byte>
            {
                Capacity = inputBytes.Length * 2
            };

            stopWatch.Start();

            await Task.Run(() =>
            {
                for (int i = 0; i < inputBytes.Length; i += EncipherBlockSizeRSA)
                {
                    var inputBlock = inputBytes
                        .Skip(i)
                        .Take(EncipherBlockSizeRSA)
                        .ToArray();

                    encipheredBytes.AddRange(_rsa.Encrypt(
                        inputBlock,
                        fOAEP: false));
                }
            });

            stopWatch.Stop();

            LogMessage(
                $"RSA enciphered in {stopWatch.ElapsedMilliseconds} ms.",
                "INFO");

            return encipheredBytes.ToArray();
        }

        private void Rc5Encipher_OnClick(object sender, RoutedEventArgs e)
        {
            var key = KeyInput.Text;
            if (key == Constants.KeyPlaceholder)
            {
                key = string.Empty;
            }

            var stopWatch = new Stopwatch();
            var rc5HashedKey = Encoding.UTF8
                .GetBytes(key)
                .GetMD5HashedKeyForRC5(KeyLength);

            var inputBytes = File.ReadAllBytes(_filepathRc5);
            LogMessage(
                $"{inputBytes.Length} bytes read from file: {_filepathRc5}.",
                "INFO");

            stopWatch.Start();
            var rc5EncipheredBytes = _rc5.EncipherCBCPAD(
                inputBytes,
                rc5HashedKey);
            stopWatch.Stop();
            LogMessage(
                $"RC5 enciphered in {stopWatch.ElapsedMilliseconds} ms.",
                "INFO");

            SaveBytesToFile(
                rc5EncipheredBytes,
                "Save RC5 enciphered file",
                _filepathRc5,
                "-enc-rc5");
        }

        private void SaveBytesToFile(
            Byte[] rc5EncipheredBytes,
            string dlgTitle,
            string oldFileName,
            string filenamePadding)
        {
            var saveFileDlg = new SaveFileDialog
            {
                Title = dlgTitle,
                FileName = PaddFilename(
                    oldFileName,
                    filenamePadding)
            };

            var dlgRes = saveFileDlg.ShowDialog();

                if (dlgRes == true)
                {
                    File.WriteAllBytes(
                        saveFileDlg.FileName,
                        rc5EncipheredBytes);

                    LogMessage($"File '{saveFileDlg.FileName}' saved.", "INFO");
                }
                else
                {
                    LogMessage($"File save cancelled.", "WARNING");
                }
        }

        private static string PaddFilename(string filePath, string padding)
        {
            var fi = new FileInfo(filePath);
            var fn = Path.GetFileNameWithoutExtension(filePath);

            return Path.Combine(
                fi.DirectoryName,
                fn + padding + fi.Extension);
        }

        private void DecipherRc5_OnClick(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(_filepathRc5))
            {
                var exMsg = "Failed to decipher, file to decipher not chosen.";
                LogMessage(exMsg, "ERROR");
                MessageBox.Show(exMsg, "Lab4");
                return;
            }

            try
            {
                PrecessDecipherRc5();
            }
            catch (Exception exception)
            {
                LogMessage(exception.Message, "ERROR");
                LogMessage("Process Decipher failed", "ERROR");
            }
        }

        private void PrecessDecipherRc5()
        {
            var key = KeyInput.Text;
            if (key == Constants.KeyPlaceholder)
            {
                key = string.Empty;
            }

            var stopWatch = new Stopwatch();
            var hashedKey = Encoding.UTF8
                .GetBytes(key)
                .GetMD5HashedKeyForRC5(KeyLength);

            var bytesToBeDecipheredRC5 = File.ReadAllBytes(_filepathRc5);
            LogMessage(
                $"{bytesToBeDecipheredRC5.Length} bytes read from file: {_filepathRc5}.",
                "INFO");

            stopWatch.Start();
            var decodedFileContent = _rc5.DecipherCBCPAD(
                bytesToBeDecipheredRC5,
                hashedKey);
            stopWatch.Stop();
            LogMessage(
                $"RC5 deciphered in {stopWatch.ElapsedMilliseconds} ms.",
                "INFO");

            SaveBytesToFile(
                decodedFileContent,
                "Save RC5 deciphered file",
                _filepathRc5,
                "-dec-rc5");
        }

        private async void DecipherRsa_OnClick(object sender, RoutedEventArgs e)
        {
            string filePadding;
            var inputBytes = await File.ReadAllBytesAsync(_filepathRsa);
            LogMessage(
                $"{inputBytes.Length} bytes read from file: {_filepathRsa}.",
                "INFO");
            Byte[] decipheredBytes;

            if (PublicRadioButton.IsChecked == true)
            {
                decipheredBytes = await DecipherRsaAsync(inputBytes);
                filePadding = "-dec-rsa";

            }
            else if (PrivateRadioButton.IsChecked == true)
            {
                decipheredBytes = await PrivateRsaDecipherAsync(inputBytes);
                filePadding = "-dec-pr-rsa";
            }
            else
            {
                return;
            }

            SaveBytesToFile(
                decipheredBytes,
                "Save RSA deciphered file",
                _filepathRsa,
                filePadding);
        }

        private async Task<Byte[]> PrivateRsaDecipherAsync(Byte[] inputBytes)
        {
            var stopWatch = new Stopwatch();
            var decipheredBytes = new List<byte>();

            stopWatch.Start();

            await Task.Run(() =>
            {
                for (int i = 0; i < inputBytes.Length; i += DecipherBlockSizeRSA)
                {
                    var inputBlock = inputBytes
                        .Skip(i)
                        .Take(DecipherBlockSizeRSA)
                        .ToArray();

                    decipheredBytes.AddRange(_rsa.PublicDecipher(inputBlock));
                }
            });

            stopWatch.Stop();

            LogMessage(
                $"RSA deciphered in {stopWatch.ElapsedMilliseconds} ms.",
                "INFO");

            return decipheredBytes.ToArray();
        }

        private async Task<Byte[]> DecipherRsaAsync(Byte[] inputBytes)
        {
            var stopWatch = new Stopwatch();
            var decipheredBytes = new List<byte>
            {
                Capacity = inputBytes.Length / 2
            };

            stopWatch.Start();

            await Task.Run(() =>
            {
                for (int i = 0; i < inputBytes.Length; i += DecipherBlockSizeRSA)
                {
                    var inputBlock = inputBytes
                        .Skip(i)
                        .Take(DecipherBlockSizeRSA)
                        .ToArray();

                    decipheredBytes.AddRange(_rsa.Decrypt(
                        inputBlock,
                        fOAEP: false));
                }
            });

            stopWatch.Stop();

            LogMessage(
                $"RSA deciphered in {stopWatch.ElapsedMilliseconds} ms.",
                "INFO");

            return decipheredBytes.ToArray();
        }
    }
}
