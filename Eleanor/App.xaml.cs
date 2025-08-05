using H.NotifyIcon;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;


namespace Eleanor
{

    public partial class App : Application
    {
        public static Window? window { get; set; }
        private Process _process = new Process();
        public static AppWindow? m_AppWindow { get; set; }

        public App()
        {
            InitializeComponent();


        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {

            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            try
            {
                File.AppendAllText(logPath, $"{DateTime.Now}: Launched {Environment.NewLine}");

                killAllZaprets(logPath);
                File.AppendAllText(logPath, $"{DateTime.Now}: Killed Zaprets; {Environment.NewLine}");

                window = new MainWindow(ref _process);

                m_AppWindow = window.AppWindow;

                IntPtr hWnd = WindowNative.GetWindowHandle(window);

                string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\eslo0.ico");

                IconHelper.SetWindowIcon(hWnd, iconPath);

                window.Activate();

                

                File.AppendAllText(logPath, $"{DateTime.Now}: OnLaunched Succes - {Environment.NewLine}");
            }
            catch (Exception ex)
            {
                File.AppendAllText(logPath, $"{DateTime.Now}: Error in OnLaunched - {ex}{Environment.NewLine}");
                throw; 
            }
        }

        protected void killAllZaprets(string logFile)
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == "winws")
                {
                    File.AppendAllText(logFile, $"{DateTime.Now}: Killed: {process.ProcessName} {Environment.NewLine}");
                    process.Kill();
                }
            }
        }

    }
}
