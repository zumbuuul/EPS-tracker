using System;
using System.Linq;
using Gtk;

namespace app.Views.Ui
{
    public static class ViewFactory
    {
        public static Button ActionButton(string label, string iconName, string tooltip, EventHandler onClicked)
        {
            var button = new Button
            {
                TooltipText = tooltip
            };

            var content = new Box(Orientation.Horizontal, 6);
            content.PackStart(Image.NewFromIconName(iconName, IconSize.Button), false, false, 0);
            content.PackStart(new Label(label), false, false, 0);

            button.Add(content);

            if (onClicked != null)
            {
                button.Clicked += onClicked;
            }

            return button;
        }

        public static SearchEntry Search(string placeholder)
        {
            return new SearchEntry
            {
                PlaceholderText = placeholder,
                WidthRequest = 260
            };
        }

        public static ComboBoxText Combo(params string[] values)
        {
            var combo = new ComboBoxText();

            foreach (var value in values)
            {
                combo.AppendText(value);
            }

            if (values.Length > 0)
            {
                combo.Active = 0;
            }

            return combo;
        }

        public static Label PageTitle(string title)
        {
            var label = new Label(title)
            {
                Xalign = 0
            };

            label.StyleContext.AddClass("page-title");
            return label;
        }

        public static Label PageSubtitle(string subtitle)
        {
            var label = new Label(subtitle)
            {
                Xalign = 0
            };

            label.StyleContext.AddClass("page-subtitle");
            return label;
        }

        public static TreeView Table(params string[] columns)
        {
            var store = new ListStore(columns.Select(_ => typeof(string)).ToArray());
            var table = new TreeView(store)
            {
                HeadersVisible = true,
                EnableSearch = true
            };

            for (var index = 0; index < columns.Length; index++)
            {
                var renderer = new CellRendererText
                {
                    Ellipsize = Pango.EllipsizeMode.End
                };

                var column = new TreeViewColumn
                {
                    Title = columns[index],
                    Resizable = true,
                    Sizing = TreeViewColumnSizing.Autosize
                };

                column.PackStart(renderer, true);
                column.AddAttribute(renderer, "text", index);
                table.AppendColumn(column);
            }

            return table;
        }

        public static ScrolledWindow Scroll(Widget child)
        {
            var scroll = new ScrolledWindow
            {
                ShadowType = ShadowType.In
            };

            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scroll.Add(child);
            return scroll;
        }
    }
}
