using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Services;
using NHibernate;

namespace Data
{
    public class WriteOnlyRepository : IWriteOnlyRepository
    {
        readonly ISession _session;

        public WriteOnlyRepository(ISession session)
        {
            _session = session;
        }

        public T Create<T>(T itemToCreate) where T : IEntity
        {
            _session.Save(itemToCreate);
            return itemToCreate;
        }

        public void ArchiveAll<T>(IEnumerable<T> list) where T : class, IEntity
        {
            foreach (T item in _session.QueryOver<T>().List())
            {
                Archive<T>(item.Id);
            }
        }

        public IEnumerable<T> CreateAll<T>(IEnumerable<T> list) where T : IEntity
        {
            List<T> items = list as List<T> ?? list.ToList();
            foreach (T item in items)
            {
                Create(item);
            }

            return items;
        }

        public void Archive<T>(long id) where T : IEntity
        {
            var itemToArhive = _session.Get<T>(id);
            itemToArhive.Archive();
            _session.Update(itemToArhive);
        }

        public T Update<T>(T itemToUpdate) where T : IEntity
        {
            ISession session = _session;
            session.Update(itemToUpdate);
            session.Flush();
            return itemToUpdate;
        }
    }
}