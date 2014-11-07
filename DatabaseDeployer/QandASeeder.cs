using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using NHibernate;

namespace DatabaseDeployer
{
    class QandASeeder : IDataSeeder
    {
      readonly ISession _session;

        public QandASeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            var operacion = new QandA
            {
                Question = "Is this a web page?",
                Answer =  "Yes it is.",
                Frequency = "10",
                Date = DateTime.Today,

                Email = "jgpaz5@gmail.com"
            };
                _session.Save(operacion);
            
        }
    }
}
