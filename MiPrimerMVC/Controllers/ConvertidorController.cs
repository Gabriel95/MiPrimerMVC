using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiPrimerMVC.Controllers
{
    public class ConvertidorController : Controller
    {
        //
        // GET: /Convertidor/
        public ActionResult ArabigosARomanos()
        {
            return View();
        }

        public ActionResult RomanosAArabigos()
        {
            return View();
        }
	}
}