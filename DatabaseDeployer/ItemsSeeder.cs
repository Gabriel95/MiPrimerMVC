using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using NHibernate;
using NHibernate.Mapping;

namespace DatabaseDeployer
{
    class ItemsSeeder : IDataSeeder
    {
         readonly ISession _session;

         public ItemsSeeder(ISession session)
        {
            _session = session;
        }
        public void Seed()
        {
            var item = new Items
            {
                Category = "Music",
                BusinessType = "For Sale",
                Description = "Fender Jim Root Jazzmaster with EMG81 and Emg 60",
                 Price = "1500",
                Title = "Fender Jim Root Jazzmaster",
                VideoUrl = "https://www.youtube.com/watch?v=pDLiifZPmRQ",
                ImagesUrl = "http://assets.fender.com/frl/97b5ec1524c0a7ae3fe8241799a1ee0e/generated/d9a5d5e7fcbcda8c0c49cbbb8a859220.png",
                UserId = 1,
                Date = DateTime.Now,
                Views = 0


            };
            _session.Save(item);
        }
    }
}
