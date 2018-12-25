using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace ITNVAvayaSMSHuntService
{
    public partial class SMSService : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool isRunning = false;
        //private static bool isRunningHB = false;

        private System.Threading.TimerCallback cb;
        private Timer timer;
        private int duetime;
        private int interval;

        private System.Threading.TimerCallback heartbeatcb;
        private Timer heartbeattimer;
        private int heartbeatduetime;
        private int heartbeatinterval;


        public SMSService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        protected override void OnStop()
        {
            StopService();
        }

        protected void StartService()
        {
            log.Info("->");
            heartbeatinterval = Configuration.Instance.HeartbeatInterval * 1000;  
            heartbeatduetime = Configuration.Instance.HeartbeatDuetime * 1000;
            log.Info("1->");

            heartbeatcb = new System.Threading.TimerCallback(ProcessTimerEventHeartbeat);
            log.Info("2->");

            isRunning = false;

            heartbeattimer = new System.Threading.Timer(heartbeatcb, null, heartbeatduetime, heartbeatinterval);
            log.Info("3->");
            ////////////////////////////////////////////////////////////////////////////////////////////

            interval = Configuration.Instance.Interval * 60 * 1000;  // in config interval is in mins
            duetime = Configuration.Instance.Duetime * 1000;
            log.Info("4->");

            cb = new System.Threading.TimerCallback(ProcessTimerEvent);
            log.Info("5->");

            timer = new System.Threading.Timer(cb, null, duetime, interval);

            log.Info("<-");
        }

        private static void ProcessTimerEventHeartbeat(object obj)
        {
            try
            {
                long rc = Main.HeartbeatProcessing();
                if (rc == 0)
                {
                    if (!isRunning)
                    {
                        log.Info("--------------->");
                        isRunning = true;
                        Main.HuntsProcessing();
                        isRunning = false;
                        log.Info("<---------------");
                    }
                    else
                    {
                        log.Info("SMSXMLApp object: sms is still running");
                    }

                }

            }
            catch (Exception e)
            {
                isRunning = false;
                log.Info(e.Message);
                log.Info(e.StackTrace);
            }

        }


        private static void ProcessTimerEvent(object obj)
        {
            try
            {
                if (!isRunning)
                {
                    log.Info("--------------->");
                    isRunning = true;
                    Main.HuntsProcessing();
                    isRunning = false;
                    log.Info("<---------------");
                }
                else
                {
                    log.Info("SMSXMLApp object: sms is still running");
                }

                
            }
            catch (Exception e)
            {
                isRunning = false;
                log.Info(e.Message);
                log.Info(e.StackTrace);
            }

        }

        private void StopService()
        {
            timer.Dispose();
            heartbeattimer.Dispose();
        }

    }
}
