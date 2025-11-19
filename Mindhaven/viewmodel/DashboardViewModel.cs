using System;
using System.Collections.Generic;
using Mindhaven.Models;

namespace Mindhaven.ViewModel
{
    public class MindHavenDashboardViewModel
    {
        public MindHavenDashboardViewModel(
            string userName,
            List<MoodLog> moodLogs,
            List<AssessmentResult> assessmentResults,
            List<Appointment> appointments,
            List<Meditation> recommendedMeditations,
            List<Meditation> meditationList,
            List<SupportGroup> supportMessages,
            List<User> users)
        {
            UserName = userName;
            MoodLogs = moodLogs;
            AssessmentResults = assessmentResults;
            Appointments = appointments;
            RecommendedMeditations = recommendedMeditations;
            MeditationList = meditationList;
            SupportMessages = supportMessages;
            Users = users;
        }

        public string UserName { get; set; }
        public List<MoodLog> MoodLogs { get; set; }
        public List<AssessmentResult> AssessmentResults { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Meditation> RecommendedMeditations { get; set; }
        public List<Meditation> MeditationList { get; set; }
        public List<SupportGroup> SupportMessages { get; set; }
        public List<User> Users { get; set; }
    }
}
