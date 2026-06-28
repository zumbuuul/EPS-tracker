using System;
using System.Collections.Generic;
using System.Linq;
using app.DTO;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class PotrosacBrojiloLinkDialog : FormDialog
    {
        private readonly long potrosacId;
        private readonly IList<BrojiloListDto> brojila;
        private readonly Entry serijskiBrojEntry;
        private readonly ComboBoxText brojiloCombo;

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

        public PotrosacBrojiloLinkDialog(
            Window parent,
            long potrosacId,
            IList<BrojiloListDto> brojila)
            : base("Veza potrosaca i brojila", parent)
        {
            this.potrosacId = potrosacId;
            this.brojila = brojila ?? new List<BrojiloListDto>();

            AddSection("Veza");

            var potrosacIdEntry = AddEntry("Potrosac ID");
            potrosacIdEntry.Text = potrosacId.ToString();
            potrosacIdEntry.Sensitive = false;

            brojiloCombo = AddCombo("Brojilo", this.brojila.Select(BuildDisplayText));
        }

        public PotrosacBrojiloLinkDto ToDto()
        {
            return new PotrosacBrojiloLinkDto
            {
                PotrosacId = potrosacId,
                SerijskiBroj = brojiloCombo != null
                    ? SelectedBrojiloSerijskiBroj()
                    : serijskiBrojEntry.Text
            };
        }

        private string SelectedBrojiloSerijskiBroj()
        {
            if (brojiloCombo.Active < 0 || brojiloCombo.Active >= brojila.Count)
            {
                throw new ArgumentException("Izaberi brojilo.");
            }

            return brojila[brojiloCombo.Active].SerijskiBroj;
        }

        private static string BuildDisplayText(BrojiloListDto brojilo)
        {
            if (brojilo == null)
            {
                return string.Empty;
            }

            var tipovi = brojilo.TipoviBrojila == null
                ? string.Empty
                : string.Join(", ", brojilo.TipoviBrojila);

            return brojilo.SerijskiBroj + " - " + tipovi + " - " + (brojilo.Lokacija ?? string.Empty);
        }
    }
}
