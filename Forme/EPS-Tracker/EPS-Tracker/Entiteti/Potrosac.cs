using System;
using System.Collections.Generic;
using System.Text;

namespace EPS_Tracker.Entiteti;

public class Potrosac
{
    public virtual long Id { get; protected set; }

    public virtual string Tip { get; set; } = string.Empty;

    public virtual string? Email { get; set; }

    public virtual string? Telefon { get; set; }

    public virtual string Adresa { get; set; } = string.Empty;

    public virtual string Grad { get; set; } = string.Empty;

    public virtual string? Komentar { get; set; }

    public virtual string Status { get; set; } = string.Empty;

    public virtual string KategorijaTarife { get; set; } = string.Empty;

    public virtual IList<Brojilo> Brojila { get; set; } = new List<Brojilo>();

}