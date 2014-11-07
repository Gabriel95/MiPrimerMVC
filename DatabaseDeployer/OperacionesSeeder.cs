using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using NHibernate;

namespace DatabaseDeployer
{
    class OperacionesSeeder : IDataSeeder
    {
        readonly ISession _session;

        public OperacionesSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            var operaciones = Builder<Operaciones>.CreateListOfSize(5).Build();
            foreach (var operacion in operaciones)
            {
                _session.Save(operacion);
            }
        }
    }
}