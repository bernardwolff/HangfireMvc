using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Hangfire;

namespace HangfireMvc
{
    public class BackgroundJobs
    {
        private static void WriteToEventLog(string message)
        {
            var source = "HangfireMvcTest";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, "Application");

            EventLog.WriteEntry(source, message);
        }

        [AutomaticRetry(Attempts = 0)]
        [Queue("serveraqueue")]
        public static void QueueServerAJob()
        {
            WriteToEventLog("HangfireMvc.BackgroundJobs.QueueServerAJob() executed");
        }

        [AutomaticRetry(Attempts = 0)]
        [Queue("serverbqueue")]
        public static void QueueServerBJob()
        {
            WriteToEventLog("HangfireMvc.BackgroundJobs.QueueServerBJob() executed");
        }

        [AutomaticRetry(Attempts = 0)]
        [Queue("serverbqueue2")]
        public static void QueueServerBJob2()
        {
            WriteToEventLog("HangfireMvc.BackgroundJobs.QueueServerBJob2() executed");
        }
    }
}