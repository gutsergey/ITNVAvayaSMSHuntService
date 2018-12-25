using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ITNVAvayaSMSHuntService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Configuration.Initialize();
            if (!Configuration.Instance.Debug)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new SMSService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                ////////for test 
                SMSTest.RunTest();
            }
        }
    }
}
