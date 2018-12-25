using System;
using Newtonsoft.Json;

namespace ITNVAvayaSMSHuntService
{
    public  class Configuration
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Configuration instance;

        private bool debug = false;
        private string aesserver;
        private string cmserver;
        private string user;
        private string password;

        private int interval;
        private int duetime;

        private int heartbeatinterval;
        private int heartbeatduetime;

        public static void Initialize()
        {
            if (instance == null)
                instance = new Configuration();

            Instance.interval = Cfg.GetValueInt("interval", 120);   // in mins
            Instance.duetime = Cfg.GetValueInt("duetime", 5);       // in secs
            Instance.heartbeatinterval = Cfg.GetValueInt("heartbeatinterval", 60);      // in secs
            Instance.heartbeatduetime = Cfg.GetValueInt("heartbeatduetime", 5);         // in secs
            Instance.aesserver = Cfg.GetValue("aesserver");
            Instance.cmserver = Cfg.GetValue("cmserver");
            Instance.user = Cfg.GetValue("user");
            Instance.password = Cfg.GetValue("password");
            Instance.debug = Cfg.GetValueBool("debug");
            PrintConfig();
        }

        public static void PrintConfig()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string jsonstr = JsonConvert.SerializeObject(instance, Formatting.Indented);
            log.Info("-----------------------> version: " + version.ToString());
            log.Info(jsonstr);


            //log.Info("ITNVMsgManager URL: " + global::AgentSide.Properties.Settings.Default.AgentSide_ITNVMsgManager_ITNVPlayMsgManager);
            //Console.Out.WriteLine(jsonstr);
            //Console.Out.WriteLine(global::AgentSide.Properties.Settings.Default.AgentSide_ITNVMsgManager_ITNVPlayMsgManager);
        }

        public int Interval { get { return interval; } }
        public int Duetime { get { return duetime; } }
        public int HeartbeatInterval { get { return heartbeatinterval; } }
        public int HeartbeatDuetime { get { return heartbeatduetime; } }
        public string Aesserver { get { return aesserver; } }
        public string Cmserver { get { return cmserver; } }
        public string User { get { return user; } }
        public string Password { get { return password; } }
        public bool Debug { get { return debug; } }

        public static Configuration Instance { get { return instance; } }
    }

}
