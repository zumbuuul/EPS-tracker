using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.DTO;
using app.Entities;
using app.Persistence;
using NHibernate;
using NHibernate.Linq;

namespace app.Services
{
    public class RacunService
    {
        private readonly NHibernateSessionFactoryProvider sessionFactoryProvider;

        public RacunService(NHibernateSessionFactoryProvider sessionFactoryProvider)
        {
            this.sessionFactoryProvider = sessionFactoryProvider
                ?? throw new ArgumentNullException(nameof(sessionFactoryProvider));
        }

        public async Task<IList<RacunListDto>> VratiRacune()
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var racuni = await session.Query<Racun>()
                    .OrderByDescending(x => x.DatumIzdavanja)
                    .ThenBy(x => x.BrojRacuna)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return racuni.Select(MapToListDto).ToList();
            }
        }

        public async Task<RacunDto> VratiRacun(string brojRacuna)
        {
            ValidateBrojRacuna(brojRacuna);

            using (var session = sessionFactoryProvider.OpenSession())
            {
                var racun = await GetRequiredRacun(session, brojRacuna).ConfigureAwait(false);
                return MapToDto(racun);
            }
        }

        public async Task<RacunDto> VratiRacunZaMerenje(long merenjeId)
        {
            if (merenjeId <= 0)
            {
                throw new ArgumentException("Id merenja je obavezan.");
            }

            using (var session = sessionFactoryProvider.OpenSession())
            {
                await GetRequiredMerenje(session, merenjeId).ConfigureAwait(false);

                var racun = await session.Query<Racun>()
                    .FirstOrDefaultAsync(x => x.Merenje.Id == merenjeId)
                    .ConfigureAwait(false);

                return racun == null ? null : MapToDto(racun);
            }
        }

        public async Task<string> DodajRacun(RacunSaveDto dto)
        {
            ValidateSaveDto(dto);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var brojRacuna = dto.BrojRacuna.Trim();
                var existing = await session.GetAsync<Racun>(brojRacuna).ConfigureAwait(false);

                if (existing != null)
                {
                    throw new InvalidOperationException("Racun sa tim brojem vec postoji.");
                }

                var potrosac = await GetRequiredPotrosac(session, dto.PotrosacId).ConfigureAwait(false);
                var merenje = await GetRequiredMerenje(session, dto.MerenjeId).ConfigureAwait(false);
                await ValidateMerenjeNemaDrugiRacun(session, dto.MerenjeId, null).ConfigureAwait(false);
                ValidatePotrosacZaMerenje(potrosac, merenje);

                var racun = new Racun();
                ApplyFields(racun, dto, potrosac, merenje);

                await session.SaveAsync(racun).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return racun.BrojRacuna;
            }
        }

        public async Task IzmeniRacun(RacunSaveDto dto)
        {
            ValidateSaveDto(dto);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var racun = await GetRequiredRacun(session, dto.BrojRacuna).ConfigureAwait(false);
                var potrosac = await GetRequiredPotrosac(session, dto.PotrosacId).ConfigureAwait(false);
                var merenje = await GetRequiredMerenje(session, dto.MerenjeId).ConfigureAwait(false);

                await ValidateMerenjeNemaDrugiRacun(session, dto.MerenjeId, dto.BrojRacuna).ConfigureAwait(false);
                ValidatePotrosacZaMerenje(potrosac, merenje);
                ApplyFields(racun, dto, potrosac, merenje);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task ObrisiRacun(string brojRacuna)
        {
            ValidateBrojRacuna(brojRacuna);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var racun = await GetRequiredRacun(session, brojRacuna).ConfigureAwait(false);

                await session.DeleteAsync(racun).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task OznaciKaoPlacen(string brojRacuna)
        {
            ValidateBrojRacuna(brojRacuna);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var racun = await GetRequiredRacun(session, brojRacuna).ConfigureAwait(false);

                racun.Status = "PLACEN";

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        private static void ApplyFields(Racun racun, RacunSaveDto dto, Potrosac potrosac, Merenje merenje)
        {
            racun.BrojRacuna = dto.BrojRacuna.Trim();
            racun.Potrosac = potrosac;
            racun.Merenje = merenje;
            racun.DatumIzdavanja = dto.DatumIzdavanja;
            racun.RokPlacanja = dto.RokPlacanja;
            racun.PeriodPotrosnjeOd = dto.PeriodPotrosnjeOd;
            racun.PeriodPotrosnjeDo = dto.PeriodPotrosnjeDo;
            racun.IznosBezPdv = dto.IznosBezPdv;
            racun.Pdv = dto.Pdv;
            racun.Status = dto.Status.Trim();
            racun.NacinPlacanja = dto.NacinPlacanja.Trim();
            racun.Komentar = Normalize(dto.Komentar);
        }

        private static RacunListDto MapToListDto(Racun racun)
        {
            return new RacunListDto
            {
                BrojRacuna = racun.BrojRacuna,
                PotrosacId = racun.Potrosac != null ? racun.Potrosac.Id : 0,
                ImeIliNazivPotrosaca = racun.Potrosac != null ? BuildImeIliNaziv(racun.Potrosac) : null,
                SerijskiBroj = racun.SerijskiBroj,
                PeriodPotrosnjeOd = racun.PeriodPotrosnjeOd,
                PeriodPotrosnjeDo = racun.PeriodPotrosnjeDo,
                UkupnaPotrosnja = racun.UkupnaPotrosnja,
                IznosBezPdv = racun.IznosBezPdv,
                Pdv = racun.Pdv,
                UkupanIznos = racun.UkupanIznos,
                DatumIzdavanja = racun.DatumIzdavanja,
                RokPlacanja = racun.RokPlacanja,
                Status = racun.Status,
                NacinPlacanja = racun.NacinPlacanja
            };
        }

        private static RacunDto MapToDto(Racun racun)
        {
            return new RacunDto
            {
                BrojRacuna = racun.BrojRacuna,
                PotrosacId = racun.Potrosac != null ? racun.Potrosac.Id : 0,
                ImeIliNazivPotrosaca = racun.Potrosac != null ? BuildImeIliNaziv(racun.Potrosac) : null,
                SerijskiBroj = racun.SerijskiBroj,
                MerenjeId = racun.Merenje != null ? racun.Merenje.Id : 0,
                DatumIzdavanja = racun.DatumIzdavanja,
                RokPlacanja = racun.RokPlacanja,
                PeriodPotrosnjeOd = racun.PeriodPotrosnjeOd,
                PeriodPotrosnjeDo = racun.PeriodPotrosnjeDo,
                UkupnaPotrosnja = racun.UkupnaPotrosnja,
                IznosBezPdv = racun.IznosBezPdv,
                Pdv = racun.Pdv,
                UkupanIznos = racun.UkupanIznos,
                Status = racun.Status,
                NacinPlacanja = racun.NacinPlacanja,
                Komentar = racun.Komentar
            };
        }

        private static async Task<Racun> GetRequiredRacun(ISession session, string brojRacuna)
        {
            var racun = await session.GetAsync<Racun>(brojRacuna.Trim()).ConfigureAwait(false);

            if (racun == null)
            {
                throw new KeyNotFoundException("Racun nije pronadjen.");
            }

            return racun;
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

        private static async Task<Merenje> GetRequiredMerenje(ISession session, long id)
        {
            var merenje = await session.GetAsync<Merenje>(id).ConfigureAwait(false);

            if (merenje == null)
            {
                throw new KeyNotFoundException("Merenje nije pronadjeno.");
            }

            return merenje;
        }

        private static async Task ValidateMerenjeNemaDrugiRacun(
            ISession session,
            long merenjeId,
            string currentBrojRacuna)
        {
            var query = session.Query<Racun>()
                .Where(x => x.Merenje.Id == merenjeId);

            if (!string.IsNullOrWhiteSpace(currentBrojRacuna))
            {
                query = query.Where(x => x.BrojRacuna != currentBrojRacuna.Trim());
            }

            var exists = await query.AnyAsync().ConfigureAwait(false);

            if (exists)
            {
                throw new InvalidOperationException("Za ovo merenje vec postoji racun.");
            }
        }

        private static void ValidatePotrosacZaMerenje(Potrosac potrosac, Merenje merenje)
        {
            if (merenje.Brojilo == null)
            {
                throw new InvalidOperationException("Merenje nema povezano brojilo.");
            }

            var serijskiBroj = merenje.Brojilo.SerijskiBroj;

            if (!potrosac.Brojila.Any(x => string.Equals(x.SerijskiBroj, serijskiBroj, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException("Potrosac nije povezan sa brojilom za izabrano merenje.");
            }
        }

        private static void ValidateSaveDto(RacunSaveDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            ValidateBrojRacuna(dto.BrojRacuna);

            if (dto.PotrosacId <= 0)
            {
                throw new ArgumentException("Potrosac ID je obavezan.");
            }

            if (dto.MerenjeId <= 0)
            {
                throw new ArgumentException("Merenje ID je obavezan.");
            }

            if (dto.DatumIzdavanja == default(DateTime))
            {
                throw new ArgumentException("Datum izdavanja je obavezan.");
            }

            if (dto.RokPlacanja == default(DateTime))
            {
                throw new ArgumentException("Rok placanja je obavezan.");
            }

            if (dto.PeriodPotrosnjeOd == default(DateTime) || dto.PeriodPotrosnjeDo == default(DateTime))
            {
                throw new ArgumentException("Period potrosnje je obavezan.");
            }

            if (dto.PeriodPotrosnjeOd > dto.PeriodPotrosnjeDo)
            {
                throw new ArgumentException("Period potrosnje nije ispravan.");
            }

            if (dto.RokPlacanja < dto.DatumIzdavanja)
            {
                throw new ArgumentException("Rok placanja ne moze biti pre datuma izdavanja.");
            }

            if (dto.IznosBezPdv < 0 || dto.Pdv < 0)
            {
                throw new ArgumentException("Iznosi ne mogu biti negativni.");
            }

            Require(dto.Status, "Status racuna je obavezan.");
            Require(dto.NacinPlacanja, "Nacin placanja je obavezan.");
        }

        private static void ValidateBrojRacuna(string brojRacuna)
        {
            Require(brojRacuna, "Broj racuna je obavezan.");
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
