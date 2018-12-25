using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System;

namespace ITNVAvayaSMSHuntService
{
    /// </summary>
    internal class Cfg
    {

        /// <summary>
        /// Method GetValue.
        /// It gets 1 param: name of parmeter and find it into appsettings section
        /// </summary>
        /// <returns>
        /// The method returns value of the parameter.
        /// </returns>
        internal static string GetValue(string paramName)
        {
            string[] prm = null;
            try
            {
                prm = System.Configuration.ConfigurationManager.AppSettings.GetValues(paramName);
                if (prm == null)
                    return "";
                else
                    return (System.Configuration.ConfigurationManager.AppSettings.GetValues(paramName))[0];
            }
            catch (Exception exc)
            { return ""; }
        }

        internal static bool GetValueBool(string paramName, bool defaultvalue = false)
        {
            string[] prm = null;
            try
            {
                prm = System.Configuration.ConfigurationManager.AppSettings.GetValues(paramName);
                if (prm == null)
                    return defaultvalue;
                else
                {
                    string s = (System.Configuration.ConfigurationManager.AppSettings.GetValues(paramName))[0];
                    bool result = false;
                    result = bool.TryParse(s, out result) ? result : defaultvalue;
                    return result;
                }
            }
            catch (Exception exc)
            { return defaultvalue; }
        }

        internal static int GetValueInt(string paramName, int defaultvalue = 0)
        {
            string[] prm = null;
            try
            {
                prm = System.Configuration.ConfigurationManager.AppSettings.GetValues(paramName);
                if (prm == null)
                    return defaultvalue;
                else
                {
                    string s = (System.Configuration.ConfigurationManager.AppSettings.GetValues(paramName))[0];
                    int result = 0;
                    result = int.TryParse(s, out result) ? result : defaultvalue;
                    return result;
                }
            }
            catch (Exception exc)
            { return defaultvalue; }
        }

        /// <summary>
        /// Method GetValue.
        /// 1 param: name of a section.
        /// 2 param: name of parmeter and find it into the section
        /// that its name defined by the 1 param.
        /// </summary>
        /// <returns>
        /// The method returns value of the parameter.
        /// </returns>
        internal static string GetValue(string sectionName, string paramName)
        {
            string prm = "";
            try
            {
                //var applicationSettings = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
                prm = ((NameValueCollection)(ConfigurationManager.GetSection(sectionName)))[paramName];
                //return applicationSettings[paramName];
                return prm;
            }
            catch (Exception exc)
            { return prm; }
        }

        internal static bool GetValueBool(string sectionName, string paramName, bool defaultvalue)
        {
            string prm = "";
            try
            {
                prm = ((NameValueCollection)(ConfigurationManager.GetSection(sectionName)))[paramName];
                bool result = false;
                result = bool.TryParse(prm, out result) ? result : defaultvalue;
                return result;

            }
            catch (Exception exc)
            { return defaultvalue; }
        }

        internal static int GetValueInt(string sectionName, string paramName, int defaultvalue)
        {
            string prm = "";
            try
            {
                prm = ((NameValueCollection)(ConfigurationManager.GetSection(sectionName)))[paramName];
                int result = 0;
                result = int.TryParse(prm, out result) ? result : defaultvalue;
                return result;

            }
            catch (Exception exc)
            { return defaultvalue; }
        }

        internal static string GetConnectionString(string connStringName)
        {
            string connString = null;
            try
            {
                connString = System.Configuration.ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
                if (connString == null)
                    return "";
                else
                    return connString;
            }
            catch (Exception exc)
            { return connString; }
        }
    }
}
