using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class QandA: IEntity
    {
        public virtual long Id { get;  set;}

        public virtual string Question { get; set; }

        public virtual string Answer { get; set; }

        public virtual string Email{ get; set; }

        public virtual bool Archived { get; set; }

        public virtual string Frequency { get; set; }

        public virtual DateTime Date { get; set; }

        public QandA()
        {
            Archived = false;

            Answer = "Question not answered yet";

            Frequency = "1";
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
