using System.Collections.Generic;
using Gtk;

namespace app.Views.Ui
{
    public abstract class FormDialog : Dialog
    {
        private readonly Grid form;
        private readonly Label errorLabel;
        private int row;

        protected FormDialog(string title, Window parent)
            : base(title, parent, DialogFlags.Modal)
        {
            SetDefaultSize(680, 640);
            DestroyWithParent = true;

            form = new Grid
            {
                ColumnSpacing = 12,
                RowSpacing = 10,
                BorderWidth = 14,
                ColumnHomogeneous = false
            };

            errorLabel = new Label
            {
                Xalign = 0,
                LineWrap = true,
                NoShowAll = true
            };

            errorLabel.StyleContext.AddClass("dialog-error");
            ContentArea.PackStart(errorLabel, false, false, 0);

            var scroll = new ScrolledWindow
            {
                ShadowType = ShadowType.None
            };

            scroll.SetPolicy(PolicyType.Never, PolicyType.Automatic);

            var viewport = new Viewport(null, null);
            viewport.Add(form);
            scroll.Add(viewport);
            ContentArea.PackStart(scroll, true, true, 0);

            AddButton("Otkazi", ResponseType.Cancel);
            AddButton("Sacuvaj", ResponseType.Ok);
        }

        public void ShowError(string message)
        {
            errorLabel.Text = message ?? string.Empty;
            errorLabel.Visible = !string.IsNullOrWhiteSpace(message);
        }

        public void ClearError()
        {
            ShowError(null);
        }

        protected void AddSection(string text)
        {
            var label = new Label
            {
                Markup = "<b>" + text + "</b>",
                Xalign = 0,
                MarginTop = row == 0 ? 0 : 12
            };

            form.Attach(label, 0, row, 2, 1);
            row++;
        }

        protected Entry AddEntry(string labelText, string placeholder = "")
        {
            var entry = new Entry
            {
                Hexpand = true,
                PlaceholderText = placeholder
            };

            AddRow(labelText, entry);
            return entry;
        }

        protected ComboBoxText AddCombo(string labelText, IEnumerable<string> values)
        {
            var combo = new ComboBoxText
            {
                Hexpand = true
            };

            foreach (var value in values)
            {
                combo.AppendText(value);
            }

            combo.Active = 0;
            AddRow(labelText, combo);
            return combo;
        }

        protected CheckButton AddCheck(string labelText)
        {
            var check = new CheckButton(labelText)
            {
                Hexpand = true
            };

            form.Attach(new Label(string.Empty), 0, row, 1, 1);
            form.Attach(check, 1, row, 1, 1);
            row++;
            return check;
        }

        protected SpinButton AddDecimal(string labelText)
        {
            var adjustment = new Adjustment(0, 0, 1000000000, 0.01, 1, 0);
            var spin = new SpinButton(adjustment, 0.01, 2)
            {
                Hexpand = true,
                Numeric = true
            };

            AddRow(labelText, spin);
            return spin;
        }

        protected TextView AddText(string labelText)
        {
            var text = new TextView
            {
                WrapMode = WrapMode.WordChar
            };

            var scroll = new ScrolledWindow
            {
                ShadowType = ShadowType.In,
                Hexpand = true
            };

            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scroll.SetSizeRequest(-1, 84);
            scroll.Add(text);
            AddRow(labelText, scroll);
            return text;
        }

        private void AddRow(string labelText, Widget editor)
        {
            var label = new Label(labelText)
            {
                Xalign = 0,
                Halign = Align.Start,
                Valign = Align.Center
            };

            form.Attach(label, 0, row, 1, 1);
            form.Attach(editor, 1, row, 1, 1);
            row++;
        }
    }
}
