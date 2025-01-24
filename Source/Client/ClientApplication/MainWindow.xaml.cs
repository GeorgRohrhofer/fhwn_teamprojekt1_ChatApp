using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientApplication.Pages;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// constructor to create a new MainWindow instance.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            this.Width = 700;
            this.Height = 850;
            this.Top = 0;
            this.Left = 0;

            MainFrame.Navigate(new Welcome_Window(this));
        }
    }
}