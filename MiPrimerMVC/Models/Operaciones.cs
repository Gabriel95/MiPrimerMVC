using System;

namespace MiPrimerMVC.Models
{
    public class OperacionesModel
    {
        public Guid Id { get; set; }
        public string Numero1 { get; set; }
        public string Numero2 { get; set; }
        public string Operador { get; set; }
        public string Resultado { get; set; }
    }
}