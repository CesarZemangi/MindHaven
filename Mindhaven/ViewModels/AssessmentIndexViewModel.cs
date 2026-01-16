using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mindhaven.Models;

namespace Mindhaven.ViewModels
{
    public class AssessmentIndexViewModel
    {
        public int AssessmentID { get; set; }
        public virtual User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Score { get; set; }
        public DateTime? TakenAt { get; set; }
    }
}