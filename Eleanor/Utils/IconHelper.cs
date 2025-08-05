using System;
using System.IO;
using System.Runtime.InteropServices;
using WinRT.Interop;

public static class IconHelper
{
    private const int WM_SETICON = 0x80;
    private const int ICON_SMALL = 0;
    private const int ICON_BIG = 1;

    private const uint IMAGE_ICON = 1;
    private const uint LR_LOADFROMFILE = 0x00000010;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr LoadImage(IntPtr hInst, string lpszName,
        uint uType, int cxDesired, int cyDesired, uint fuLoad);

    public static void SetWindowIcon(IntPtr hWnd, string iconFilePath, int iconWidth = 256, int iconHeight = 256)
    {
        if (!File.Exists(iconFilePath))
        {
            System.Diagnostics.Debug.WriteLine($"Icon file not found: {iconFilePath}");
            return;
        }

        IntPtr hIcon = LoadImage(IntPtr.Zero, iconFilePath, IMAGE_ICON, iconWidth, iconHeight, LR_LOADFROMFILE);
        if (hIcon == IntPtr.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            System.Diagnostics.Debug.WriteLine($"Failed to load icon: {error}");
            return;
        }

        SendMessage(hWnd, WM_SETICON, ICON_BIG, hIcon);
        SendMessage(hWnd, WM_SETICON, ICON_SMALL, hIcon);
    }

}