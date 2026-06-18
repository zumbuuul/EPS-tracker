using Gtk;

namespace app.Views.Ui
{
    public static class AppTheme
    {
        public static void Apply()
        {
            var css = @"
window {
    background: #f5f6f4;
}

.topbar {
    background: #1f2933;
    color: #f7f8f3;
    padding: 12px 16px;
}

.app-title {
    font-size: 20px;
    font-weight: 700;
}

.app-subtitle {
    color: #d6ddd3;
}

.page-title {
    font-size: 18px;
    font-weight: 700;
    color: #20252b;
}

.page-subtitle {
    color: #5d6872;
}

.statusbar {
    background: #ecefed;
    border-top: 1px solid #d1d6d3;
    padding: 6px 12px;
}

.dialog-error {
    background: #fde8e8;
    color: #7f1d1d;
    padding: 8px 12px;
}

button {
    min-height: 30px;
    padding: 4px 10px;
    border-radius: 6px;
}

entry, combobox, spinbutton {
    min-height: 30px;
}

treeview {
    background-color: #ffffff;
    color: #1f2933;
}

treeview.view {
    background-color: #ffffff;
    color: #1f2933;
}

treeview.view:selected {
    background-color: #2f6fca;
    color: #ffffff;
}

treeview.view header button {
    background: #5c6268;
    color: #ffffff;
}
";

            var provider = new CssProvider();
            provider.LoadFromData(css);
            StyleContext.AddProviderForScreen(Gdk.Screen.Default, provider, 800);
        }
    }
}
