using System.Collections.Generic;
using Mindhaven.Models; // make sure your models exist

namespace Mindhaven.ViewModels
{
    public class LandingViewModel
    {
        public IEnumerable<Article> LatestArticles { get; set; }
        public IEnumerable<Announcement> Announcements { get; set; }
        public IEnumerable<CaseStudy> CaseStudies { get; set; }
        public IEnumerable<MediaMention> MediaMentions { get; set; }
        public IEnumerable<Resource1> Resources1 { get; set; }

    }
}
