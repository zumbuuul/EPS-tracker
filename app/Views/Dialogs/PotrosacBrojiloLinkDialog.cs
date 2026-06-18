using app.DTO;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class PotrosacBrojiloLinkDialog : FormDialog
    {
        private readonly long potrosacId;
        private readonly Entry serijskiBrojEntry;

        public PotrosacBrojiloLinkDialog(Window parent, long potrosacId)
            : base("Veza potrosaca i brojila", parent)
        {
            this.potrosacId = potrosacId;

            AddSection("Veza");

            var potrosacIdEntry = AddEntry("Potrosac ID");
            potrosacIdEntry.Text = potrosacId.ToString();
            potrosacIdEntry.Sensitive = false;

            serijskiBrojEntry = AddEntry("Serijski broj brojila");
        }

        public PotrosacBrojiloLinkDto ToDto()
        {
            return new PotrosacBrojiloLinkDto
            {
                PotrosacId = potrosacId,
                SerijskiBroj = serijskiBrojEntry.Text
            };
        }
    }
}
