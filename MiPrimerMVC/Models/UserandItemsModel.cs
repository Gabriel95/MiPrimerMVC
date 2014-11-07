using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace MiPrimerMVC.Models
{
    public class UserandItemsModel
    {
        public User cUser { get; set; }

        public List<Items> Itemses { get; set; }

        public MessageModel Mes { get; set; }

        public Items JustItem { get; set; }

        public AdvancedSearchModel Asearch { get; set; }
    }
}