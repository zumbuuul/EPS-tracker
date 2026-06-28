using System;
using System.IO;
using Gtk;
using app.Persistence;
using app.Services;
using app.Views;
using app.Views.Ui;

namespace app
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            NHibernateSessionFactoryProvider sessionFactoryProvider = null;
            LoadDotEnv();

            Application.Init();
            AppTheme.Apply();

            var gtkApp = new Application("org.eps.tracker", GLib.ApplicationFlags.None);
            gtkApp.Register(GLib.Cancellable.Current);

            try
            {
                var connectionString =
                    Environment.GetEnvironmentVariable("EPS_TRACKER_ORACLE_CONNECTION_STRING")
                    ?? Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");

                PotrosacService potrosacService = null;
                BrojiloService brojiloService = null;
                MerenjeService merenjeService = null;
                RacunService racunService = null;
                KvarService kvarService = null;
                StanjeService stanjeService = null;

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    sessionFactoryProvider = new NHibernateSessionFactoryProvider(
                        new OraclePersistenceOptions(connectionString));

                    potrosacService = new PotrosacService(sessionFactoryProvider);
                    brojiloService = new BrojiloService(sessionFactoryProvider);
                    merenjeService = new MerenjeService(sessionFactoryProvider);
                    racunService = new RacunService(sessionFactoryProvider);
                    kvarService = new KvarService(sessionFactoryProvider);
                    stanjeService = new StanjeService(sessionFactoryProvider);
                }

                var win = new MainWindow(
                    potrosacService,
                    brojiloService,
                    merenjeService,
                    racunService,
                    kvarService,
                    stanjeService);
                gtkApp.AddWindow(win);

                win.Show();
                Application.Run();
            }
            finally
            {
                if (sessionFactoryProvider != null)
                {
                    sessionFactoryProvider.Dispose();
                }
            }
        }

        private static void LoadDotEnv()
        {
            var dotenvPath = FindDotEnvPath();

            if (dotenvPath == null)
            {
                return;
            }

            foreach (var rawLine in File.ReadAllLines(dotenvPath))
            {
                var line = rawLine.Trim();

                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=');

                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line.Substring(0, separatorIndex).Trim();
                var value = line.Substring(separatorIndex + 1).Trim();

                if (value.Length >= 2
                    && ((value[0] == '"' && value[value.Length - 1] == '"')
                        || (value[0] == '\'' && value[value.Length - 1] == '\'')))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                if (string.IsNullOrWhiteSpace(key)
                    || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                {
                    continue;
                }

                Environment.SetEnvironmentVariable(key, value);
            }
        }

        private static string FindDotEnvPath()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (directory != null)
            {
                var path = Path.Combine(directory.FullName, ".env");

                if (File.Exists(path))
                {
                    return path;
                }

                directory = directory.Parent;
            }

            return null;
        }
    }
}
