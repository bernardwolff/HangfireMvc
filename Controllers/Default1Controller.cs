using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using HangfireMvc.Models;

namespace HangfireMvc.Controllers
{
    public class Default1Controller : Controller
    {
        //
        // GET: /Default1/

        public ActionResult Index()
        {
            var serverRole = ConfigurationManager.AppSettings["ServerRole"];
            return View(new Default1Model(){ServerRole = serverRole});
        }

        public ActionResult QueueServerAJob(Default1Model model)
        {
            var jobClient = new BackgroundJobClient();
            var jobId = jobClient.Enqueue(() => BackgroundJobs.QueueServerAJob());
            model.ServerAJobId = jobId;
            return View("Index", model);
        }

        public ActionResult QueueServerBJob(Default1Model model)
        {
            var jobClient = new BackgroundJobClient();
            var jobId = jobClient.Enqueue(() => BackgroundJobs.QueueServerBJob());
            model.ServerBJobId = jobId;
            return View("Index", model);
        }

        public ActionResult QueueServerBJob2(Default1Model model)
        {
            var jobClient = new BackgroundJobClient();
            var jobId = jobClient.Enqueue(() => BackgroundJobs.QueueServerBJob2());
            model.ServerBJob2Id = jobId;
            return View("Index", model);
        }
    }
}
