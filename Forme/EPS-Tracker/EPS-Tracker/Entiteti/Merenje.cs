using System;
using System.Collections.Generic;
using System.Text;

namespace EPS_Tracker.Entiteti;

public class Merenje
{
    public virtual long Id { get; protected set; }

    public virtual Brojilo Brojilo { get; set; } = null!;

    public virtual DateTime DatumVremeMerenja { get; set; }

    public virtual decimal? PotrosnjaAktivna { get; set; }

    public virtual decimal? PotrosnjaReaktivna { get; set; }

    public virtual decimal? Snaga { get; set; }

    public virtual decimal? Napon { get; set; }

    public virtual decimal? Struja { get; set; }

    public virtual string? TipMerenja { get; set; }

    public virtual string? TipIzvora { get; set; }

    public virtual string IsValidirano { get; set; } = "N";

    public virtual string? Komentar { get; set; }
}