using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using ITNVAvayaSMSHuntService.com.avaya.smsxml;

namespace ITNVAvayaSMSHuntService
{
    public class Hunt : SMSXMLApp
    {
        // only for Hunts not Skills ( where ACD = N)
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> extensions;

        //public List<string> Extensions { get { return extensions; } }

        public Hunt()
        {
            extensions = new List<string>();
        }

        public List<string> Run(string number)
        {
            log.Info("-> number: " + number);
            
            if (!GetHunt(number))
                extensions = null;

            log.Info("<-");
            
            return extensions; 
        }

        private bool GetHunt(string number)
        {
            log.Info("->");

            try
            {
                string extnumber = "";

                extensions.Clear();
                object[] resultObjectArray = DoOperation("display", "HuntGroup", "Group_Number", number);

                for (int i = 0; i < ((HuntGroupType)resultObjectArray[0])?.Assigned_Extension.Length; i++)
                {
                    extnumber = ((HuntGroupType)resultObjectArray[0]).Assigned_Extension[i].Value;
                    extensions.Add(extnumber);
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
