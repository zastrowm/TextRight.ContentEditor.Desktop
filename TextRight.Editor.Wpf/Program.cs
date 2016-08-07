using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using TextRight.ContentEditor.Desktop.App;

namespace TextRight.Editor.Wpf
{
  public class Program
  {
    [STAThread]
    public static void Main(string[] args)
    {
      var application = new Application();
      application.Startup += delegate
                             {
                               var window = new MainWindow();
                               application.MainWindow = window;

                               // dotnet watch will auto-restart the app when it detects a file change.  Unfortunately, the
                               // window that will be started takes focus (and appears right over Visual-Studio).  So, if
                               // compiled with WATCHING defined, we position the window on the third monitor and give focus
                               // back to Visual-Studio on startup. 

#if WATCHING
                               window.Left = 1920 + 50;
                               window.Top = 20;
                               window.ShowActivated = false;
                               window.WindowStartupLocation = WindowStartupLocation.Manual;
#endif

                               window.Show();
                               Focus_VisualStudio();
                             };
      application.Run();
    }

    [Conditional("WATCHING")]
    private static void Focus_VisualStudio()
    {
      var vsProcess = Process.GetProcessesByName("devenv").FirstOrDefault();
      if (vsProcess == null)
        return;

      SetForegroundWindow(vsProcess.MainWindowHandle);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
  }
}