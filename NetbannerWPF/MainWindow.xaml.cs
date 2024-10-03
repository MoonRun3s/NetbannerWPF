using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using NetbannerWPF.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppBar;

namespace NetbannerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static RegistryKey SoftwareKey = Registry.LocalMachine.OpenSubKey("Software", true);
        //static RegistryKey ClassificationLevelKey = SoftwareKey.CreateSubKey("ClassificationLevel", true);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => InitAppBarState();
            ShowInTaskbar = false;
        }

        private void InitAppBarState()
        {
            AppBarFunctions.SetAppBar(this, ABEdge.Top);
            LeftLabel.Content = "NSA | " + Dns.GetHostEntry(Dns.GetHostName().ToString()).AddressList[0].ToString() + " | " + Environment.UserName.ToString().ToUpper();
            RightLabel.Content = "NOFORN";
            Banner.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fce83a"));
        }

        void FormLoaded(object sender, RoutedEventArgs args)
        {
            // Variable to hold the handle for the form
            var helper = new WindowInteropHelper(this).Handle;
            // Hide the form from Alt+Tab menu
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }
    }
}
