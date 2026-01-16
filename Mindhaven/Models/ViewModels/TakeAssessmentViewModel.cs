using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Mindhaven.ViewModels
{
    public class TakeAssessmentViewModel
    {
        public int AssessmentID { get; set; }
        public string Title { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
    }

    public class QuestionViewModel
    {
        public int QuestionID { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }

        public List<OptionViewModel> Options { get; set; }
    }

    public class OptionViewModel
    {
        public int OptionID { get; set; }
        public string Text { get; set; }
    }

    public class AnswerViewModel
    {
        public int QuestionID { get; set; }
        public int SelectedOptionID { get; set; }
    }
}
