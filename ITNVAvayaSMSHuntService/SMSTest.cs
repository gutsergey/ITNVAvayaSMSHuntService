using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ITNVAvayaSMSHuntService
{
    public class SMSTest
    {

        public static void RunTest()
        {
            Main.HeartbeatProcessing();
            Main.HuntsProcessing();
        }


    }
}
