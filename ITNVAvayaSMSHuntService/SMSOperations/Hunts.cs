using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using ITNVAvayaSMSHuntService.com.avaya.smsxml;

namespace ITNVAvayaSMSHuntService
{
    public class Hunts : SMSXMLApp
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public class Element
        {
            public string name;
            public string number;
            public string groupextension;
            public bool acd;

            public Element(string name, string number, string groupextension, bool acd)
            {
                this.name = name;
                this.number = number;
                this.groupextension = groupextension;
                this.acd = acd;
            }
        }

        private Dictionary<string, Element> dic;

        public Dictionary<string, Element> Dic { get { return dic; } }

        public Hunts()
        {
            dic = new Dictionary<string, Element>();
        }

        public bool Run()
        {
            log.Info("->");
            if (!setup())
            {
                releaseSession();
                return false;
            }
            bool rc = GetHunts();

            log.Info("<-");

            releaseSession();
            return rc; //  0;
        }

        private bool GetHunts()
        {
            log.Info("->");

            try
            {
                dic.Clear();
                object[] resultObjectArray = DoOperation("list", "HuntGroup", "", "");
                for (int i = 0; i < resultObjectArray?.Length; i++)
                {
                    bool acd = ((HuntGroupType)resultObjectArray[i]).ACD.Equals("y");
                    string groupextension = ((HuntGroupType)resultObjectArray[i]).Group_Extension;
                    string name = ((HuntGroupType)resultObjectArray[i]).Group_Name;
                    string number = ((HuntGroupType)resultObjectArray[i]).Group_Number;
                    Element e = new Element(name, number, groupextension, acd);

                    dic.Add(groupextension, e);

                }
                return true;

            }
            catch (Exception e)
            {
                log.Info("SMSService failed with an unexpected exception:");
                log.Info(e.Message);
                log.Info(e.StackTrace);
                return false;
            }
            finally
            {
                //releaseSession();
                log.Info("<-");
            }
        }

   }
}
