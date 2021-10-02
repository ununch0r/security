using Lab3.RC5Cryptography;
using Lab3.RC5Cryptography.Extensions;
using Lab3.RC5Cryptography.Options;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Lab3UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RC5 _rc5;
        private const KeyLengthInBytesEnum KeyLength = KeyLengthInBytesEnum.Bytes_8;

        public MainWindow()
        {
            InitializeComponent();
            _rc5 = new RC5(new AlgorithmSettings
            {
                RoundCount = RoundCountEnum.Rounds_8,
                WordLengthInBits = WordLengthInBitsEnum.Bit32
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                ChosenFile.Text = openFileDialog.FileName;
            }
        }

        private void EncipherBtn_Click(Object sender, EventArgs e)
        {
            if (!File.Exists(ChosenFile.Text))
            {
                MessageBox.Show("File not chosen!", "RC5");

                return;
            }

            try
            {
                var hashedKey = Encoding.UTF8
                    .GetBytes(Password.Text)
                    .GetMD5HashedKeyForRC5(KeyLength);

                var encodedFileContent = _rc5.EncipherCBCPAD(
                    File.ReadAllBytes(ChosenFile.Text),
                    hashedKey);

                File.WriteAllBytes(PaddFilename(ChosenFile.Text, "-enc"), encodedFileContent);

                MessageBox.Show("Enciphered", "RC5");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RC5");
            }
        }

        private void DecipherBtn_Click(Object sender, EventArgs e)
        {
            if (!File.Exists(ChosenFile.Text))
            {
                MessageBox.Show("File not choosen!", "RC5");

                return;
            }
            try
            {
                var hashedKey = Encoding.UTF8
                .GetBytes(Password.Text)
                .GetMD5HashedKeyForRC5(KeyLength);

                var decodedFileContent = _rc5.DecipherCBCPAD(
                    File.ReadAllBytes(ChosenFile.Text),
                    hashedKey);

                File.WriteAllBytes(PaddFilename(ChosenFile.Text, "-dec"), decodedFileContent);

                MessageBox.Show("Deciphered", "RC5");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RC5");
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
    }
}
