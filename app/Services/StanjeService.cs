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
    public class StanjeService
    {
        private readonly NHibernateSessionFactoryProvider sessionFactoryProvider;

        public StanjeService(NHibernateSessionFactoryProvider sessionFactoryProvider)
        {
            this.sessionFactoryProvider = sessionFactoryProvider
                ?? throw new ArgumentNullException(nameof(sessionFactoryProvider));
        }

        public async Task<IList<StanjeListDto>> VratiStanja()
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var stanja = await session.Query<Stanje>()
                    .OrderByDescending(x => x.DatumIVreme)
                    .ThenByDescending(x => x.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return stanja.Select(MapToListDto).ToList();
            }
        }

        public async Task<StanjeDto> VratiStanje(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var stanje = await GetRequiredStanje(session, id).ConfigureAwait(false);
                return MapToDto(stanje);
            }
        }

        public async Task<long> DodajStanje(StanjeSaveDto dto)
        {
            ValidateSaveDto(dto, false);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var stanje = new Stanje();
                ApplyFields(stanje, dto);

                await session.SaveAsync(stanje).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return stanje.Id;
            }
        }

        public async Task IzmeniStanje(StanjeSaveDto dto)
        {
            ValidateSaveDto(dto, true);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var stanje = await GetRequiredStanje(session, dto.Id).ConfigureAwait(false);

                ApplyFields(stanje, dto);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task ObrisiStanje(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var stanje = await GetRequiredStanje(session, id).ConfigureAwait(false);

                await session.DeleteAsync(stanje).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        private static void ApplyFields(Stanje stanje, StanjeSaveDto dto)
        {
            stanje.DatumIVreme = dto.DatumIVreme;
            stanje.Lokacija = dto.Lokacija.Trim();
            stanje.NaponskiNivo = dto.NaponskiNivo;
            stanje.Status = dto.Status.Trim();
            stanje.Transformator = dto.Transformator.Trim();
            stanje.Komentar = Normalize(dto.Komentar);
            stanje.UkupnaPotrosnja = dto.UkupnaPotrosnja;
            stanje.Gubitak = dto.Gubitak;
            stanje.DistribuiranaEnergija = dto.DistribuiranaEnergija;
            stanje.ProizvedenaEnergija = dto.ProizvedenaEnergija;
        }

        private static StanjeListDto MapToListDto(Stanje stanje)
        {
            return new StanjeListDto
            {
                Id = stanje.Id,
                DatumIVreme = stanje.DatumIVreme,
                Lokacija = stanje.Lokacija,
                Transformator = stanje.Transformator,
                NaponskiNivo = stanje.NaponskiNivo,
                UkupnaPotrosnja = stanje.UkupnaPotrosnja,
                Gubitak = stanje.Gubitak,
                DistribuiranaEnergija = stanje.DistribuiranaEnergija,
                ProizvedenaEnergija = stanje.ProizvedenaEnergija,
                Status = stanje.Status
            };
        }

        private static StanjeDto MapToDto(Stanje stanje)
        {
            return new StanjeDto
            {
                Id = stanje.Id,
                DatumIVreme = stanje.DatumIVreme,
                Lokacija = stanje.Lokacija,
                NaponskiNivo = stanje.NaponskiNivo,
                Status = stanje.Status,
                Transformator = stanje.Transformator,
                Komentar = stanje.Komentar,
                UkupnaPotrosnja = stanje.UkupnaPotrosnja,
                Gubitak = stanje.Gubitak,
                DistribuiranaEnergija = stanje.DistribuiranaEnergija,
                ProizvedenaEnergija = stanje.ProizvedenaEnergija
            };
        }

        private static async Task<Stanje> GetRequiredStanje(ISession session, long id)
        {
            var stanje = await session.GetAsync<Stanje>(id).ConfigureAwait(false);

            if (stanje == null)
            {
                throw new KeyNotFoundException("Stanje mreze nije pronadjeno.");
            }

            return stanje;
        }

        private static void ValidateSaveDto(StanjeSaveDto dto, bool isEdit)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (isEdit && dto.Id <= 0)
            {
                throw new ArgumentException("Id stanja mreze je obavezan za izmenu.");
            }

            if (dto.DatumIVreme == default(DateTime))
            {
                throw new ArgumentException("Datum i vreme su obavezni.");
            }

            Require(dto.Lokacija, "Lokacija stanice je obavezna.");
            Require(dto.Transformator, "Transformator je obavezan.");
            Require(dto.Status, "Status mreze je obavezan.");

            ValidateNonNegative(dto.NaponskiNivo, "Naponski nivo ne moze biti negativan.");
            ValidateNonNegative(dto.ProizvedenaEnergija, "Proizvedena energija ne moze biti negativna.");
            ValidateNonNegative(dto.DistribuiranaEnergija, "Distribuirana energija ne moze biti negativna.");
            ValidateNonNegative(dto.Gubitak, "Gubici u mrezi ne mogu biti negativni.");
            ValidateNonNegative(dto.UkupnaPotrosnja, "Ukupna potrosnja ne moze biti negativna.");
        }

        private static void ValidateNonNegative(decimal value, string message)
        {
            if (value < 0)
            {
                throw new ArgumentException(message);
            }
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
