using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab5.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DSACryptoServiceProvider _dsa = new DSACryptoServiceProvider();
        private readonly SHA1 _sha1 = SHA1.Create();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateFromFileBtn_Click(Object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                FileInput.Text = filePath;

                ProcessSignature(File.ReadAllBytes(filePath));
            }
        }

        private void CreateFromTextBtn_Click(Object sender, EventArgs e)
        {
            ProcessSignature(Encoding.Default.GetBytes(TextInput.Text));
        }

        private void SaveSigBtn_Click(Object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SignatureOutput.Text))
            {
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                RestoreDirectory = true,
                FileName = "Signature.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, SignatureOutput.Text);
            }
        }

        private void ProcessSignature(Byte[] message)
        {
            byte[] hash = _sha1.ComputeHash(message);
            string result = Convert.ToBase64String(_dsa.CreateSignature(hash));

            SignatureOutput.Text = result.Trim();
        }

        private bool VerifySignature(byte[] message, string sign)
        {
            try
            {
                byte[] hash = _sha1.ComputeHash(message);
                bool verified = _dsa.VerifySignature(hash, Convert.FromBase64String(sign));
                return verified;
            }
            catch
            {
                return false;
            }
        }

        private void BtnPickFile_Click(Object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                ChosenFile.Text = openFileDialog.FileName;
            }
        }

        private void PickSigBtn_Click(Object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                ChosenSignatureFile.Text = openFileDialog.FileName;
            }
        }

        private void VerifyBtn_Click(Object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ChosenFile.Text) || string.IsNullOrWhiteSpace(ChosenSignatureFile.Text))
            {
                return;
            }

            byte[] message = File.ReadAllBytes(ChosenFile.Text);
            string sign = File.ReadAllText(ChosenSignatureFile.Text);

            var result = VerifySignature(message, sign)
                ? "Verified"
                : "Not verified";

            MessageBox.Show(result);
        }
    }
}
