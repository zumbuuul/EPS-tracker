using Gtk;
using app.Services;
using app.Views.Pages;

namespace app.Views
{
    public class MainWindow : Window
    {
        private readonly Label statusLabel;

        public MainWindow(PotrosacService potrosacService, BrojiloService brojiloService)
            : base("EPS Tracker")
        {
            SetDefaultSize(1280, 760);
            SetPosition(WindowPosition.Center);

            DeleteEvent += (sender, args) => Application.Quit();

            var root = new Box(Orientation.Vertical, 0);
            Add(root);

            root.PackStart(BuildTopbar(), false, false, 0);

            statusLabel = new Label("Spremno")
            {
                Xalign = 0
            };

            var notebook = new Notebook
            {
                Scrollable = true,
                BorderWidth = 0
            };

            notebook.AppendPage(new PotrosaciPage(potrosacService, SetStatus), new Label("Potrosaci"));
            notebook.AppendPage(new BrojilaPage(brojiloService, SetStatus), new Label("Brojila"));
            notebook.AppendPage(new MerenjaPage(SetStatus), new Label("Merenja"));
            notebook.AppendPage(new RacuniPage(SetStatus), new Label("Racuni"));
            notebook.AppendPage(new KvaroviPage(SetStatus), new Label("Kvarovi"));
            notebook.AppendPage(new StanjeMrezePage(SetStatus), new Label("Stanje mreze"));

            root.PackStart(notebook, true, true, 0);

            var statusbar = new Box(Orientation.Horizontal, 0);
            statusbar.StyleContext.AddClass("statusbar");
            statusbar.PackStart(statusLabel, true, true, 0);
            root.PackEnd(statusbar, false, false, 0);

            ShowAll();
        }

        private Widget BuildTopbar()
        {
            var topbar = new Box(Orientation.Vertical, 2);
            topbar.StyleContext.AddClass("topbar");

            var title = new Label("EPS Tracker")
            {
                Xalign = 0
            };
            title.StyleContext.AddClass("app-title");

            var subtitle = new Label("Evidencija i upravljanje potrosnjom elektricne energije")
            {
                Xalign = 0
            };
            subtitle.StyleContext.AddClass("app-subtitle");

            topbar.PackStart(title, false, false, 0);
            topbar.PackStart(subtitle, false, false, 0);
            return topbar;
        }

        private void SetStatus(string message)
        {
            statusLabel.Text = message;
        }
    }
}
