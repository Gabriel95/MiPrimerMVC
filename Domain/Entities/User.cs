using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : IEntity
    {
        public virtual long Id { get; set; }

        public virtual bool Archived { get; set; }

        public virtual string Name { get; set; }

        public virtual string Email { get; set; }

        public virtual string Password { get; set; }

        public virtual string Telephone { get; set; }

        public virtual string Info { get; set; }

        public User()
        {
            Archived = false;
        }

        public virtual void Archive()
        {
            Archived = true;
        }

        public virtual void Activate()
        {
            Archived = false;
        }
    }
}
