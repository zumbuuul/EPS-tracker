using System;
using System.Collections.Generic;
using System.Text;

namespace EPS_Tracker.Entiteti;

public class Brojilo
{
    public virtual string SerijskiBroj { get; set; } = string.Empty;

    public virtual DateTime DatumInstalacije { get; set; }

    public virtual string Status { get; set; } = string.Empty;

    public virtual string? Lokacija { get; set; }

    public virtual decimal? KoeficijentMnozenja { get; set; }

    public virtual string? Komentar { get; set; }

    public virtual IList<Potrosac> Potrosaci { get; set; } = new List<Potrosac>();

    public virtual IList<Merenje> Merenja { get; set; } = new List<Merenje>();
}