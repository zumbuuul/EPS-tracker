using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
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
        private const int SqlCommandTimeoutSeconds = 10;

        private readonly NHibernateSessionFactoryProvider sessionFactoryProvider;

        public PotrosacService(NHibernateSessionFactoryProvider sessionFactoryProvider)
        {
            this.sessionFactoryProvider = sessionFactoryProvider
                ?? throw new ArgumentNullException(nameof(sessionFactoryProvider));
        }

        public async Task<IList<PotrosacListDto>> VratiPotrosace()
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var query = session.Query<Potrosac>()
                    .GroupJoin(
                        session.Query<Domacinstvo>(),
                        potrosac => potrosac.Id,
                        domacinstvo => domacinstvo.Id,
                        (potrosac, domacinstva) => new
                        {
                            Potrosac = potrosac,
                            Domacinstva = domacinstva
                        })
                    .SelectMany(
                        x => x.Domacinstva.DefaultIfEmpty(),
                        (x, domacinstvo) => new
                        {
                            x.Potrosac,
                            Domacinstvo = domacinstvo
                        })
                    .GroupJoin(
                        session.Query<Firma>(),
                        x => x.Potrosac.Id,
                        firma => firma.Id,
                        (x, firme) => new
                        {
                            x.Potrosac,
                            x.Domacinstvo,
                            Firme = firme
                        })
                    .SelectMany(
                        x => x.Firme.DefaultIfEmpty(),
                        (x, firma) => new
                        {
                            Potrosac = x.Potrosac,
                            x.Potrosac.Id,
                            x.Potrosac.Tip,
                            x.Potrosac.Grad,
                            x.Potrosac.Telefon,
                            x.Potrosac.Email,
                            x.Potrosac.Status,
                            x.Potrosac.KategorijaTarife,
                            Ime = x.Domacinstvo.Ime,
                            Prezime = x.Domacinstvo.Prezime,
                            Naziv = firma.Naziv
                        })
                    .OrderBy(x => x.Id);

                var rows = await query.ToListAsync().ConfigureAwait(false);

                return rows.Select(row => new PotrosacListDto
                {
                    Id = row.Id,
                    Tip = row.Tip.ToString(),
                    ImeIliNaziv = BuildImeIliNaziv(
                        row.Tip,
                        row.Ime,
                        row.Prezime,
                        row.Naziv),
                    Grad = row.Grad,
                    Telefon = row.Telefon,
                    Email = row.Email,
                    Status = row.Status,
                    KategorijaTarife = row.KategorijaTarife,
                    BrojBrojila = row.Potrosac.VezeBrojila.Count(IsAktivnaVeza)
                }).ToList();
            }
        }

        public async Task<PotrosacDto> VratiPotrosaca(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var potrosac = await GetRequiredPotrosac(session, id).ConfigureAwait(false);
                return MapToDto(potrosac);
            }
        }

        public async Task<IList<BrojiloListDto>> VratiBrojilaZaPotrosaca(long potrosacId)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            {
                var potrosac = await GetRequiredPotrosac(session, potrosacId).ConfigureAwait(false);

                return potrosac.Brojila
                    .OrderBy(x => x.SerijskiBroj)
                    .Select(MapToBrojiloListDto)
                    .ToList();
            }
        }

        public async Task<long> SacuvajPotrosaca(PotrosacSaveDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.Id == 0)
            {
                return await DodajPotrosaca(dto).ConfigureAwait(false);
            }

            await IzmeniPotrosaca(dto).ConfigureAwait(false);
            return dto.Id;
        }

        public async Task<long> DodajPotrosaca(PotrosacSaveDto dto)
        {
            ValidateSaveDto(dto);

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = new Potrosac();
                ApplyScalarFields(potrosac, dto);
                await ApplyDetails(session, potrosac, dto).ConfigureAwait(false);

                await session.SaveAsync(potrosac).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return potrosac.Id;
            }
        }

        public async Task IzmeniPotrosaca(PotrosacSaveDto dto)
        {
            ValidateSaveDto(dto);

            if (dto.Id <= 0)
            {
                throw new ArgumentException("Id potrosaca je obavezan za izmenu.", nameof(dto));
            }

            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = await GetRequiredPotrosac(session, dto.Id).ConfigureAwait(false);

                ApplyScalarFields(potrosac, dto);
                await ApplyDetails(session, potrosac, dto).ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task ObrisiPotrosaca(long id)
        {
            using (var session = sessionFactoryProvider.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var potrosac = await GetRequiredPotrosac(session, id).ConfigureAwait(false);

                if (potrosac.Racuni.Count > 0)
                {
                    throw new InvalidOperationException("Potrosac ima racune i ne moze biti obrisan.");
                }

                if (potrosac.Kvarovi.Count > 0)
                {
                    throw new InvalidOperationException("Potrosac ima evidentirane kvarove i ne moze biti obrisan.");
                }

                foreach (var veza in potrosac.VezeBrojila.ToList())
                {
                    await session.DeleteAsync(veza).ConfigureAwait(false);
                }

                await session.DeleteAsync(potrosac).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task PoveziPotrosacaIBrojilo(PotrosacBrojiloLinkDto dto)
        {
            DebugLog("PoveziPotrosacaIBrojilo(dto) START");

            if (dto == null)
            {
                DebugLog("PoveziPotrosacaIBrojilo(dto) dto is null");
                throw new ArgumentNullException(nameof(dto));
            }

            DebugLog(
                "Povezi DTO potrosacId="
                + dto.PotrosacId.ToString(CultureInfo.InvariantCulture)
                + ", serijskiBroj="
                + Safe(dto.SerijskiBroj)
                + ", datumOd="
                + FormatDate(dto.DatumOd)
                + ", datumDo="
                + FormatDate(dto.DatumDo));

            await UpisiVezuPotrosacBrojilo(
                dto.PotrosacId,
                dto.SerijskiBroj,
                dto.DatumOd,
                dto.DatumDo).ConfigureAwait(false);

            DebugLog("PoveziPotrosacaIBrojilo(dto) END");
        }

        private async Task UpisiVezuPotrosacBrojilo(
            long potrosacId,
            string serijskiBroj,
            DateTime? datumOd,
            DateTime? datumDo)
        {
            DebugLog(
                "UpisiVezu START potrosacId="
                + potrosacId.ToString(CultureInfo.InvariantCulture)
                + ", serijskiBroj="
                + Safe(serijskiBroj));

            ValidateSerijskiBroj(serijskiBroj);
            ValidateDatumVeze(datumOd, datumDo);
            var normalizedSerijskiBroj = serijskiBroj.Trim();
            DebugLog("UpisiVezu normalizedSerijskiBroj=" + normalizedSerijskiBroj);

            DebugLog("UpisiVezu OpenSession BEFORE");
            using (var session = sessionFactoryProvider.OpenSession())
            {
                DebugLog("UpisiVezu OpenSession AFTER");

                DebugLog("UpisiVezu GetRequiredPotrosac BEFORE");
                await GetRequiredPotrosac(session, potrosacId).ConfigureAwait(false);
                DebugLog("UpisiVezu GetRequiredPotrosac AFTER");

                DebugLog("UpisiVezu GetRequiredBrojilo BEFORE");
                await GetRequiredBrojilo(session, normalizedSerijskiBroj).ConfigureAwait(false);
                DebugLog("UpisiVezu GetRequiredBrojilo AFTER");

                DebugLog("UpisiVezu ActiveVezaExists BEFORE");
                if (await ActiveVezaExists(session, potrosacId, normalizedSerijskiBroj).ConfigureAwait(false))
                {
                    DebugLog("UpisiVezu ActiveVezaExists AFTER result=true");
                    throw new InvalidOperationException("Potrosac je vec aktivno povezan sa tim brojilom.");
                }
                DebugLog("UpisiVezu ActiveVezaExists AFTER result=false");

                var resolvedDatumOd = datumOd ?? DateTime.Today;

                DebugLog("UpisiVezu BeginTransaction BEFORE");
                using (var transaction = session.BeginTransaction())
                {
                    DebugLog("UpisiVezu BeginTransaction AFTER");
                    int inserted;

                    if (datumDo.HasValue)
                    {
                        DebugLog("UpisiVezu INSERT with DATUM_DO BEFORE");
                        inserted = await session.CreateSQLQuery(@"
                                insert into POTROSAC_BROJILO_VEZA
                                    (POTROSAC_ID, SERIJSKI_BROJ, DATUM_OD, DATUM_DO)
                                values
                                    (:potrosacId, :serijskiBroj, :datumOd, :datumDo)")
                            .SetTimeout(SqlCommandTimeoutSeconds)
                            .SetInt64("potrosacId", potrosacId)
                            .SetString("serijskiBroj", normalizedSerijskiBroj)
                            .SetDateTime("datumOd", resolvedDatumOd)
                            .SetDateTime("datumDo", datumDo.Value)
                            .ExecuteUpdateAsync()
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        DebugLog("UpisiVezu INSERT without DATUM_DO BEFORE");
                        inserted = await session.CreateSQLQuery(@"
                                insert into POTROSAC_BROJILO_VEZA
                                    (POTROSAC_ID, SERIJSKI_BROJ, DATUM_OD)
                                values
                                    (:potrosacId, :serijskiBroj, :datumOd)")
                            .SetTimeout(SqlCommandTimeoutSeconds)
                            .SetInt64("potrosacId", potrosacId)
                            .SetString("serijskiBroj", normalizedSerijskiBroj)
                            .SetDateTime("datumOd", resolvedDatumOd)
                            .ExecuteUpdateAsync()
                            .ConfigureAwait(false);
                    }

                    DebugLog("UpisiVezu INSERT AFTER inserted=" + inserted.ToString(CultureInfo.InvariantCulture));

                    if (inserted != 1)
                    {
                        DebugLog("UpisiVezu inserted count invalid");
                        throw new InvalidOperationException("Veza nije upisana u bazu.");
                    }

                    DebugLog("UpisiVezu Flush BEFORE");
                    await session.FlushAsync().ConfigureAwait(false);
                    DebugLog("UpisiVezu Flush AFTER");

                    DebugLog("UpisiVezu verify ActiveVezaExists BEFORE");
                    if (!await ActiveVezaExists(session, potrosacId, normalizedSerijskiBroj).ConfigureAwait(false))
                    {
                        DebugLog("UpisiVezu verify ActiveVezaExists AFTER result=false");
                        throw new InvalidOperationException("Veza nije pronadjena u bazi nakon upisa.");
                    }
                    DebugLog("UpisiVezu verify ActiveVezaExists AFTER result=true");

                    DebugLog("UpisiVezu Commit BEFORE");
                    await transaction.CommitAsync().ConfigureAwait(false);
                    DebugLog("UpisiVezu Commit AFTER");
                }
            }

            DebugLog("UpisiVezu END");
        }

        public async Task RaskiniVezuPotrosacBrojilo(PotrosacBrojiloLinkDto dto)
        {
            DebugLog("RaskiniVezuPotrosacBrojilo(dto) START");

            if (dto == null)
            {
                DebugLog("RaskiniVezuPotrosacBrojilo(dto) dto is null");
                throw new ArgumentNullException(nameof(dto));
            }

            DebugLog(
                "Raskini DTO potrosacId="
                + dto.PotrosacId.ToString(CultureInfo.InvariantCulture)
                + ", serijskiBroj="
                + Safe(dto.SerijskiBroj)
                + ", datumDo="
                + FormatDate(dto.DatumDo));

            await ZatvoriVezuPotrosacBrojilo(dto.PotrosacId, dto.SerijskiBroj, dto.DatumDo).ConfigureAwait(false);

            DebugLog("RaskiniVezuPotrosacBrojilo(dto) END");
        }

        private async Task ZatvoriVezuPotrosacBrojilo(long potrosacId, string serijskiBroj, DateTime? datumDo)
        {
            DebugLog(
                "ZatvoriVezu START potrosacId="
                + potrosacId.ToString(CultureInfo.InvariantCulture)
                + ", serijskiBroj="
                + Safe(serijskiBroj));

            ValidateSerijskiBroj(serijskiBroj);
            var normalizedSerijskiBroj = serijskiBroj.Trim();
            DebugLog("ZatvoriVezu normalizedSerijskiBroj=" + normalizedSerijskiBroj);

            DebugLog("ZatvoriVezu OpenSession BEFORE");
            using (var session = sessionFactoryProvider.OpenSession())
            {
                DebugLog("ZatvoriVezu OpenSession AFTER");

                DebugLog("ZatvoriVezu GetRequiredPotrosac BEFORE");
                await GetRequiredPotrosac(session, potrosacId).ConfigureAwait(false);
                DebugLog("ZatvoriVezu GetRequiredPotrosac AFTER");

                DebugLog("ZatvoriVezu GetRequiredBrojilo BEFORE");
                await GetRequiredBrojilo(session, normalizedSerijskiBroj).ConfigureAwait(false);
                DebugLog("ZatvoriVezu GetRequiredBrojilo AFTER");

                var resolvedDatumDo = datumDo ?? DateTime.Today;

                DebugLog("ZatvoriVezu BeginTransaction BEFORE");
                using (var transaction = session.BeginTransaction())
                {
                    DebugLog("ZatvoriVezu BeginTransaction AFTER");
                    DebugLog("ZatvoriVezu UPDATE BEFORE");
                    var updated = await session.CreateSQLQuery(@"
                            update POTROSAC_BROJILO_VEZA
                            set DATUM_DO = :datumDo
                            where POTROSAC_ID = :potrosacId
                                and SERIJSKI_BROJ = :serijskiBroj
                                and DATUM_DO is null
                                and DATUM_OD <= :datumDo")
                        .SetTimeout(SqlCommandTimeoutSeconds)
                        .SetDateTime("datumDo", resolvedDatumDo)
                        .SetInt64("potrosacId", potrosacId)
                        .SetString("serijskiBroj", normalizedSerijskiBroj)
                        .ExecuteUpdateAsync()
                        .ConfigureAwait(false);
                    DebugLog("ZatvoriVezu UPDATE AFTER updated=" + updated.ToString(CultureInfo.InvariantCulture));

                    if (updated == 0)
                    {
                        DebugLog("ZatvoriVezu updated=0, ActiveVezaExists BEFORE");
                        if (await ActiveVezaExists(session, potrosacId, normalizedSerijskiBroj).ConfigureAwait(false))
                        {
                            DebugLog("ZatvoriVezu ActiveVezaExists AFTER result=true");
                            throw new ArgumentException("Datum kraja veze ne moze biti pre datuma pocetka veze.");
                        }

                        DebugLog("ZatvoriVezu ActiveVezaExists AFTER result=false");
                        throw new InvalidOperationException("Potrosac nije povezan sa tim brojilom.");
                    }

                    DebugLog("ZatvoriVezu Flush BEFORE");
                    await session.FlushAsync().ConfigureAwait(false);
                    DebugLog("ZatvoriVezu Flush AFTER");

                    DebugLog("ZatvoriVezu Commit BEFORE");
                    await transaction.CommitAsync().ConfigureAwait(false);
                    DebugLog("ZatvoriVezu Commit AFTER");
                }
            }

            DebugLog("ZatvoriVezu END");
        }

        private static void ApplyScalarFields(Potrosac potrosac, PotrosacSaveDto dto)
        {
            potrosac.Tip = ParsePotrosacTip(dto.Tip);
            potrosac.Email = Normalize(dto.Email);
            potrosac.Telefon = Normalize(dto.Telefon);
            potrosac.Adresa = dto.Adresa.Trim();
            potrosac.Grad = dto.Grad.Trim();
            potrosac.Komentar = Normalize(dto.Komentar);
            potrosac.Status = dto.Status.Trim();
            potrosac.KategorijaTarife = dto.KategorijaTarife.Trim();
        }

        private static async Task ApplyDetails(ISession session, Potrosac potrosac, PotrosacSaveDto dto)
        {
            if (potrosac.Tip == PotrosacTip.DOMACINSTVO)
            {
                await RemoveFirma(session, potrosac).ConfigureAwait(false);
                ApplyDomacinstvo(potrosac, dto.Domacinstvo);
                return;
            }

            if (potrosac.Tip == PotrosacTip.FIRMA)
            {
                await RemoveDomacinstvo(session, potrosac).ConfigureAwait(false);
                ApplyFirma(potrosac, dto.Firma);
                return;
            }

            await RemoveDomacinstvo(session, potrosac).ConfigureAwait(false);
            await RemoveFirma(session, potrosac).ConfigureAwait(false);
        }

        private static void ApplyDomacinstvo(Potrosac potrosac, DomacinstvoDto dto)
        {
            if (potrosac.Domacinstvo == null)
            {
                potrosac.Domacinstvo = new Domacinstvo
                {
                    Potrosac = potrosac
                };
            }

            potrosac.Domacinstvo.Jmbg = dto.Jmbg.Trim();
            potrosac.Domacinstvo.Ime = dto.Ime.Trim();
            potrosac.Domacinstvo.Prezime = dto.Prezime.Trim();
        }

        private static void ApplyFirma(Potrosac potrosac, FirmaDto dto)
        {
            if (potrosac.Firma == null)
            {
                potrosac.Firma = new Firma
                {
                    Potrosac = potrosac
                };
            }

            potrosac.Firma.Naziv = dto.Naziv.Trim();
            potrosac.Firma.Pib = dto.Pib.Trim();
        }

        private static async Task RemoveDomacinstvo(ISession session, Potrosac potrosac)
        {
            if (potrosac.Domacinstvo == null)
            {
                return;
            }

            await session.DeleteAsync(potrosac.Domacinstvo).ConfigureAwait(false);
            potrosac.Domacinstvo = null;
        }

        private static async Task RemoveFirma(ISession session, Potrosac potrosac)
        {
            if (potrosac.Firma == null)
            {
                return;
            }

            await session.DeleteAsync(potrosac.Firma).ConfigureAwait(false);
            potrosac.Firma = null;
        }

        private static PotrosacDto MapToDto(Potrosac potrosac)
        {
            return new PotrosacDto
            {
                Id = potrosac.Id,
                Tip = potrosac.Tip.ToString(),
                Email = potrosac.Email,
                Telefon = potrosac.Telefon,
                Adresa = potrosac.Adresa,
                Grad = potrosac.Grad,
                Komentar = potrosac.Komentar,
                Status = potrosac.Status,
                KategorijaTarife = potrosac.KategorijaTarife,
                Domacinstvo = MapDomacinstvo(potrosac.Domacinstvo),
                Firma = MapFirma(potrosac.Firma),
                Brojila = potrosac.Brojila
                    .Select(MapToBrojiloListDto)
                    .ToList(),
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

        private static RacunListDto MapToRacunListDto(Racun racun, Potrosac potrosac)
        {
            return new RacunListDto
            {
                BrojRacuna = racun.BrojRacuna,
                PotrosacId = potrosac.Id,
                ImeIliNazivPotrosaca = BuildImeIliNaziv(potrosac),
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

        private static string BuildImeIliNaziv(PotrosacTip tip, string ime, string prezime, string naziv)
        {
            if (tip == PotrosacTip.DOMACINSTVO)
            {
                return ((ime ?? string.Empty) + " " + (prezime ?? string.Empty)).Trim();
            }

            if (tip == PotrosacTip.FIRMA)
            {
                return naziv;
            }

            return tip.ToString();
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

        private static async Task<Brojilo> GetRequiredBrojilo(ISession session, string serijskiBroj)
        {
            var brojilo = await session.GetAsync<Brojilo>(serijskiBroj).ConfigureAwait(false);

            if (brojilo == null)
            {
                throw new KeyNotFoundException("Brojilo nije pronadjeno.");
            }

            return brojilo;
        }

        private static async Task<bool> ActiveVezaExists(ISession session, long potrosacId, string serijskiBroj)
        {
            DebugLog(
                "ActiveVezaExists BEFORE potrosacId="
                + potrosacId.ToString(CultureInfo.InvariantCulture)
                + ", serijskiBroj="
                + Safe(serijskiBroj));

            var result = await session.CreateSQLQuery(@"
                    select count(*)
                    from POTROSAC_BROJILO_VEZA
                    where POTROSAC_ID = :potrosacId
                        and SERIJSKI_BROJ = :serijskiBroj
                        and DATUM_DO is null")
                .SetTimeout(SqlCommandTimeoutSeconds)
                .SetInt64("potrosacId", potrosacId)
                .SetString("serijskiBroj", serijskiBroj)
                .UniqueResultAsync()
                .ConfigureAwait(false);

            var exists = Convert.ToInt64(result, CultureInfo.InvariantCulture) > 0;
            DebugLog("ActiveVezaExists AFTER exists=" + exists.ToString(CultureInfo.InvariantCulture));
            return exists;
        }

        private static void ValidateSaveDto(PotrosacSaveDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var tip = ParsePotrosacTip(dto.Tip);
            Require(dto.Adresa, "Adresa je obavezna.");
            Require(dto.Grad, "Grad je obavezan.");
            Require(dto.Status, "Status potrosaca je obavezan.");
            Require(dto.KategorijaTarife, "Kategorija tarife je obavezna.");
            ValidatePotrosacDetails(dto, tip);

            if (!string.IsNullOrWhiteSpace(dto.Telefon) && !IsDigitsOnly(dto.Telefon.Trim()))
            {
                throw new ArgumentException("Telefon sme da sadrzi samo cifre.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email.Trim()))
            {
                throw new ArgumentException("Email mora biti u ispravnom formatu.");
            }
        }

        private static void ValidatePotrosacDetails(PotrosacSaveDto dto, PotrosacTip tip)
        {
            if (tip == PotrosacTip.DOMACINSTVO)
            {
                ValidateDomacinstvo(dto.Domacinstvo);
                return;
            }

            if (tip == PotrosacTip.FIRMA)
            {
                ValidateFirma(dto.Firma);
            }
        }

        private static void ValidateDomacinstvo(DomacinstvoDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Podaci o domacinstvu su obavezni za tip DOMACINSTVO.");
            }

            Require(dto.Ime, "Ime je obavezno.");
            Require(dto.Prezime, "Prezime je obavezno.");
            RequireDigits(dto.Jmbg, 13, "JMBG mora imati tacno 13 cifara.");
        }

        private static void ValidateFirma(FirmaDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Podaci o firmi su obavezni za tip FIRMA.");
            }

            Require(dto.Naziv, "Naziv firme je obavezan.");
            RequireDigits(dto.Pib, 9, "PIB mora imati tacno 9 cifara.");
        }

        private static void ValidateSerijskiBroj(string serijskiBroj)
        {
            Require(serijskiBroj, "Serijski broj je obavezan.");
        }

        private static void ValidateDatumVeze(DateTime? datumOd, DateTime? datumDo)
        {
            if (datumOd.HasValue && datumDo.HasValue && datumDo.Value < datumOd.Value)
            {
                throw new ArgumentException("Datum kraja veze ne moze biti pre datuma pocetka veze.");
            }
        }

        private static bool IsAktivnaVeza(PotrosacBrojiloVeza veza)
        {
            return veza != null && !veza.DatumDo.HasValue;
        }

        private static bool IsAktivnaVezaZaBrojilo(PotrosacBrojiloVeza veza, string serijskiBroj)
        {
            return IsAktivnaVeza(veza)
                && veza.Brojilo != null
                && IsSameSerijskiBroj(veza.Brojilo.SerijskiBroj, serijskiBroj);
        }

        private static PotrosacTip ParsePotrosacTip(string value)
        {
            var text = Require(value, "Tip potrosaca je obavezan.");
            PotrosacTip tip;

            if (!Enum.TryParse(text, out tip))
            {
                throw new ArgumentException("Tip potrosaca nije ispravan.");
            }

            return tip;
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

        private static void DebugLog(string message)
        {
            Console.WriteLine(
                "[EPS DEBUG] "
                + DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)
                + " [T"
                + Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)
                + "] PotrosacService: "
                + message);
        }

        private static string Safe(string value)
        {
            return value ?? "<null>";
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                : "<null>";
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
