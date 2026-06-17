using EPS_Tracker.Entiteti;
using NHibernate;
using NHibernate.Linq;
using static EPS_Tracker.PotrosacPregled;

namespace EPS_Tracker;

public class DTOManager
{
    public static List<PotrosacPregled> VratiPotrosace()
    {
        List<PotrosacPregled> lista = new();
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                var potrosaci = session.Query<Potrosac>().ToList();

                foreach (var p in potrosaci)
                {
                    lista.Add(new PotrosacPregled(
                        p.Id,
                        p.Tip,
                        p.Email ?? "",
                        p.Telefon ?? "",
                        p.Adresa,
                        p.Grad,
                        p.Status
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return lista;
    }
    public static List<BrojiloPregled> VratiBrojila()
    {
        List<BrojiloPregled> lista = new();
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                var brojila = session.Query<Brojilo>().ToList();

                foreach (var b in brojila)
                {
                    lista.Add(new BrojiloPregled(
                        b.SerijskiBroj,
                        b.DatumInstalacije,
                        b.Status,
                        b.Lokacija ?? "",
                        b.KoeficijentMnozenja
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return lista;
    }

    public static bool DodajPotrosaca(PotrosacBasic pb)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Potrosac p = new Potrosac
                {
                    Tip = pb.Tip,
                    Email = pb.Email,
                    Telefon = pb.Telefon,
                    Adresa = pb.Adresa,
                    Grad = pb.Grad,
                    Status = pb.Status,
                    KategorijaTarife = pb.KategorijaTarife
                };

                session.Save(p);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static bool ObrisiPotrosaca(long id)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Potrosac p = session.Load<Potrosac>(id);

                session.Delete(p);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static PotrosacBasic VratiPotrosacaBasic(long id)
    {
        PotrosacBasic pb = new();
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Potrosac p = session.Load<Potrosac>(id);

                pb = new PotrosacBasic
                {
                    Id = p.Id,
                    Tip = p.Tip,
                    Email = p.Email,
                    Telefon = p.Telefon,
                    Adresa = p.Adresa,
                    Grad = p.Grad,
                    Status = p.Status,
                    KategorijaTarife = p.KategorijaTarife
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return pb;
    }

    public static bool IzmeniPotrosaca(PotrosacBasic pb)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Potrosac p = session.Load<Potrosac>(pb.Id);

                p.Tip = pb.Tip;
                p.Email = pb.Email;
                p.Telefon = pb.Telefon;
                p.Adresa = pb.Adresa;
                p.Grad = pb.Grad;
                p.Status = pb.Status;
                p.KategorijaTarife = pb.KategorijaTarife;

                session.Update(p);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static bool DodajBrojilo(BrojiloBasic bb)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Brojilo b = new Brojilo
                {
                    SerijskiBroj = bb.SerijskiBroj,
                    DatumInstalacije = bb.DatumInstalacije,
                    Status = bb.Status,
                    Lokacija = bb.Lokacija,
                    KoeficijentMnozenja = bb.KoeficijentMnozenja,
                    Komentar = bb.Komentar
                };

                session.Save(b);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static bool ObrisiBrojilo(string serijskiBroj)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Brojilo b = session.Load<Brojilo>(serijskiBroj);

                session.Delete(b);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static BrojiloBasic VratiBrojiloBasic(string serijskiBroj)
    {
        BrojiloBasic bb = new();
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Brojilo b = session.Load<Brojilo>(serijskiBroj);

                bb = new BrojiloBasic
                {
                    SerijskiBroj = b.SerijskiBroj,
                    DatumInstalacije = b.DatumInstalacije,
                    Status = b.Status,
                    Lokacija = b.Lokacija,
                    KoeficijentMnozenja = b.KoeficijentMnozenja,
                    Komentar = b.Komentar
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return bb;
    }

    public static bool IzmeniBrojilo(BrojiloBasic bb)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Brojilo b = session.Load<Brojilo>(bb.SerijskiBroj);

                b.DatumInstalacije = bb.DatumInstalacije;
                b.Status = bb.Status;
                b.Lokacija = bb.Lokacija;
                b.KoeficijentMnozenja = bb.KoeficijentMnozenja;
                b.Komentar = bb.Komentar;

                session.Update(b);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static List<BrojiloPregled> VratiBrojilaZaPotrosaca(long potrosacId)
    {
        List<BrojiloPregled> lista = new();
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Potrosac p = session.Load<Potrosac>(potrosacId);

                foreach (var b in p.Brojila)
                {
                    lista.Add(new BrojiloPregled(
                        b.SerijskiBroj,
                        b.DatumInstalacije,
                        b.Status,
                        b.Lokacija ?? "",
                        b.KoeficijentMnozenja
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return lista;
    }

    public static bool PoveziPotrosacaIBrojilo(long potrosacId, string serijskiBroj)
    {
        ISession? session = null;
        ITransaction? transaction = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                transaction = session.BeginTransaction();

                Potrosac p = session.Get<Potrosac>(potrosacId);
                Brojilo b = session.Get<Brojilo>(serijskiBroj);

                if (p == null)
                {
                    MessageBox.Show("Potrošač nije pronađen.");
                    return false;
                }

                if (b == null)
                {
                    MessageBox.Show("Brojilo nije pronađeno.");
                    return false;
                }

                if (!p.Brojila.Any(x => x.SerijskiBroj == serijskiBroj))
                {
                    p.Brojila.Add(b);
                    b.Potrosaci.Add(p);
                }

                session.Update(p);
                session.Update(b);

                transaction.Commit();

                return true;
            }
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static bool RaskiniVezuPotrosacBrojilo(long potrosacId, string serijskiBroj)
    {
        ISession? session = null;
        ITransaction? transaction = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                transaction = session.BeginTransaction();

                Potrosac p = session.Get<Potrosac>(potrosacId);
                Brojilo b = session.Get<Brojilo>(serijskiBroj);

                if (p == null)
                {
                    MessageBox.Show("Potrošač nije pronađen.");
                    return false;
                }

                if (b == null)
                {
                    MessageBox.Show("Brojilo nije pronađeno.");
                    return false;
                }

                Brojilo? brojiloZaBrisanje = p.Brojila.FirstOrDefault(x => x.SerijskiBroj == serijskiBroj);
                Potrosac? potrosacZaBrisanje = b.Potrosaci.FirstOrDefault(x => x.Id == potrosacId);

                if (brojiloZaBrisanje == null)
                {
                    MessageBox.Show("Ovaj potrošač nije povezan sa tim brojilom.");
                    return false;
                }

                p.Brojila.Remove(brojiloZaBrisanje);

                if (potrosacZaBrisanje != null)
                {
                    b.Potrosaci.Remove(potrosacZaBrisanje);
                }

                session.Update(p);
                session.Update(b);

                transaction.Commit();

                return true;
            }
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static List<MerenjePregled> VratiMerenjaZaBrojilo(string serijskiBroj)
    {
        List<MerenjePregled> lista = new();
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                var merenja = session.Query<Merenje>()
                    .Where(m => m.Brojilo.SerijskiBroj == serijskiBroj)
                    .ToList();

                foreach (var m in merenja)
                {
                    lista.Add(new MerenjePregled(
                        m.Id,
                        m.Brojilo.SerijskiBroj,
                        m.DatumVremeMerenja,
                        m.PotrosnjaAktivna,
                        m.Snaga,
                        m.Napon,
                        m.IsValidirano
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return lista;
    }

    public static bool DodajMerenje(MerenjeBasic mb)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Brojilo b = session.Load<Brojilo>(mb.SerijskiBroj);

                Merenje m = new Merenje
                {
                    Brojilo = b,
                    DatumVremeMerenja = mb.DatumVremeMerenja,
                    PotrosnjaAktivna = mb.PotrosnjaAktivna,
                    PotrosnjaReaktivna = mb.PotrosnjaReaktivna,
                    Snaga = mb.Snaga,
                    Napon = mb.Napon,
                    Struja = mb.Struja,
                    TipMerenja = mb.TipMerenja,
                    TipIzvora = mb.TipIzvora,
                    IsValidirano = mb.IsValidirano,
                    Komentar = mb.Komentar
                };

                session.Save(m);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static bool ObrisiMerenje(long id)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Merenje m = session.Load<Merenje>(id);

                session.Delete(m);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }

    public static MerenjeBasic VratiMerenjeBasic(long id)
    {
        MerenjeBasic mb = new();
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Merenje m = session.Load<Merenje>(id);

                mb = new MerenjeBasic
                {
                    Id = m.Id,
                    SerijskiBroj = m.Brojilo.SerijskiBroj,
                    DatumVremeMerenja = m.DatumVremeMerenja,
                    PotrosnjaAktivna = m.PotrosnjaAktivna,
                    PotrosnjaReaktivna = m.PotrosnjaReaktivna,
                    Snaga = m.Snaga,
                    Napon = m.Napon,
                    Struja = m.Struja,
                    TipMerenja = m.TipMerenja,
                    TipIzvora = m.TipIzvora,
                    IsValidirano = m.IsValidirano,
                    Komentar = m.Komentar
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return mb;
    }

    public static bool IzmeniMerenje(MerenjeBasic mb)
    {
        ISession? session = null;

        try
        {
            session = DataLayer.GetSession();

            if (session != null)
            {
                Merenje m = session.Load<Merenje>(mb.Id);

                m.DatumVremeMerenja = mb.DatumVremeMerenja;
                m.PotrosnjaAktivna = mb.PotrosnjaAktivna;
                m.PotrosnjaReaktivna = mb.PotrosnjaReaktivna;
                m.Snaga = mb.Snaga;
                m.Napon = mb.Napon;
                m.Struja = mb.Struja;
                m.TipMerenja = mb.TipMerenja;
                m.TipIzvora = mb.TipIzvora;
                m.IsValidirano = mb.IsValidirano;
                m.Komentar = mb.Komentar;

                session.Update(m);
                session.Flush();

                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            session?.Close();
        }

        return false;
    }
}