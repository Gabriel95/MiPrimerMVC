using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using NHibernate;

namespace DatabaseDeployer
{
    class MessageSeeder : IDataSeeder
    {
         readonly ISession _session;

        public MessageSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            var operaciones = new Messages
            {
               Froms = "rammfire490@hotmail.com",

               Tos = 1,

               Date = DateTime.Now,

               Message = "I would like to buy the Jim Root Jazzmaster"

            };

            _session.Save(operaciones);
        }
    }
}
