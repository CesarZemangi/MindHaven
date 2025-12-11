using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mindhaven.Models
{
    public partial class AssessmentQuestion
    {
        [NotMapped]
        public List<string> ParsedOptions { get; set; }
    }
}
