using System;
using Gtk;
using app.Views.Ui;

namespace app.Views.Pages
{
    public abstract class ListPage : Box
    {
        private readonly Action<string> showStatus;

        protected ListPage(string title, string subtitle, Action<string> showStatus)
            : base(Orientation.Vertical, 10)
        {
            this.showStatus = showStatus ?? (_ => { });
            BorderWidth = 14;

            var header = new Box(Orientation.Horizontal, 12);
            var text = new Box(Orientation.Vertical, 2);
            text.PackStart(ViewFactory.PageTitle(title), false, false, 0);
            text.PackStart(ViewFactory.PageSubtitle(subtitle), false, false, 0);

            Toolbar = new Box(Orientation.Horizontal, 6)
            {
                Halign = Align.End
            };

            header.PackStart(text, true, true, 0);
            header.PackEnd(Toolbar, false, false, 0);

            Filters = new Box(Orientation.Horizontal, 8);

            PackStart(header, false, false, 0);
            PackStart(Filters, false, false, 0);
        }

        protected Box Filters { get; }

        protected Box Toolbar { get; }

        protected TreeView Table { get; private set; }

        protected Window DialogParent => Toplevel as Window;

        protected void AddAction(string label, string iconName, string tooltip, EventHandler onClicked)
        {
            Toolbar.PackStart(ViewFactory.ActionButton(label, iconName, tooltip, onClicked), false, false, 0);
        }

        protected void AddFilter(Widget widget)
        {
            Filters.PackStart(widget, false, false, 0);
        }

        protected void SetTable(params string[] columns)
        {
            Table = ViewFactory.Table(columns);
            PackStart(ViewFactory.Scroll(Table), true, true, 0);
        }

        protected void OpenDialog(FormDialog dialog, string statusAfterOk)
        {
            dialog.ShowAll();
            var response = (ResponseType)dialog.Run();
            dialog.Destroy();

            if (response == ResponseType.Ok)
            {
                Report(statusAfterOk);
            }
        }

        protected void Report(string message)
        {
            showStatus(message);
        }
    }
}
