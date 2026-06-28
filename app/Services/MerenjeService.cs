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
    public class MerenjeService
    {
        private readonly NHibernateSessionFactoryProvider sessionFactoryProvider;

        public MerenjeService(NHibernateSessionFactoryProvider sessionFactoryProvider)
        {
            this.sessionFactoryProvider = sessionFactoryProvider
                ?? throw new ArgumentNullException(nameof(sessionFactoryProvider));
        }

        public async Task<IList<MerenjeListDto>> VratiMerenja()
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var merenja = await session.Query<Merenje>()
                    .OrderByDescending(x => x.DatumVremeMerenja)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return merenja.Select(MapToListDto).ToList();
            }
        }

        public async Task<MerenjeDto> VratiMerenje(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var merenje = await GetRequiredMerenje(session, id).ConfigureAwait(false);
                return MapToDto(merenje);
            }
        }

        public async Task<RacunDto> VratiRacunZaMerenje(long merenjeId)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                await GetRequiredMerenje(session, merenjeId).ConfigureAwait(false);

                var racun = await session.Query<Racun>()
                    .FirstOrDefaultAsync(x => x.Merenje.Id == merenjeId)
                    .ConfigureAwait(false);

                return racun == null ? null : MapToRacunDto(racun);
            }
        }

        public async Task<long> DodajMerenje(MerenjeSaveDto dto)
        {
            ValidateSaveDto(dto);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var brojilo = await GetRequiredBrojilo(session, dto.SerijskiBroj).ConfigureAwait(false);
                var merenje = new Merenje();

                ApplyFields(merenje, dto, brojilo);

                await session.SaveAsync(merenje).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return merenje.Id;
            }
        }

        public async Task IzmeniMerenje(MerenjeSaveDto dto)
        {
            ValidateSaveDto(dto);

            if (dto.Id <= 0)
            {
                throw new ArgumentException("Id merenja je obavezan za izmenu.", nameof(dto));
            }

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var merenje = await GetRequiredMerenje(session, dto.Id).ConfigureAwait(false);
                var brojilo = await GetRequiredBrojilo(session, dto.SerijskiBroj).ConfigureAwait(false);

                ApplyFields(merenje, dto, brojilo);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task ObrisiMerenje(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var merenje = await GetRequiredMerenje(session, id).ConfigureAwait(false);
                var imaRacune = await session.Query<Racun>()
                    .AnyAsync(x => x.Merenje.Id == id)
                    .ConfigureAwait(false);

                if (imaRacune)
                {
                    throw new InvalidOperationException("Merenje ima racune i ne moze biti obrisano.");
                }

                await session.DeleteAsync(merenje).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task ValidirajMerenje(ValidacijaMerenjaDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.MerenjeId <= 0)
            {
                throw new ArgumentException("Id merenja je obavezan za validaciju.", nameof(dto));
            }

            var isValidirano = ParseDaNe(dto.IsValidirano);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var merenje = await GetRequiredMerenje(session, dto.MerenjeId).ConfigureAwait(false);

                merenje.IsValidirano = isValidirano;

                if (!string.IsNullOrWhiteSpace(dto.Komentar))
                {
                    merenje.Komentar = dto.Komentar.Trim();
                }

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        private static void ApplyFields(Merenje merenje, MerenjeSaveDto dto, Brojilo brojilo)
        {
            merenje.Brojilo = brojilo;
            merenje.DatumVremeMerenja = dto.DatumVremeMerenja;
            merenje.PotrosnjaAktivna = dto.PotrosnjaAktivna;
            merenje.PotrosnjaReaktivna = dto.PotrosnjaReaktivna;
            merenje.Snaga = dto.Snaga;
            merenje.Napon = dto.Napon;
            merenje.Struja = dto.Struja;
            merenje.TipMerenja = dto.TipMerenja.Trim();
            merenje.TipIzvora = ParseTipIzvora(dto.TipIzvora);
            merenje.IsValidirano = ParseDaNe(dto.IsValidirano);
            merenje.Komentar = dto.Komentar.Trim();
        }

        private static MerenjeDto MapToDto(Merenje merenje)
        {
            return new MerenjeDto
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
                IsValidirano = ToDaNeText(merenje.IsValidirano),
                Komentar = merenje.Komentar
            };
        }

        private static MerenjeListDto MapToListDto(Merenje merenje)
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
                IsValidirano = ToDaNeText(merenje.IsValidirano)
            };
        }

        private static RacunDto MapToRacunDto(Racun racun)
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

        private static string BuildImeIliNaziv(Potrosac potrosac)
        {
            if (potrosac.Domacinstvo != null)
            {
                return ((potrosac.Domacinstvo.Ime ?? string.Empty) + " " + (potrosac.Domacinstvo.Prezime ?? string.Empty)).Trim();
            }

            if (potrosac.Firma != null)
            {
                return potrosac.Firma.Naziv;
            }

            return potrosac.Tip.ToString();
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

        private static void ValidateSaveDto(MerenjeSaveDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            Require(dto.SerijskiBroj, "Serijski broj brojila je obavezan.");

            if (dto.DatumVremeMerenja == default(DateTime))
            {
                throw new ArgumentException("Datum i vreme merenja su obavezni.");
            }

            RequireDecimal(dto.PotrosnjaAktivna, "Aktivna potrosnja je obavezna.");
            RequireDecimal(dto.PotrosnjaReaktivna, "Reaktivna potrosnja je obavezna.");
            RequireDecimal(dto.Snaga, "Snaga je obavezna.");
            RequireDecimal(dto.Napon, "Napon je obavezan.");
            RequireDecimal(dto.Struja, "Struja je obavezna.");
            Require(dto.TipMerenja, "Tip merenja je obavezan.");
            ParseTipIzvora(dto.TipIzvora);
            ParseDaNe(dto.IsValidirano);
            Require(dto.Komentar, "Komentar je obavezan.");
        }

        private static TipIzvoraMerenja ParseTipIzvora(string value)
        {
            var text = Require(value, "Izvor podataka je obavezan.");
            TipIzvoraMerenja result;

            if (!Enum.TryParse(text, out result))
            {
                throw new ArgumentException("Izvor podataka nije ispravan.");
            }

            return result;
        }

        private static DaNe ParseDaNe(string value)
        {
            var text = Require(value, "Validacija merenja je obavezna.");

            if (string.Equals(text, "DA", StringComparison.OrdinalIgnoreCase)
                || string.Equals(text, "D", StringComparison.OrdinalIgnoreCase))
            {
                return DaNe.D;
            }

            if (string.Equals(text, "NE", StringComparison.OrdinalIgnoreCase)
                || string.Equals(text, "N", StringComparison.OrdinalIgnoreCase))
            {
                return DaNe.N;
            }

            throw new ArgumentException("Validacija merenja nije ispravna.");
        }

        private static string ToDaNeText(DaNe value)
        {
            return value == DaNe.D ? "DA" : "NE";
        }

        private static decimal RequireDecimal(decimal? value, string message)
        {
            if (!value.HasValue)
            {
                throw new ArgumentException(message);
            }

            return value.Value;
        }

        private static string Require(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message);
            }

            return value.Trim();
        }
    }
}
