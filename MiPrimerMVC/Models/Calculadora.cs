using System.Collections.Generic;
using Domain.Entities;
using MiPrimerMVC.Controllers;

namespace MiPrimerMVC.Models
{
    public class Calculadora
    {
        public string Entrada { get; set; }
        public string Salida { get; set; }
        public List<Operaciones> Operaciones { get; set; } 
    }
}