using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Domain.Entities
{
   public class Items : IEntity
    {
        public virtual long Id { get; set; }

        public virtual bool Archived { get; protected set; }

        public virtual string Category { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual string Price { get; set; }

        public virtual string BusinessType { get; set; }

        public virtual string VideoUrl { get; set; }

        public virtual string ImagesUrl { get; set; }

       public virtual long UserId { get; set; }

       public virtual DateTime Date { get; set; }

       public virtual int Views { get; set; }

       public Items()
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
