//namespace Mindhaven.Controllers
    namespace Mindhaven.ViewModel
{
    internal class DashboardViewModel
    {
        private object moodLogCount;
        private double averageMood;
        private string adminName;

        public DashboardViewModel(object moodLogCount, double averageMood, string adminName)
        {
            this.moodLogCount = moodLogCount;
            this.averageMood = averageMood;
            this.adminName = adminName;
        }

    }
}