using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.SqlServer.Msmq;

namespace HangfireMvc
{
    public class HangfireBootstrapper
    {
        private static BackgroundJobServer _serverA;
        private static BackgroundJobServer _serverB;
        private static bool _started;
        private static SqlServerStorage _currentJobStorage;

        // process server
        private static void StartServerA()
        {
            _currentJobStorage.UseMsmqQueues(@".\private$\hangfire-{0}", "serveraqueue");

            var options = new BackgroundJobServerOptions
            {
                Queues = new[] { "serveraqueue" },
                WorkerCount = 1,
                ServerName = String.Format("{0}:serveraqueue", "ServerA"),
            };

            _serverA = new BackgroundJobServer(options, JobStorage.Current);
            _serverA.Start();
        }

        // web server
        private static void StartServerB()
        {
            _currentJobStorage.UseMsmqQueues(@".\private$\hangfire-{0}", "serverbqueue", "serverbqueue2");

            var serverBOptions = new BackgroundJobServerOptions
            {
                Queues = new[] { "serverbqueue" },
                WorkerCount = 8,
                ServerName = String.Format("{0}:serverbqueue", "ServerB"),
            };
            _serverA = new BackgroundJobServer(serverBOptions, JobStorage.Current);
            

            var serverB2Options = new BackgroundJobServerOptions
            {
                Queues = new[] { "serverbqueue2" },
                WorkerCount = 1,
                ServerName =
                    String.Format("{0}:serverbqueue2", "ServerB"),
            };
            _serverB = new BackgroundJobServer(serverB2Options, JobStorage.Current);

            _serverA.Start();
            _serverB.Start();
        }

        private static void StartAllInOneServer()
        {
            _currentJobStorage.UseMsmqQueues(@".\private$\hangfire-{0}", "serveraqueue", "serverbqueue", "serverbqueue2");

            var defaultHangFireInstanceOptions = new BackgroundJobServerOptions
            {
                Queues = new[] { "serveraqueue", "serverbqueue" },
                WorkerCount = 2,
                ServerName = String.Format("{0}:serverbqueue", "AllInOneServer"),
            };
            _serverA = new BackgroundJobServer(defaultHangFireInstanceOptions, JobStorage.Current);

            var exportContactsInstanceOptions = new BackgroundJobServerOptions
            {
                Queues = new[] { "serverbqueue2" },
                WorkerCount = 1,
                ServerName = String.Format("{0}:serverbqueue2", "AllInOneServer"),
            };
            _serverB = new BackgroundJobServer(exportContactsInstanceOptions, JobStorage.Current);

            _serverA.Start();
            _serverB.Start();
        }

        public static void Start()
        {
            if (_started) return;
            _started = true;

            var serverRole = ConfigurationManager.AppSettings["ServerRole"];
            _currentJobStorage = new SqlServerStorage(ConfigurationManager.ConnectionStrings["HangfireConn"].ConnectionString, new SqlServerStorageOptions());

            JobStorage.Current = _currentJobStorage;

            if (serverRole == "ServerA")
            {
                StartServerA();
            }
            else if (serverRole == "ServerB")
            {
                StartServerB();
            }
            else
            {
                StartAllInOneServer();
            }
        }

        public static void Stop()
        {
            if (_serverA != null)
            {
                _serverA.Stop();
                _serverA.Dispose();
            }

            if (_serverB != null)
            {
                _serverB.Stop();
                _serverB.Dispose();
            }
        }
    }
}