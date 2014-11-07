using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiPrimerMVC.Models
{
    public class RegisterModel
    {
        public string Name { get; set; }

        public string Email{ get; set; }

        public string Password { get; set; }

        public string RePassword { get; set; }
    }
}