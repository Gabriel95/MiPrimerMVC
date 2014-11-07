namespace Domain.Entities
{
    public class Operaciones : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool Archived { get; protected set; }

        public virtual string Numero1 { get; set; }
        public virtual string Operador { get; set; }
        public virtual string Numero2 { get; set; }
        public virtual string Resultado { get; set; }

        protected Operaciones()
        {
            Archived = false;
        }

        public Operaciones(string numero1)
        {
            Numero1 = numero1;
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

        public virtual void ChangeNumero1(string numero1)
        {
            Numero1 = numero1;
        }
    }
}