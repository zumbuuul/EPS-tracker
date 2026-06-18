using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using app.DTO;
using app.Entities;
using app.Entities.Enums;
using app.Persistence;
using NHibernate;
using NHibernate.Linq;

namespace app.Services
{
    public class PotrosacService
    {
        private readonly NHibernateSessionFactoryProvider sessionFactoryProvider;

        public PotrosacService(NHibernateSessionFactoryProvider sessionFactoryProvider)
        {
            this.sessionFactoryProvider = sessionFactoryProvider
                ?? throw new ArgumentNullException(nameof(sessionFactoryProvider));
        }

        public IList<PotrosacListDto> VratiPotrosace()
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                return session.Query<Potrosac>()
                    .ToList()
                    .Select(MapToListDto)
                    .ToList();
            }
        }

        public PotrosacDto VratiPotrosaca(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var potrosac = GetRequiredPotrosac(session, id);
                return MapToDto(potrosac);
            }
        }

        public IList<BrojiloListDto> VratiBrojilaZaPotrosaca(long potrosacId)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var potrosac = GetRequiredPotrosac(session, potrosacId);

                return potrosac.Brojila
                    .Select(MapToBrojiloListDto)
                    .ToList();
            }
        }

        public long SacuvajPotrosaca(PotrosacSaveDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.Id == 0)
            {
                return DodajPotrosaca(dto);
            }

            IzmeniPotrosaca(dto);
            return dto.Id;
        }

        public long DodajPotrosaca(PotrosacSaveDto dto)
        {
            ValidateSaveDto(dto);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = new Potrosac();
                ApplyScalarFields(potrosac, dto);
                ApplyDetails(session, potrosac, dto);

                session.Save(potrosac);
                transaction.Commit();

                return potrosac.Id;
            }
        }

        public void IzmeniPotrosaca(PotrosacSaveDto dto)
        {
            ValidateSaveDto(dto);

            if (dto.Id <= 0)
            {
                throw new ArgumentException("Id potrosaca je obavezan za izmenu.", nameof(dto));
            }

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = GetRequiredPotrosac(session, dto.Id);

                ApplyScalarFields(potrosac, dto);
                ApplyDetails(session, potrosac, dto);

                transaction.Commit();
            }
        }

        public void ObrisiPotrosaca(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = GetRequiredPotrosac(session, id);

                if (potrosac.Racuni.Count > 0)
                {
                    throw new InvalidOperationException("Potrosac ima racune i ne moze biti obrisan.");
                }

                if (potrosac.Kvarovi.Count > 0)
                {
                    throw new InvalidOperationException("Potrosac ima evidentirane kvarove i ne moze biti obrisan.");
                }

                foreach (var brojilo in potrosac.Brojila.ToList())
                {
                    potrosac.Brojila.Remove(brojilo);
                    brojilo.Potrosaci.Remove(potrosac);
                }

                session.Delete(potrosac);
                transaction.Commit();
            }
        }

        public void PoveziPotrosacaIBrojilo(PotrosacBrojiloLinkDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            PoveziPotrosacaIBrojilo(dto.PotrosacId, dto.SerijskiBroj);
        }

        public void PoveziPotrosacaIBrojilo(long potrosacId, string serijskiBroj)
        {
            ValidateSerijskiBroj(serijskiBroj);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = GetRequiredPotrosac(session, potrosacId);
                var brojilo = GetRequiredBrojilo(session, serijskiBroj);

                if (!potrosac.Brojila.Any(x => IsSameSerijskiBroj(x.SerijskiBroj, serijskiBroj)))
                {
                    potrosac.Brojila.Add(brojilo);
                    brojilo.Potrosaci.Add(potrosac);
                }

                transaction.Commit();
            }
        }

        public void RaskiniVezuPotrosacBrojilo(PotrosacBrojiloLinkDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            RaskiniVezuPotrosacBrojilo(dto.PotrosacId, dto.SerijskiBroj);
        }

        public void RaskiniVezuPotrosacBrojilo(long potrosacId, string serijskiBroj)
        {
            ValidateSerijskiBroj(serijskiBroj);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = GetRequiredPotrosac(session, potrosacId);
                var brojilo = GetRequiredBrojilo(session, serijskiBroj);
                var linkedBrojilo = potrosac.Brojila
                    .FirstOrDefault(x => IsSameSerijskiBroj(x.SerijskiBroj, serijskiBroj));

                if (linkedBrojilo == null)
                {
                    throw new InvalidOperationException("Potrosac nije povezan sa tim brojilom.");
                }

                potrosac.Brojila.Remove(linkedBrojilo);
                brojilo.Potrosaci.Remove(potrosac);

                transaction.Commit();
            }
        }

        private static void ApplyScalarFields(Potrosac potrosac, PotrosacSaveDto dto)
        {
            potrosac.Tip = dto.Tip;
            potrosac.Email = Normalize(dto.Email);
            potrosac.Telefon = Normalize(dto.Telefon);
            potrosac.Adresa = Require(dto.Adresa, "Adresa je obavezna.");
            potrosac.Grad = Require(dto.Grad, "Grad je obavezan.");
            potrosac.Komentar = Normalize(dto.Komentar);
            potrosac.Status = Require(dto.Status, "Status potrosaca je obavezan.");
            potrosac.KategorijaTarife = Require(dto.KategorijaTarife, "Kategorija tarife je obavezna.");
        }

        private static void ApplyDetails(ISession session, Potrosac potrosac, PotrosacSaveDto dto)
        {
            if (dto.Tip == PotrosacTip.DOMACINSTVO)
            {
                RemoveFirma(session, potrosac);
                ApplyDomacinstvo(potrosac, dto.Domacinstvo);
                return;
            }

            if (dto.Tip == PotrosacTip.FIRMA)
            {
                RemoveDomacinstvo(session, potrosac);
                ApplyFirma(potrosac, dto.Firma);
                return;
            }

            RemoveDomacinstvo(session, potrosac);
            RemoveFirma(session, potrosac);
        }

        private static void ApplyDomacinstvo(Potrosac potrosac, DomacinstvoDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Podaci o domacinstvu su obavezni za tip DOMACINSTVO.");
            }

            if (potrosac.Domacinstvo == null)
            {
                potrosac.Domacinstvo = new Domacinstvo
                {
                    Potrosac = potrosac
                };
            }

            potrosac.Domacinstvo.Jmbg = RequireDigits(dto.Jmbg, 13, "JMBG mora imati tacno 13 cifara.");
            potrosac.Domacinstvo.Ime = Require(dto.Ime, "Ime je obavezno.");
            potrosac.Domacinstvo.Prezime = Require(dto.Prezime, "Prezime je obavezno.");
        }

        private static void ApplyFirma(Potrosac potrosac, FirmaDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Podaci o firmi su obavezni za tip FIRMA.");
            }

            if (potrosac.Firma == null)
            {
                potrosac.Firma = new Firma
                {
                    Potrosac = potrosac
                };
            }

            potrosac.Firma.Naziv = Require(dto.Naziv, "Naziv firme je obavezan.");
            potrosac.Firma.Pib = RequireDigits(dto.Pib, 9, "PIB mora imati tacno 9 cifara.");
        }

        private static void RemoveDomacinstvo(ISession session, Potrosac potrosac)
        {
            if (potrosac.Domacinstvo == null)
            {
                return;
            }

            session.Delete(potrosac.Domacinstvo);
            potrosac.Domacinstvo = null;
        }

        private static void RemoveFirma(ISession session, Potrosac potrosac)
        {
            if (potrosac.Firma == null)
            {
                return;
            }

            session.Delete(potrosac.Firma);
            potrosac.Firma = null;
        }

        private static PotrosacListDto MapToListDto(Potrosac potrosac)
        {
            return new PotrosacListDto
            {
                Id = potrosac.Id,
                Tip = potrosac.Tip,
                ImeIliNaziv = BuildImeIliNaziv(potrosac),
                Grad = potrosac.Grad,
                Telefon = potrosac.Telefon,
                Email = potrosac.Email,
                Status = potrosac.Status,
                KategorijaTarife = potrosac.KategorijaTarife,
                BrojBrojila = potrosac.Brojila.Count
            };
        }

        private static PotrosacDto MapToDto(Potrosac potrosac)
        {
            return new PotrosacDto
            {
                Id = potrosac.Id,
                Tip = potrosac.Tip,
                Email = potrosac.Email,
                Telefon = potrosac.Telefon,
                Adresa = potrosac.Adresa,
                Grad = potrosac.Grad,
                Komentar = potrosac.Komentar,
                Status = potrosac.Status,
                KategorijaTarife = potrosac.KategorijaTarife,
                Domacinstvo = MapDomacinstvo(potrosac.Domacinstvo),
                Firma = MapFirma(potrosac.Firma),
                Brojila = potrosac.Brojila.Select(MapToBrojiloListDto).ToList(),
                Racuni = potrosac.Racuni.Select(x => MapToRacunListDto(x, potrosac)).ToList(),
                Kvarovi = potrosac.Kvarovi.Select(MapToKvarListDto).ToList()
            };
        }

        private static DomacinstvoDto MapDomacinstvo(Domacinstvo domacinstvo)
        {
            if (domacinstvo == null)
            {
                return null;
            }

            return new DomacinstvoDto
            {
                Id = domacinstvo.Id,
                Jmbg = domacinstvo.Jmbg,
                Ime = domacinstvo.Ime,
                Prezime = domacinstvo.Prezime
            };
        }

        private static FirmaDto MapFirma(Firma firma)
        {
            if (firma == null)
            {
                return null;
            }

            return new FirmaDto
            {
                Id = firma.Id,
                Naziv = firma.Naziv,
                Pib = firma.Pib
            };
        }

        private static BrojiloListDto MapToBrojiloListDto(Brojilo brojilo)
        {
            return new BrojiloListDto
            {
                SerijskiBroj = brojilo.SerijskiBroj,
                TipoviBrojila = brojilo.TipoviBrojila.ToList(),
                DatumInstalacije = brojilo.DatumInstalacije,
                PoslednjiDatumZamene = brojilo.DatumiZamene.Count == 0
                    ? (DateTime?)null
                    : brojilo.DatumiZamene.Max(),
                Status = brojilo.Status,
                Lokacija = brojilo.Lokacija,
                KoeficijentMnozenja = brojilo.KoeficijentMnozenja,
                BrojPotrosaca = brojilo.Potrosaci.Count
            };
        }

        private static RacunListDto MapToRacunListDto(Racun racun, Potrosac potrosac)
        {
            var merenje = racun.Merenje;

            return new RacunListDto
            {
                BrojRacuna = racun.BrojRacuna,
                PotrosacId = potrosac.Id,
                ImeIliNazivPotrosaca = BuildImeIliNaziv(potrosac),
                SerijskiBroj = merenje != null && merenje.Brojilo != null
                    ? merenje.Brojilo.SerijskiBroj
                    : null,
                PeriodPotrosnjeOd = racun.PeriodPotrosnjeOd,
                PeriodPotrosnjeDo = racun.PeriodPotrosnjeDo,
                UkupnaPotrosnja = merenje != null ? merenje.PotrosnjaAktivna : null,
                IznosBezPdv = racun.IznosBezPdv,
                Pdv = racun.Pdv,
                UkupanIznos = racun.IznosBezPdv + racun.Pdv,
                DatumIzdavanja = racun.DatumIzdavanja,
                RokPlacanja = racun.RokPlacanja,
                Status = racun.Status,
                NacinPlacanja = racun.NacinPlacanja
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
                Status = kvar.Status,
                Prioritet = kvar.Prioritet,
                NadlezniTim = kvar.NadlezniTim,
                TrajanjeUSatima = CalculateTrajanjeUSatima(kvar)
            };
        }

        private static decimal? CalculateTrajanjeUSatima(Kvar kvar)
        {
            if (!kvar.DatumOtklanjanja.HasValue)
            {
                return null;
            }

            return (decimal)(kvar.DatumOtklanjanja.Value - kvar.DatumPrijave).TotalHours;
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

        private static Potrosac GetRequiredPotrosac(ISession session, long id)
        {
            var potrosac = session.Get<Potrosac>(id);

            if (potrosac == null)
            {
                throw new KeyNotFoundException("Potrosac nije pronadjen.");
            }

            return potrosac;
        }

        private static Brojilo GetRequiredBrojilo(ISession session, string serijskiBroj)
        {
            var brojilo = session.Get<Brojilo>(serijskiBroj);

            if (brojilo == null)
            {
                throw new KeyNotFoundException("Brojilo nije pronadjeno.");
            }

            return brojilo;
        }

        private static void ValidateSaveDto(PotrosacSaveDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            Require(dto.Adresa, "Adresa je obavezna.");
            Require(dto.Grad, "Grad je obavezan.");
            Require(dto.Status, "Status potrosaca je obavezan.");
            Require(dto.KategorijaTarife, "Kategorija tarife je obavezna.");

            if (!string.IsNullOrWhiteSpace(dto.Telefon) && !IsDigitsOnly(dto.Telefon.Trim()))
            {
                throw new ArgumentException("Telefon sme da sadrzi samo cifre.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email.Trim()))
            {
                throw new ArgumentException("Email mora biti u ispravnom formatu.");
            }
        }

        private static void ValidateSerijskiBroj(string serijskiBroj)
        {
            Require(serijskiBroj, "Serijski broj je obavezan.");
        }

        private static string Require(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message);
            }

            return value.Trim();
        }

        private static string RequireDigits(string value, int length, string message)
        {
            var text = Require(value, message);

            if (text.Length != length || !IsDigitsOnly(text))
            {
                throw new ArgumentException(message);
            }

            return text;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool IsSameSerijskiBroj(string left, string right)
        {
            return string.Equals(left, right, StringComparison.Ordinal);
        }

        private static bool IsDigitsOnly(string value)
        {
            return value != null && value.Length > 0 && value.All(IsAsciiDigit);
        }

        private static bool IsAsciiDigit(char value)
        {
            return value >= '0' && value <= '9';
        }

        private static bool IsValidEmail(string value)
        {
            try
            {
                var address = new MailAddress(value);
                return address.Address == value;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
