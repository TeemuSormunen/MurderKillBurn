using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace harjoitustyo
{
    /// <summary>
    /// Interaction logic for NameWindow.xaml
    /// </summary>
    public partial class NameWindow : Window
    {
        public string input;
        public NameWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            input = txtName.Text;

            if (input == "Insert Player Name" || input == "")
            {
                MessageBox.Show("Give player name!");
            }

            else
            {
                StartWindow startwindow = new StartWindow();
                startwindow.Show();
                foreach (Window item in App.Current.Windows)
                {
                    if (item != startwindow)
                        item.Close();
                }

            }
        }
    }
}
