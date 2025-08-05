using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Shell;
using WinRT.Interop;


namespace Eleanor
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;

        public Process _process;

        private bool isRunning = false;


        private int appWidth = 400, appHeight = 600;


        public MainWindow(ref Process process)
        {

            _process = process;

            InitializeComponent();
            m_AppWindow = GetAppWindowForCurrentWindow();
            OverlappedPresenter _presenter = m_AppWindow.Presenter as OverlappedPresenter;

            // AppWindow
            m_AppWindow.SetIcon(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\eslo0.png");

            m_AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            m_AppWindow.Resize(new Windows.Graphics.SizeInt32(appWidth, appHeight));
            _presenter.IsResizable = false;
            _presenter.IsMaximizable = false;

            m_AppWindow.Closing += killZapret;
            m_AppWindow.Closing += OnClosed;


            // initialize UI
            initUI();

        }

        private void initUI()
        {
            this.ToggleBtn.Width = appWidth - 80;
        }

        private void ToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleBtn.IsChecked == true)
            {
                Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                isRunning = NewStartZapret();
                Debug.WriteLine(isRunning);
            }
            else
            {
                killZapret();
            }

            ToggleBtn.Content = (isRunning) ? "Connected" : "Disconnected";
            TrayState.Text = (isRunning) ? "Connected" : "Disconnected";
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }


        private bool StartZapret()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\zapret\\zapret-winws\\";
            string exePath = System.IO.Path.Combine(basePath, "winws.exe");

            string arguments =
                "--wf-tcp=80,443 " +
                "--wf-udp=443,50000-50099 " +
                "--filter-tcp=80 " +
                "--dpi-desync=fake,fakedsplit " +
                "--dpi-desync-autottl=2 " +
                "--dpi-desync-fooling=md5sig " +
                "--new " +
                "--filter-tcp=443 " +
                "--hostlist=\"" + System.IO.Path.Combine(basePath, @"files\list-youtube.txt") + "\" " +
                "--dpi-desync=fake,multidisorder " +
                "--dpi-desync-split-pos=1,midsld " +
                "--dpi-desync-repeats=11 " +
                "--dpi-desync-fooling=md5sig " +
                "--dpi-desync-fake-tls-mod=rnd,dupsid,sni=www.google.com " +
                "--new " +
                "--filter-tcp=443 " +
                "--dpi-desync=fake,multidisorder " +
                "--dpi-desync-split-pos=midsld " +
                "--dpi-desync-repeats=6 " +
                "--dpi-desync-fooling=badseq,md5sig " +
                "--new " +
                "--filter-udp=443 " +
                "--hostlist=\"" + System.IO.Path.Combine(basePath, @"files\list-youtube.txt") + "\" " +
                "--dpi-desync=fake " +
                "--dpi-desync-repeats=11 " +
                "--dpi-desync-fake-quic=\"" + System.IO.Path.Combine(basePath, @"files\quic_initial_www_google_com.bin") + "\" " +
                "--new " +
                "--filter-udp=443 " +
                "--dpi-desync=fake " +
                "--dpi-desync-repeats=11 " +
                "--new " +
                "--filter-udp=50000-50099 " +
                "--filter-l7=discord,stun " +
                "--dpi-desync=fake";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = basePath,
            };

            try
            {
                this._process = Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        private bool NewStartZapret()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\zapret\\zapret-winws-ytds\\";
            string exePath = System.IO.Path.Combine(basePath, @"bin\winws.exe");


            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = BuildArguments(basePath),
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = basePath,
            };

                try
            {
                this._process = Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        private bool killZapret()
        {
            if (isRunning)
            {
                _process.Kill();
                isRunning = false;
                return true;
            }
            return false;
        }

        private void killZapret(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            killZapret();
        }

        private void OnClosed(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            trayIcon.Dispose();
        }

        [RelayCommand]
        private void ShowHideWindow()
        {
            if (m_AppWindow.IsVisible)
            {
                m_AppWindow.Hide();
            } else
            {
                m_AppWindow.Show();
            }
        }

        private void HideToTray(object sender, RoutedEventArgs e)
        {
            m_AppWindow.Hide();
        }

        [RelayCommand]
        public void ExitApplication()
        {
            killZapret();
            trayIcon.Dispose();
            App.window?.Close();
        }

        [RelayCommand]
        public void ConnectDisconnect()
        {
            if (isRunning)
            {
                killZapret();
                TrayState.Text = "Disconnected";
                isRunning = false;
            }
            else
            {
                StartZapret();
                TrayState.Text = "Connected";
                isRunning = true;
            }
            ToggleBtn.IsChecked = isRunning;
            ToggleBtn.Content = (isRunning) ? "Connected" : "Disconnected";
        }

        public void ShowAboutWindow(object sender, RoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            IntPtr hWnd = WindowNative.GetWindowHandle(aboutWindow);

            string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets\\eslo0.ico");

            IconHelper.SetWindowIcon(hWnd, iconPath);
            aboutWindow.Show();
        }

        static string BuildArguments(string basePath)
        {
            string binPath = Path.Combine(basePath, "bin\\"), listsPath = Path.Combine(basePath, "lists\\");
            return
            "--wf-tcp=80,443 " +
            "--wf-udp=443,50000-50099 " +
            "--filter-tcp=80 " +
            "--hostlist=\"" + System.IO.Path.Combine(listsPath, "list-general.txt") + "\" " +
            "--dpi-desync=fake,fakedsplit " +
            "--dpi-desync-autottl=2 " +
            "--dpi-desync-fooling=md5sig " +
            "--new " +
            "--filter-tcp=80 " +
            "--ipset=\"" + System.IO.Path.Combine(listsPath, "ipset-all.txt") + "\" " +
            "--dpi-desync=fake,fakedsplit " +
            "--dpi-desync-autottl=2 " +
            "--dpi-desync-fooling=md5sig " +
            "--new " +
            "--filter-tcp=443 " +
            "--ipset=\"" + System.IO.Path.Combine(listsPath, "ipset-all.txt") + "\" " +
            "--dpi-desync=fake,multisplit " +
            "--dpi-desync-split-seqovl=681 " +
            "--dpi-desync-split-pos=1 " +
            "--dpi-desync-fooling=badseq " +
            "--dpi-desync-repeats=8 " +
            "--dpi-desync-split-seqovl-pattern=\"" + System.IO.Path.Combine(binPath, "tls_clienthello_www_google_com.bin") + "\" " +
            "--dpi-desync-fake-tls-mod=rnd,dupsid,sni=www.google.com " +
            "--new " +
            "--filter-tcp=443 " +
            "--hostlist=\"" + System.IO.Path.Combine(listsPath, "list-general.txt") + "\" " +
            "--dpi-desync=fake,multisplit " +
            "--dpi-desync-split-seqovl=681 " +
            "--dpi-desync-split-pos=1 " +
            "--dpi-desync-fooling=badseq " +
            "--dpi-desync-repeats=8 " +
            "--dpi-desync-split-seqovl-pattern=\"" + System.IO.Path.Combine(binPath, "tls_clienthello_www_google_com.bin") + "\" " +
            "--dpi-desync-fake-tls-mod=rnd,dupsid,sni=www.google.com " +
            "--new " +
            "--filter-udp=443 " +
            "--hostlist=\"" + System.IO.Path.Combine(listsPath, "list-general.txt") + "\" " +
            "--dpi-desync=fake " +
            "--dpi-desync-repeats=11 " +
            "--dpi-desync-fake-quic=\"" + System.IO.Path.Combine(binPath, "quic_initial_www_google_com.bin") + "\" " +
            "--new " +
            "--filter-udp=50000-50099 " +
            "--filter-l7=discord,stun " +
            "--dpi-desync=fake " +
            "--dpi-desync-repeats=6 " +
            "--new " +
            "--filter-udp=443 " +
            "--ipset=\"" + System.IO.Path.Combine(listsPath, "ipset-all.txt") + "\" " +
            "--dpi-desync=fake " +
            "--dpi-desync-repeats=11 " +
            "--dpi-desync-fake-quic=\"" + System.IO.Path.Combine(binPath, "quic_initial_www_google_com.bin") + "\" " +
            "--new";
        }
    }
}
