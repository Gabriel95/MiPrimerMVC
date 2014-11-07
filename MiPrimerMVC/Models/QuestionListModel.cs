using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace MiPrimerMVC.Models
{
    public class QuestionListModel
    {
        public List<QandA> ListQa { get; set; }

        public QuestionModel NewQuestion { get; set; }
    }
}