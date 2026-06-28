using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.DTO;
using app.Entities;
using app.Entities.Enums;
using app.Persistence;
using NHibernate;
using NHibernate.Linq;

namespace app.Services
{
    public class BrojiloService
    {
        private readonly NHibernateSessionFactoryProvider sessionFactoryProvider;

        public BrojiloService(NHibernateSessionFactoryProvider sessionFactoryProvider)
        {
            this.sessionFactoryProvider = sessionFactoryProvider
                ?? throw new ArgumentNullException(nameof(sessionFactoryProvider));
        }

        public async Task<IList<BrojiloListDto>> VratiBrojila()
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var brojila = await session.Query<Brojilo>()
                    .OrderBy(x => x.SerijskiBroj)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return brojila.Select(MapToListDto).ToList();
            }
        }

        public async Task<BrojiloDto> VratiBrojilo(string serijskiBroj)
        {
            ValidateSerijskiBroj(serijskiBroj);

            using (var session = sessionFactoryProvider.OpenSession())
            {
                var brojilo = await GetRequiredBrojilo(session, serijskiBroj).ConfigureAwait(false);
                return MapToDto(brojilo);
            }
        }

        public async Task<IList<MerenjeListDto>> VratiMerenjaZaBrojilo(string serijskiBroj)
        {
            ValidateSerijskiBroj(serijskiBroj);

            using (var session = sessionFactoryProvider.OpenSession())
            {
                var brojilo = await GetRequiredBrojilo(session, serijskiBroj).ConfigureAwait(false);

                return brojilo.Merenja
                    .OrderByDescending(x => x.DatumVremeMerenja)
                    .Select(MapToMerenjeListDto)
                    .ToList();
            }
        }

        public async Task<IList<KvarListDto>> VratiKvaroveZaBrojilo(string serijskiBroj)
        {
            ValidateSerijskiBroj(serijskiBroj);

            using (var session = sessionFactoryProvider.OpenSession())
            {
                var brojilo = await GetRequiredBrojilo(session, serijskiBroj).ConfigureAwait(false);

                return brojilo.Kvarovi
                    .OrderByDescending(x => x.DatumPrijave)
                    .Select(MapToKvarListDto)
                    .ToList();
            }
        }

        public async Task DodajBrojilo(BrojiloSaveDto dto)
        {
            var tipovi = ValidateSaveDto(dto);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var serijskiBroj = dto.SerijskiBroj.Trim();
                var existing = await session.GetAsync<Brojilo>(serijskiBroj).ConfigureAwait(false);

                if (existing != null)
                {
                    throw new InvalidOperationException("Brojilo sa tim serijskim brojem vec postoji.");
                }

                var brojilo = new Brojilo();
                ApplyScalarFields(brojilo, dto, tipovi);
                await ApplySubtypeDetails(session, brojilo, dto, tipovi).ConfigureAwait(false);

                await session.SaveAsync(brojilo).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task IzmeniBrojilo(BrojiloSaveDto dto)
        {
            var tipovi = ValidateSaveDto(dto);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var brojilo = await GetRequiredBrojilo(session, dto.SerijskiBroj).ConfigureAwait(false);

                ApplyScalarFields(brojilo, dto, tipovi);
                await ApplySubtypeDetails(session, brojilo, dto, tipovi).ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task ObrisiBrojilo(string serijskiBroj)
        {
            ValidateSerijskiBroj(serijskiBroj);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var brojilo = await GetRequiredBrojilo(session, serijskiBroj).ConfigureAwait(false);

                foreach (var veza in brojilo.VezePotrosaca.ToList())
                {
                    await session.DeleteAsync(veza).ConfigureAwait(false);
                }

                foreach (var kvar in brojilo.Kvarovi.ToList())
                {
                    await session.DeleteAsync(kvar).ConfigureAwait(false);
                }

                foreach (var merenje in brojilo.Merenja.ToList())
                {
                    var racuni = await session.Query<Racun>()
                        .Where(x => x.Merenje.Id == merenje.Id)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    foreach (var racun in racuni)
                    {
                        await session.DeleteAsync(racun).ConfigureAwait(false);
                    }

                    await session.DeleteAsync(merenje).ConfigureAwait(false);
                }

                await RemoveMehanicko(session, brojilo).ConfigureAwait(false);
                await RemovePametno(session, brojilo).ConfigureAwait(false);
                await RemoveTrofazno(session, brojilo).ConfigureAwait(false);
                await RemoveRasvetno(session, brojilo).ConfigureAwait(false);

                await session.DeleteAsync(brojilo).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        private static void ApplyScalarFields(
            Brojilo brojilo,
            BrojiloSaveDto dto,
            IList<TipBrojila> tipovi)
        {
            brojilo.SerijskiBroj = dto.SerijskiBroj.Trim();
            brojilo.DatumInstalacije = dto.DatumInstalacije;
            brojilo.Status = dto.Status.Trim();
            brojilo.Lokacija = Normalize(dto.Lokacija);
            brojilo.KoeficijentMnozenja = dto.KoeficijentMnozenja;
            brojilo.Komentar = Normalize(dto.Komentar);

            brojilo.TipoviBrojila.Clear();
            foreach (var tip in tipovi)
            {
                brojilo.TipoviBrojila.Add(tip);
            }

            brojilo.DatumiZamene.Clear();
            foreach (var datumZamene in dto.DatumiZamene)
            {
                brojilo.DatumiZamene.Add(datumZamene);
            }
        }

        private static async Task ApplySubtypeDetails(
            ISession session,
            Brojilo brojilo,
            BrojiloSaveDto dto,
            IList<TipBrojila> tipovi)
        {
            if (tipovi.Contains(TipBrojila.MEHANICKO))
            {
                ApplyMehanicko(brojilo, dto.Mehanicko);
            }
            else
            {
                await RemoveMehanicko(session, brojilo).ConfigureAwait(false);
            }

            if (tipovi.Contains(TipBrojila.PAMETNO))
            {
                ApplyPametno(brojilo, dto.Pametno);
            }
            else
            {
                await RemovePametno(session, brojilo).ConfigureAwait(false);
            }

            if (tipovi.Contains(TipBrojila.TROFAZNO))
            {
                ApplyTrofazno(brojilo, dto.Trofazno);
            }
            else
            {
                await RemoveTrofazno(session, brojilo).ConfigureAwait(false);
            }

            if (tipovi.Contains(TipBrojila.RASVETNO))
            {
                ApplyRasvetno(brojilo, dto.Rasvetno);
            }
            else
            {
                await RemoveRasvetno(session, brojilo).ConfigureAwait(false);
            }
        }

        private static void ApplyMehanicko(Brojilo brojilo, MehanickoBrojiloDto dto)
        {
            if (brojilo.Mehanicko == null)
            {
                brojilo.Mehanicko = new MehanickoBrojilo
                {
                    Brojilo = brojilo
                };
            }

            brojilo.Mehanicko.PoslednjaKalibracija = dto.PoslednjaKalibracija;
            brojilo.Mehanicko.Preciznost = Normalize(dto.Preciznost);
            brojilo.Mehanicko.MaxGreska = dto.MaxGreska;
        }

        private static void ApplyPametno(Brojilo brojilo, PametnoBrojiloDto dto)
        {
            if (brojilo.Pametno == null)
            {
                brojilo.Pametno = new PametnoBrojilo
                {
                    Brojilo = brojilo
                };
            }

            brojilo.Pametno.Protokol = ParseOptionalEnum<PametnoBrojiloProtokol>(dto.Protokol, "Protokol pametnog brojila nije ispravan.");
            brojilo.Pametno.Frekvencija = dto.Frekvencija;
            brojilo.Pametno.IsDaljinskoIskljucenje = ParseRequiredEnum<DaNe>(dto.IsDaljinskoIskljucenje, "Daljinsko iskljucenje je obavezno.");
            brojilo.Pametno.NivoBaterije = dto.NivoBaterije;
        }

        private static void ApplyTrofazno(Brojilo brojilo, TrofaznoBrojiloDto dto)
        {
            if (brojilo.Trofazno == null)
            {
                brojilo.Trofazno = new TrofaznoBrojilo
                {
                    Brojilo = brojilo
                };
            }

            brojilo.Trofazno.MaxSnaga = dto.MaxSnaga;
            brojilo.Trofazno.MogucnostMerenjaPoZonama = ParseRequiredEnum<DaNe>(dto.MogucnostMerenjaPoZonama, "Merenje po zonama je obavezno.");
            brojilo.Trofazno.UgovorenaSnaga = dto.UgovorenaSnaga;
        }

        private static void ApplyRasvetno(Brojilo brojilo, RasvetnoBrojiloDto dto)
        {
            if (brojilo.Rasvetno == null)
            {
                brojilo.Rasvetno = new RasvetnoBrojilo
                {
                    Brojilo = brojilo
                };
            }

            brojilo.Rasvetno.VremeUkljucenja = Normalize(dto.VremeUkljucenja);
            brojilo.Rasvetno.VremeIskljucenja = Normalize(dto.VremeIskljucenja);
            brojilo.Rasvetno.StatusSenzora = Normalize(dto.StatusSenzora);
            brojilo.Rasvetno.RadnoVreme = dto.RadnoVreme;
        }

        private static async Task RemoveMehanicko(ISession session, Brojilo brojilo)
        {
            if (brojilo.Mehanicko == null)
            {
                return;
            }

            await session.DeleteAsync(brojilo.Mehanicko).ConfigureAwait(false);
            brojilo.Mehanicko = null;
        }

        private static async Task RemovePametno(ISession session, Brojilo brojilo)
        {
            if (brojilo.Pametno == null)
            {
                return;
            }

            await session.DeleteAsync(brojilo.Pametno).ConfigureAwait(false);
            brojilo.Pametno = null;
        }

        private static async Task RemoveTrofazno(ISession session, Brojilo brojilo)
        {
            if (brojilo.Trofazno == null)
            {
                return;
            }

            await session.DeleteAsync(brojilo.Trofazno).ConfigureAwait(false);
            brojilo.Trofazno = null;
        }

        private static async Task RemoveRasvetno(ISession session, Brojilo brojilo)
        {
            if (brojilo.Rasvetno == null)
            {
                return;
            }

            await session.DeleteAsync(brojilo.Rasvetno).ConfigureAwait(false);
            brojilo.Rasvetno = null;
        }

        private static BrojiloDto MapToDto(Brojilo brojilo)
        {
            return new BrojiloDto
            {
                SerijskiBroj = brojilo.SerijskiBroj,
                DatumInstalacije = brojilo.DatumInstalacije,
                Status = brojilo.Status,
                Lokacija = brojilo.Lokacija,
                KoeficijentMnozenja = brojilo.KoeficijentMnozenja,
                Komentar = brojilo.Komentar,
                TipoviBrojila = brojilo.TipoviBrojila.Select(x => x.ToString()).ToList(),
                DatumiZamene = brojilo.DatumiZamene.ToList(),
                Mehanicko = MapMehanicko(brojilo.Mehanicko),
                Pametno = MapPametno(brojilo.Pametno),
                Trofazno = MapTrofazno(brojilo.Trofazno),
                Rasvetno = MapRasvetno(brojilo.Rasvetno),
                Potrosaci = brojilo.Potrosaci
                    .Select(MapToPotrosacListDto)
                    .ToList(),
                Merenja = brojilo.Merenja.Select(MapToMerenjeListDto).ToList(),
                Kvarovi = brojilo.Kvarovi.Select(MapToKvarListDto).ToList()
            };
        }

        private static BrojiloListDto MapToListDto(Brojilo brojilo)
        {
            return new BrojiloListDto
            {
                SerijskiBroj = brojilo.SerijskiBroj,
                TipoviBrojila = brojilo.TipoviBrojila.Select(x => x.ToString()).ToList(),
                DatumInstalacije = brojilo.DatumInstalacije,
                PoslednjiDatumZamene = brojilo.DatumiZamene.Count == 0
                    ? (DateTime?)null
                    : brojilo.DatumiZamene.Max(),
                Status = brojilo.Status,
                Lokacija = brojilo.Lokacija,
                KoeficijentMnozenja = brojilo.KoeficijentMnozenja,
                BrojPotrosaca = brojilo.VezePotrosaca.Count(IsAktivnaVeza)
            };
        }

        private static MehanickoBrojiloDto MapMehanicko(MehanickoBrojilo mehanicko)
        {
            if (mehanicko == null)
            {
                return null;
            }

            return new MehanickoBrojiloDto
            {
                PoslednjaKalibracija = mehanicko.PoslednjaKalibracija,
                Preciznost = mehanicko.Preciznost,
                MaxGreska = mehanicko.MaxGreska
            };
        }

        private static PametnoBrojiloDto MapPametno(PametnoBrojilo pametno)
        {
            if (pametno == null)
            {
                return null;
            }

            return new PametnoBrojiloDto
            {
                Protokol = pametno.Protokol.HasValue ? pametno.Protokol.Value.ToString() : null,
                Frekvencija = pametno.Frekvencija,
                IsDaljinskoIskljucenje = pametno.IsDaljinskoIskljucenje.ToString(),
                NivoBaterije = pametno.NivoBaterije
            };
        }

        private static TrofaznoBrojiloDto MapTrofazno(TrofaznoBrojilo trofazno)
        {
            if (trofazno == null)
            {
                return null;
            }

            return new TrofaznoBrojiloDto
            {
                MaxSnaga = trofazno.MaxSnaga,
                MogucnostMerenjaPoZonama = trofazno.MogucnostMerenjaPoZonama.ToString(),
                UgovorenaSnaga = trofazno.UgovorenaSnaga
            };
        }

        private static RasvetnoBrojiloDto MapRasvetno(RasvetnoBrojilo rasvetno)
        {
            if (rasvetno == null)
            {
                return null;
            }

            return new RasvetnoBrojiloDto
            {
                VremeUkljucenja = rasvetno.VremeUkljucenja,
                VremeIskljucenja = rasvetno.VremeIskljucenja,
                StatusSenzora = rasvetno.StatusSenzora,
                RadnoVreme = rasvetno.RadnoVreme
            };
        }

        private static PotrosacListDto MapToPotrosacListDto(Potrosac potrosac)
        {
            return new PotrosacListDto
            {
                Id = potrosac.Id,
                Tip = potrosac.Tip.ToString(),
                ImeIliNaziv = BuildImeIliNaziv(potrosac),
                Grad = potrosac.Grad,
                Telefon = potrosac.Telefon,
                Email = potrosac.Email,
                Status = potrosac.Status,
                KategorijaTarife = potrosac.KategorijaTarife,
                BrojBrojila = potrosac.VezeBrojila.Count(IsAktivnaVeza)
            };
        }

        private static bool IsAktivnaVeza(PotrosacBrojiloVeza veza)
        {
            return veza != null && !veza.DatumDo.HasValue;
        }

        private static MerenjeListDto MapToMerenjeListDto(Merenje merenje)
        {
            return new MerenjeListDto
            {
                Id = merenje.Id,
                SerijskiBroj = merenje.Brojilo != null ? merenje.Brojilo.SerijskiBroj : null,
                DatumVremeMerenja = merenje.DatumVremeMerenja,
                PotrosnjaAktivna = merenje.PotrosnjaAktivna,
                PotrosnjaReaktivna = merenje.PotrosnjaReaktivna,
                Snaga = merenje.Snaga,
                Napon = merenje.Napon,
                Struja = merenje.Struja,
                TipMerenja = merenje.TipMerenja,
                TipIzvora = merenje.TipIzvora.HasValue ? merenje.TipIzvora.Value.ToString() : null,
                IsValidirano = merenje.IsValidirano == DaNe.D ? "DA" : "NE"
            };
        }

        private static KvarListDto MapToKvarListDto(Kvar kvar)
        {
            return new KvarListDto
            {
                Id = kvar.Id,
                SerijskiBroj = kvar.Brojilo != null ? kvar.Brojilo.SerijskiBroj : null,
                PotrosacId = kvar.Potrosac != null ? kvar.Potrosac.Id : 0,
                ImeIliNazivPotrosaca = kvar.Potrosac != null ? BuildImeIliNaziv(kvar.Potrosac) : null,
                DatumPrijave = kvar.DatumPrijave,
                TipKvara = kvar.TipKvara,
                Status = kvar.Status.ToString(),
                Prioritet = kvar.Prioritet.HasValue ? kvar.Prioritet.Value.ToString() : null,
                NadlezniTim = kvar.NadlezniTim,
                TrajanjeUSatima = kvar.TrajanjeUSatima,
                DatumOtklanjanja = kvar.DatumOtklanjanja
            };
        }

        private static string BuildImeIliNaziv(Potrosac potrosac)
        {
            if (potrosac.Domacinstvo != null)
            {
                return (Normalize(potrosac.Domacinstvo.Ime) + " " + Normalize(potrosac.Domacinstvo.Prezime)).Trim();
            }

            if (potrosac.Firma != null)
            {
                return potrosac.Firma.Naziv;
            }

            return potrosac.Tip.ToString();
        }

        private static async Task<Brojilo> GetRequiredBrojilo(ISession session, string serijskiBroj)
        {
            var brojilo = await session.GetAsync<Brojilo>(serijskiBroj.Trim()).ConfigureAwait(false);

            if (brojilo == null)
            {
                throw new KeyNotFoundException("Brojilo nije pronadjeno.");
            }

            return brojilo;
        }

        private static IList<TipBrojila> ValidateSaveDto(BrojiloSaveDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            ValidateSerijskiBroj(dto.SerijskiBroj);

            if (dto.DatumInstalacije == default(DateTime))
            {
                throw new ArgumentException("Datum instalacije je obavezan.");
            }

            Require(dto.Status, "Status brojila je obavezan.");

            var tipovi = ParseTipoviBrojila(dto.TipoviBrojila);
            ValidateSubtypeDetails(dto, tipovi);
            return tipovi;
        }

        private static void ValidateSubtypeDetails(BrojiloSaveDto dto, IList<TipBrojila> tipovi)
        {
            if (tipovi.Contains(TipBrojila.MEHANICKO) && dto.Mehanicko == null)
            {
                throw new ArgumentException("Podaci za mehanicko brojilo su obavezni.");
            }

            if (tipovi.Contains(TipBrojila.PAMETNO))
            {
                if (dto.Pametno == null)
                {
                    throw new ArgumentException("Podaci za pametno brojilo su obavezni.");
                }

                ParseOptionalEnum<PametnoBrojiloProtokol>(dto.Pametno.Protokol, "Protokol pametnog brojila nije ispravan.");
                ParseRequiredEnum<DaNe>(dto.Pametno.IsDaljinskoIskljucenje, "Daljinsko iskljucenje je obavezno.");
            }

            if (tipovi.Contains(TipBrojila.TROFAZNO))
            {
                if (dto.Trofazno == null)
                {
                    throw new ArgumentException("Podaci za trofazno brojilo su obavezni.");
                }

                ParseRequiredEnum<DaNe>(dto.Trofazno.MogucnostMerenjaPoZonama, "Merenje po zonama je obavezno.");
            }

            if (tipovi.Contains(TipBrojila.RASVETNO) && dto.Rasvetno == null)
            {
                throw new ArgumentException("Podaci za rasvetno brojilo su obavezni.");
            }
        }

        private static IList<TipBrojila> ParseTipoviBrojila(IList<string> values)
        {
            if (values == null || values.Count == 0)
            {
                throw new ArgumentException("Izaberi bar jedan tip brojila.");
            }

            var tipovi = new List<TipBrojila>();

            foreach (var value in values)
            {
                var tip = ParseRequiredEnum<TipBrojila>(value, "Tip brojila nije ispravan.");

                if (!tipovi.Contains(tip))
                {
                    tipovi.Add(tip);
                }
            }

            if (tipovi.Contains(TipBrojila.PAMETNO) && tipovi.Contains(TipBrojila.MEHANICKO))
            {
                throw new ArgumentException("Brojilo ne moze biti i PAMETNO i MEHANICKO.");
            }

            if (tipovi.Contains(TipBrojila.JEDNOFAZNO) && tipovi.Contains(TipBrojila.TROFAZNO))
            {
                throw new ArgumentException("Brojilo ne moze biti i JEDNOFAZNO i TROFAZNO.");
            }

            return tipovi;
        }

        private static void ValidateSerijskiBroj(string serijskiBroj)
        {
            Require(serijskiBroj, "Serijski broj je obavezan.");
        }

        private static TEnum ParseRequiredEnum<TEnum>(string value, string message) where TEnum : struct
        {
            var text = Require(value, message);
            TEnum result;

            if (!Enum.TryParse(text, out result))
            {
                throw new ArgumentException(message);
            }

            return result;
        }

        private static TEnum? ParseOptionalEnum<TEnum>(string value, string message) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            TEnum result;

            if (!Enum.TryParse(value.Trim(), out result))
            {
                throw new ArgumentException(message);
            }

            return result;
        }

        private static string Require(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message);
            }

            return value.Trim();
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
