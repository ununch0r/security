using System;
using System.Windows;

namespace Lab4.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


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

    }
}
