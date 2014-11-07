using System;
using System.Collections.Generic;
using System.Threading;
using AcklenAvenue.Data.NHibernate;
using Data;
using DomainDrivenDatabaseDeployer;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace DatabaseDeployer
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConnectionStrings.Get();

            MsSqlConfiguration databaseConfiguration = MsSqlConfiguration.MsSql2008.ShowSql().
                ConnectionString(x => x.Is(connectionString));

            DomainDrivenDatabaseDeployer.DatabaseDeployer dd = null;
            ISessionFactory sessionFactory = new SessionFactoryBuilder(new MappingScheme(), databaseConfiguration)
                .Build(cfg => { dd = new DomainDrivenDatabaseDeployer.DatabaseDeployer(cfg); });

            dd.Drop();
            Console.WriteLine("Database dropped.");
            Thread.Sleep(1000);

            dd.Create();
            Console.WriteLine("Database created.");

            ISession session = sessionFactory.OpenSession();
            using (ITransaction tx = session.BeginTransaction())
            {
                dd.Seed(new List<IDataSeeder>
                {
                    new OperacionesSeeder(session),
                    new ObjectSeeder(session),
                    new UserSeeder(session),
                    new QandASeeder(session),
                    new ItemsSeeder(session),
                    new MessageSeeder(session)
                });
                tx.Commit();
            }
            session.Close();
            sessionFactory.Close();
            Console.WriteLine("Seed data added.");
            Thread.Sleep(2000);
        }
    }

    public class ObjectSeeder : IDataSeeder
    {
        readonly ISession _session;

        public ObjectSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            
        }
    }
}