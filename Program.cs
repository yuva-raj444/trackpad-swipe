using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

class Program
{
    private const int WH_MOUSE_LL = 14;
    private const int WM_MOUSEHWHEEL = 0x020E;
    private const int THRESHOLD = 30;

    private static LowLevelMouseProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    const int SW_HIDE = 0;
    const int KEYEVENTF_KEYUP = 0x0002;
    const byte VK_MENU = 0x12;
    const byte VK_LEFT = 0x25;
    const byte VK_RIGHT = 0x27;

    [STAThread]
    static void Main()
    {
        var handle = GetConsoleWindow();
       // ShowWindow(handle, SW_HIDE);

        Console.WriteLine("Swipe listener running...");
        _hookID = SetHook(_proc);
        Application.Run(); // Keeps app alive
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using Process curProcess = Process.GetCurrentProcess();
        using ProcessModule curModule = curProcess.MainModule!;
        return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEHWHEEL)
        {
            var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            int wheelDelta = (short)((hookStruct.mouseData >> 16) & 0xffff);

            if (wheelDelta >= THRESHOLD)
            {
                Console.WriteLine("Right Swipe → Alt+Right");
                SimulateAltArrowKey(VK_RIGHT);
            }
            else if (wheelDelta <= -THRESHOLD)
            {
                Console.WriteLine("Left Swipe ← Alt+Left");
                SimulateAltArrowKey(VK_LEFT);
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static void SimulateAltArrowKey(byte arrowKey)
    {
        IntPtr fgWindow = GetForegroundWindow();
        if (fgWindow != IntPtr.Zero)
        {
            SetForegroundWindow(fgWindow);
            Thread.Sleep(50); // Ensure focus
        }

        keybd_event(VK_MENU, 0, 0, UIntPtr.Zero);              // Alt down
        keybd_event(arrowKey, 0, 0, UIntPtr.Zero);             // Arrow down
        keybd_event(arrowKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Arrow up
        keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);  // Alt up
    }

    #region WinAPI Structs and Consts                       

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x, y;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr GetModuleHandle(string lpModuleName);

    #endregion
}
