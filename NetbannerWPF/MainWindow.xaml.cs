using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using NetbannerWPF.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
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
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        public MainWindow()
        {
            InitializeComponent();
            AdminRelauncher();
            this.Loaded += (s, e) => InitAppBarState();
            ShowInTaskbar = false;
        }

        private void AdminRelauncher()
        {
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator.\n\n" + ex.ToString());
                }
            }
        }

        private bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetClassification(string ClassificationType, string BarHex, string TextHex)
        {
            Banner.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BarHex));
            Label.Content = ClassificationType;
            Label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextHex));
            RightLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextHex));
            LeftLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(TextHex));
        }

        private void InitAppBarState()
        {
            AppBarFunctions.SetAppBar(this, ABEdge.Top);

            // Registry Settings
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\NetbannerWPF");
            string agency = (string)SoftwareKey.GetValue("Agency", "NONE");
            bool ip = (bool)SoftwareKey.GetValue("ShowIP", false);
            bool showuser = (bool)SoftwareKey.GetValue("ShowUser", false);
            string classification = (string)SoftwareKey.GetValue("Classification", "Classification not set");
            string rlc = (string)SoftwareKey.GetValue("RLC", "");
            SoftwareKey.Close();

            LeftLabel.Content = agency;
            if (ip) { LeftLabel.Content += " | " + Dns.GetHostEntry(Dns.GetHostName().ToString()).AddressList[0].ToString(); }
            if (showuser) { LeftLabel.Content += " | " + Environment.UserName.ToString().ToUpper(); }

            RightLabel.Content = rlc;
            SetClassification(classification, "#fce83a", "#000000");
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
