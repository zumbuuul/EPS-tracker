using System;
using Gtk;
using app.Views;
using app.Views.Ui;

namespace app
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            AppTheme.Apply();

            var gtkApp = new Application("org.eps.tracker", GLib.ApplicationFlags.None);
            gtkApp.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            gtkApp.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
