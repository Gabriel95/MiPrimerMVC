using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Domain.Entities;
using Domain.Services;
using MiPrimerMVC.Models;
using Ninject;

namespace MiPrimerMVC.Controllers
{
    public class CalculadoraRomanaController : Controller
    {
        readonly ICalculadora _calc;
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;

        public CalculadoraRomanaController(ICalculadora calc,IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _calc = calc;
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        //
        // GET: /Calculadora/
        public ActionResult Index()
        {
            var calculadora = new Calculadora
            {
                Operaciones = _readOnlyRepository.GetAll<Operaciones>().ToList()
            };
            return View(calculadora);
        }


        [HttpPost]
        public ActionResult Index(Calculadora calculadora)
        {
            
            var operacion = new Operaciones("1");

            if (calculadora.Entrada.Contains("-"))
            {
                operacion.Operador = "-";
                operacion.Numero1 = calculadora.Entrada.Split('-')[0];
                operacion.Numero2 = calculadora.Entrada.Split('-')[1];
                operacion.Resultado = _calc.Resta(calculadora.Entrada);
            }
            else if (calculadora.Entrada.Contains("+"))
            {
                operacion.Operador = "+";
                operacion.Numero1 = calculadora.Entrada.Split('+')[0];
                operacion.Numero2 = calculadora.Entrada.Split('+')[1];
                operacion.Resultado = _calc.Suma(calculadora.Entrada);
            }

            calculadora.Salida = operacion.Resultado;

            var createdOperation = _writeOnlyRepository.Create(operacion);
            calculadora.Operaciones = _readOnlyRepository.GetAll<Operaciones>().ToList();

            return View(calculadora);
        }
        

        //GET: /CalculadoraRomana/Detalle/{id}
        public ActionResult Detalle(long id)
        {
            var operacion = _readOnlyRepository.GetById<Operaciones>(id);
            return View(operacion);
        }

    }

    public class CalculadoraRomana : ICalculadora
    {
        // I + II
        public string Suma(string value)
        {
            return "III";
        }

        // VII - II
        public string Resta(string value)
        {
            return "V";
        }
    }

    public class CalculadoraNormal : ICalculadora
    {
        public string Suma(string value)
        {
            return "3";
        }

        public string Resta(string value)
        {
            return "5";
        }
    }

    public interface ICalculadora
    {
        string Suma(string value);
        string Resta(string value);
    }
}