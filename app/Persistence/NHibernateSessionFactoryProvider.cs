using System;
using app.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace app.Persistence
{
    public sealed class NHibernateSessionFactoryProvider : IDisposable
    {
        private readonly Lazy<ISessionFactory> sessionFactory;
        private bool disposed;

        public NHibernateSessionFactoryProvider(OraclePersistenceOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            sessionFactory = new Lazy<ISessionFactory>(BuildSessionFactory);
        }

        public OraclePersistenceOptions Options { get; }

        public ISession OpenSession()
        {
            ThrowIfDisposed();
            return sessionFactory.Value.OpenSession();
        }

        public IStatelessSession OpenStatelessSession()
        {
            ThrowIfDisposed();
            return sessionFactory.Value.OpenStatelessSession();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            if (sessionFactory.IsValueCreated)
            {
                sessionFactory.Value.Dispose();
            }

            disposed = true;
        }

        private ISessionFactory BuildSessionFactory()
        {
            var database = OracleManagedDataClientConfiguration.Oracle10
                .ConnectionString(Options.ConnectionString);

            if (Options.ShowSql)
            {
                database = database.ShowSql();
            }

            if (Options.FormatSql)
            {
                database = database.FormatSql();
            }

            return Fluently.Configure()
                .Database(database)
                .Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<MappingAnchor>())
                .ExposeConfiguration(configuration =>
                {
                    if (!string.IsNullOrWhiteSpace(Options.QuerySubstitutions))
                    {
                        configuration.SetProperty("query.substitutions", Options.QuerySubstitutions);
                    }

                    configuration.SetProperty(
                        "oracle.use_n_prefixed_types_for_unicode",
                        Options.UseNPrefixedTypesForUnicode ? "true" : "false");
                })
                .BuildSessionFactory();
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(NHibernateSessionFactoryProvider));
            }
        }
    }
}
