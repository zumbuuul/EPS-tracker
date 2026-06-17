using EPS_Tracker.Entiteti;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using EPS_Tracker.Entiteti;

namespace EPS_Tracker;

static class DataLayer
{
    private static ISessionFactory? factory;
    private static readonly object lockObj = new object();

    public static ISession? GetSession()
    {
        if (factory == null)
        {
            lock (lockObj)
            {
                if (factory == null)
                {
                    factory = CreateSessionFactory();
                }
            }
        }

        return factory?.OpenSession();
    }

    private static ISessionFactory? CreateSessionFactory()
    {
        try
        {
            string cs = ConfigurationManager.ConnectionStrings["OracleCS"].ConnectionString;

            var cfg = OracleManagedDataClientConfiguration.Oracle10
                .ShowSql()
                .ConnectionString(c => c.Is(cs));

            return Fluently.Configure()
                .Database(cfg)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Potrosac>())
                .BuildSessionFactory();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message + "\n\n" +
                ex.InnerException?.Message
            );

            return null;
        }
    }
}