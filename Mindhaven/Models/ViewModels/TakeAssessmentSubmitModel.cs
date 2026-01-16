using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mindhaven.Models
{
    public class TakeAssessmentSubmitModel
    {
        public int AssessmentID { get; set; }
        public List<AnswerItem> Answers { get; set; }
    }

    public class AnswerItem
    {
        public int QuestionID { get; set; }
        public int ScoreValue { get; set; }
    }
}
