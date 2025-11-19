using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Quartz;
using Quartz.Impl;

namespace Mindhaven
{
    public class MvcApplication : System.Web.HttpApplication

    {
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity.IsAuthenticated &&
                HttpContext.Current.Session != null &&
                HttpContext.Current.Session["Role"] != null)
            {
                string role = HttpContext.Current.Session["Role"].ToString();
                string[] roles = new string[] { role };

                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                    HttpContext.Current.User.Identity, roles);
            }
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Start Quartz scheduler
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start().Wait();

            IJobDetail job = JobBuilder.Create<MoodReminderJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("MoodReminderTrigger", "default")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(60) // run every hour
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger).Wait();
        }


        
    }
}
