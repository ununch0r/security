using Lab2.HashAlgorithm.Concrete;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab2.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MD5 _hasher;
        public MainWindow()
        {
            InitializeComponent();

            _hasher = new MD5();
        }

        private async void UploadFile_OnClick(object sender, RoutedEventArgs e)
        {
            Input.Clear();

            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    UploadFile.IsEnabled = false;
                    await _hasher.HashFileAsync(openFileDialog.FileName);
                    ResultHash.Text = _hasher.HashAsString;
                    MessageBox.Show("File Hash Generated");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    UploadFile.IsEnabled = true;
                }
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ResultHash.Text))
            {
                MessageBox.Show("Nothing to save!");

                return;
            }

            var openFileDialog = new SaveFileDialog
            {
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(openFileDialog.FileName, ResultHash.Text);
            }
        }

        private void UploadHash_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadedHash.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void Verify_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ResultHash.Text)
                || string.IsNullOrWhiteSpace(LoadedHash.Text))
            {
                MessageBox.Show("Result or loaded hash is not set!");

                return;
            }

            var isHashMatch = string.Equals(
                ResultHash.Text.Trim(),
                LoadedHash.Text.Trim(),
                StringComparison.CurrentCultureIgnoreCase);

            if (isHashMatch)
            {
                VerificationStatus.Content = "Verified";
                VerificationStatus.Background = new SolidColorBrush(Colors.LawnGreen);
            }
            else
            {
                VerificationStatus.Content = "Failed";
                VerificationStatus.Background = new SolidColorBrush(Colors.Red);
            }

            VerificationStatus.Visibility = Visibility.Visible;
        }

        private void Input_OnKeyUp(object sender, KeyEventArgs e)
        {
            _hasher.ComputeHash(Input.Text);
            ResultHash.Text = _hasher.HashAsString;
        }
    }
}
