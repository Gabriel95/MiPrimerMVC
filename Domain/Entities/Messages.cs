using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Messages : IEntity
    {
        public virtual long Id { get; set; }

        public virtual bool Archived { get; set; }

        public virtual string Froms { get; set; }

        public virtual long Tos { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual string Message { get; set; }


        public Messages()
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
