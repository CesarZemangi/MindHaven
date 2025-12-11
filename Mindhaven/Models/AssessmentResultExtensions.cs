using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mindhaven.Models
{
    public partial class AssessmentResult
    {
        [NotMapped]
        public Dictionary<int, object> ParsedAnswers { get; set; }
    }
}
