using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mindhaven.Models
{
    public class TakeAssessmentViewModel
    {
        public int UserID { get; set; }
        public Assessment Assessment { get; set; }
        public List<AssessmentQuestion> Questions { get; set; }
    }
}
