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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

namespace Eleanor
{
    public sealed partial class AboutWindow : Window
    {
        private AppWindow a_AppWindow;

        private int aWidth = 300, aHeight = 400;

        public AboutWindow()
        {
            InitializeComponent();

            a_AppWindow = GetAppWindowForCurrentWindow();

            OverlappedPresenter _presenter = a_AppWindow.Presenter as OverlappedPresenter;

            // AppWindow
            a_AppWindow.SetIcon(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\eslo0.png");

            a_AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            a_AppWindow.Resize(new Windows.Graphics.SizeInt32(aWidth, aHeight));
            _presenter.IsResizable = false;
            _presenter.IsMaximizable = false;
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
    }
} 