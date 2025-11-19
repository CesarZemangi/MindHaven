using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mindhaven.Models;

namespace Mindhaven.ViewModels
{
    public class HomeworkDashboardViewModel
    {
      
            public List<TherapeuticHomework> Homeworks { get; set; }
            public List<Feedback> Feedbacks { get; set; }
        }
    }

