using System;
using System.Threading.Tasks;
using System.Windows;
using Lab2.HashAlgorithm.Concrete;
using Microsoft.Win32;

namespace Lab2.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MD5 _hasher;
        private readonly MD5 _fileHasher;
        public MainWindow()
        {
            InitializeComponent();

            _hasher = new MD5();
            _fileHasher = new MD5();
        }

        private async void UploadFile_OnClick(object sender, RoutedEventArgs e)
        {
            var filepath = string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                filepath = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                UploadFile.IsEnabled = false;
                await _fileHasher.ComputeFileHashAsync(filepath);
                ResultHash.Text = _fileHasher.HashAsString;
                MessageBox.Show("File Hash Generated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UploadFile.IsEnabled = false;
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void UploadHash_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Verify_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
