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
    public class KvarService
    {
        private readonly NHibernateSessionFactoryProvider sessionFactoryProvider;

        public KvarService(NHibernateSessionFactoryProvider sessionFactoryProvider)
        {
            this.sessionFactoryProvider = sessionFactoryProvider
                ?? throw new ArgumentNullException(nameof(sessionFactoryProvider));
        }

        public async Task<IList<KvarListDto>> VratiKvarove()
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var kvarovi = await session.Query<Kvar>()
                    .OrderByDescending(x => x.DatumPrijave)
                    .ThenByDescending(x => x.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return kvarovi.Select(MapToListDto).ToList();
            }
        }

        public async Task<KvarDto> VratiKvar(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var kvar = await GetRequiredKvar(session, id).ConfigureAwait(false);
                return MapToDto(kvar);
            }
        }

        public async Task<long> DodajKvar(KvarSaveDto dto)
        {
            ValidateSaveDto(dto, false);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var brojilo = await GetRequiredBrojilo(session, dto.SerijskiBroj).ConfigureAwait(false);
                var potrosac = await GetRequiredPotrosac(session, dto.PotrosacId).ConfigureAwait(false);
                ValidatePotrosacZaBrojilo(potrosac, brojilo);

                var kvar = new Kvar();
                ApplyFields(kvar, dto, brojilo, potrosac, CalculateTrajanje(dto));

                await session.SaveAsync(kvar).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return kvar.Id;
            }
        }

        public async Task IzmeniKvar(KvarSaveDto dto)
        {
            ValidateSaveDto(dto, true);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var kvar = await GetRequiredKvar(session, dto.Id).ConfigureAwait(false);
                var brojilo = await GetRequiredBrojilo(session, dto.SerijskiBroj).ConfigureAwait(false);
                var potrosac = await GetRequiredPotrosac(session, dto.PotrosacId).ConfigureAwait(false);
                ValidatePotrosacZaBrojilo(potrosac, brojilo);

                ApplyFields(kvar, dto, brojilo, potrosac, CalculateTrajanje(dto));

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task ObrisiKvar(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var kvar = await GetRequiredKvar(session, id).ConfigureAwait(false);

                await session.DeleteAsync(kvar).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task OtkloniKvar(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var kvar = await GetRequiredKvar(session, id).ConfigureAwait(false);
                var datumOtklanjanja = DateTime.Now;

                kvar.Status = KvarStatus.OTKLONJEN;
                kvar.DatumOtklanjanja = datumOtklanjanja;
                kvar.TrajanjeUSatima = CalculateTrajanje(kvar.DatumPrijave, datumOtklanjanja);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task OtkloniKvar(OtkloniKvarDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.KvarId <= 0)
            {
                throw new ArgumentException("Id kvara je obavezan.");
            }

            if (dto.DatumOtklanjanja == default(DateTime))
            {
                throw new ArgumentException("Datum otklanjanja je obavezan.");
            }

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var kvar = await GetRequiredKvar(session, dto.KvarId).ConfigureAwait(false);

                if (dto.DatumOtklanjanja < kvar.DatumPrijave)
                {
                    throw new ArgumentException("Datum otklanjanja ne moze biti pre datuma prijave.");
                }

                kvar.Status = KvarStatus.OTKLONJEN;
                kvar.DatumOtklanjanja = dto.DatumOtklanjanja;
                kvar.TrajanjeUSatima = dto.TrajanjeUSatima ?? CalculateTrajanje(kvar.DatumPrijave, dto.DatumOtklanjanja);

                if (!string.IsNullOrWhiteSpace(dto.Komentar))
                {
                    kvar.Komentar = dto.Komentar.Trim();
                }

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        private static void ApplyFields(
            Kvar kvar,
            KvarSaveDto dto,
            Brojilo brojilo,
            Potrosac potrosac,
            decimal? trajanjeUSatima)
        {
            kvar.Brojilo = brojilo;
            kvar.Potrosac = potrosac;
            kvar.DatumPrijave = dto.DatumPrijave;
            kvar.TipKvara = dto.TipKvara.Trim();
            kvar.OpisProblema = Normalize(dto.OpisProblema);
            kvar.Status = ParseRequiredEnum<KvarStatus>(dto.Status, "Status kvara nije ispravan.");
            kvar.DatumOtklanjanja = dto.DatumOtklanjanja;
            kvar.TrajanjeUSatima = trajanjeUSatima;
            kvar.Prioritet = ParseOptionalEnum<KvarPrioritet>(dto.Prioritet, "Prioritet kvara nije ispravan.");
            kvar.NadlezniTim = Normalize(dto.NadlezniTim);
            kvar.Komentar = Normalize(dto.Komentar);
        }

        private static KvarListDto MapToListDto(Kvar kvar)
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

        private static KvarDto MapToDto(Kvar kvar)
        {
            return new KvarDto
            {
                Id = kvar.Id,
                SerijskiBroj = kvar.Brojilo != null ? kvar.Brojilo.SerijskiBroj : null,
                PotrosacId = kvar.Potrosac != null ? kvar.Potrosac.Id : 0,
                ImeIliNazivPotrosaca = kvar.Potrosac != null ? BuildImeIliNaziv(kvar.Potrosac) : null,
                DatumPrijave = kvar.DatumPrijave,
                TipKvara = kvar.TipKvara,
                OpisProblema = kvar.OpisProblema,
                Status = kvar.Status.ToString(),
                DatumOtklanjanja = kvar.DatumOtklanjanja,
                TrajanjeUSatima = kvar.TrajanjeUSatima,
                Prioritet = kvar.Prioritet.HasValue ? kvar.Prioritet.Value.ToString() : null,
                NadlezniTim = kvar.NadlezniTim,
                Komentar = kvar.Komentar
            };
        }

        private static async Task<Kvar> GetRequiredKvar(ISession session, long id)
        {
            var kvar = await session.GetAsync<Kvar>(id).ConfigureAwait(false);

            if (kvar == null)
            {
                throw new KeyNotFoundException("Kvar nije pronadjen.");
            }

            return kvar;
        }

        private static async Task<Brojilo> GetRequiredBrojilo(ISession session, string serijskiBroj)
        {
            var text = Require(serijskiBroj, "Serijski broj brojila je obavezan.");
            var brojilo = await session.GetAsync<Brojilo>(text).ConfigureAwait(false);

            if (brojilo == null)
            {
                throw new KeyNotFoundException("Brojilo nije pronadjeno.");
            }

            return brojilo;
        }

        private static async Task<Potrosac> GetRequiredPotrosac(ISession session, long id)
        {
            var potrosac = await session.GetAsync<Potrosac>(id).ConfigureAwait(false);

            if (potrosac == null)
            {
                throw new KeyNotFoundException("Potrosac nije pronadjen.");
            }

            return potrosac;
        }

        private static void ValidateSaveDto(KvarSaveDto dto, bool isEdit)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (isEdit && dto.Id <= 0)
            {
                throw new ArgumentException("Id kvara je obavezan za izmenu.");
            }

            Require(dto.SerijskiBroj, "Serijski broj brojila je obavezan.");

            if (dto.PotrosacId <= 0)
            {
                throw new ArgumentException("Potrosac ID je obavezan.");
            }

            if (dto.DatumPrijave == default(DateTime))
            {
                throw new ArgumentException("Datum prijave je obavezan.");
            }

            Require(dto.TipKvara, "Tip kvara je obavezan.");
            ParseRequiredEnum<KvarStatus>(dto.Status, "Status kvara nije ispravan.");
            ParseOptionalEnum<KvarPrioritet>(dto.Prioritet, "Prioritet kvara nije ispravan.");

            if (dto.DatumOtklanjanja.HasValue && dto.DatumOtklanjanja.Value < dto.DatumPrijave)
            {
                throw new ArgumentException("Datum otklanjanja ne moze biti pre datuma prijave.");
            }

            if (dto.TrajanjeUSatima.HasValue && dto.TrajanjeUSatima.Value < 0)
            {
                throw new ArgumentException("Trajanje kvara ne moze biti negativno.");
            }
        }

        private static void ValidatePotrosacZaBrojilo(Potrosac potrosac, Brojilo brojilo)
        {
            if (!potrosac.Brojila.Any(x => string.Equals(x.SerijskiBroj, brojilo.SerijskiBroj, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException("Potrosac nije povezan sa izabranim brojilom.");
            }
        }

        private static decimal? CalculateTrajanje(KvarSaveDto dto)
        {
            if (dto.TrajanjeUSatima.HasValue && dto.TrajanjeUSatima.Value > 0)
            {
                return dto.TrajanjeUSatima.Value;
            }

            if (!dto.DatumOtklanjanja.HasValue)
            {
                return null;
            }

            return CalculateTrajanje(dto.DatumPrijave, dto.DatumOtklanjanja.Value);
        }

        private static decimal CalculateTrajanje(DateTime datumPrijave, DateTime datumOtklanjanja)
        {
            return Math.Round(Convert.ToDecimal((datumOtklanjanja - datumPrijave).TotalHours), 2);
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
