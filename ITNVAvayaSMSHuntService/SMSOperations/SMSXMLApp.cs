
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.Web.Services.Protocols;
using ITNVAvayaSMSHuntService.com.avaya.smsxml;

namespace ITNVAvayaSMSHuntService
{
    public class SMSXMLApp
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Package Namespace
        private static string WEB_REFERENCE_NAMESPACE = "ITNVAvayaSMSHuntService.com.avaya.smsxml";

        // SOAP Objects
        private SystemManagementService sms;

        // Keep reference to the sessionHeader element, as it is also used across requests
        private @string SID;

        // connection parameters
        private string root = "";
        private string login = "";
        private string pw = "";

        // request parameters
        public string model;
        public string operation;
        public string qualifier;
        public string objectname;
        public string fields;

        // results
        public Result result;
        public modelChoices mc;
        public MethodInfo method;
        public arrayType[] xxx;


        // Output control
        //private string pp = "";
        private bool faultRaised = false;

        // XML format output
        //private string format = "";		// string(it's default) or xml
        //private static string FORMAT_XML = "xml";

        // Delimiter
        private char[] delimiter = new char[1] { '|' };
        //private static SMSXMLApp client = null;

        public SMSXMLApp()
        {
                root = "https://" + Configuration.Instance.Aesserver;   
                log.Info("---> SMS Host=" + root);

                login = Configuration.Instance.User + "@" + Configuration.Instance.Cmserver;       
                log.Info("---> CM Login ID=" + login);

                pw = Cfg.GetValue("password");                               
        }


        /*  steps executing SMS request
        *   1.  Initial setup of SOAP binding, session management
        *   2.  Submitting a request to SMS and obtaining the result
        *   3.  Doing session "bookkeeping" after the request
        *   4.  Releasing SMS session resources, when finished.
        */
        public Result execRequest()
        {
            Result result = submitRequest();
            manageSession();
            return result;
        }

        // Step 1: setup()
        // Performs initial setup of SOAP port/binding
        public bool setup()
        {

            // this is the web service object... 
            sms = new SystemManagementService();

            // define soap protocol... 
            sms.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap11;

            // define the encoding... 
            sms.RequestEncoding = System.Text.Encoding.UTF8;

            try
            {
                // Access SMS at the root URL (http(s)://smshostaddress)
                // and the fixed path "/smsxml/SystemManagementService.php"
                Uri uri = new Uri(root + "/smsxml/SystemManagementService.php");

                // Bind to a port on the AES service.
                sms.Url = uri.AbsoluteUri;

                // Set a timeout for the web service response. Here we allow 10,000 ms (10 seconds)
                sms.Timeout = 1000000;
            }

            catch (UriFormatException e)
            {
                log.Info("Bad URL (" + root + "/sms/SystemManagementService.php); cannot access SMS:");
                log.Info(e.Message);
                log.Info(e.StackTrace);
                return false;
            }

            // define Session ID to be null initially 
            SID = new @string();
            SID.Text = new string[] { };

            // Set some properties expected by the SMS server
            SID.Actor = "http://schemas.xmlsoap.org/soap/actor/next";
            SID.MustUnderstand = true;

            // Set the sessionID element in the SOAP header (this causes it to be written to all subsequent requests)
            sms.sessionID = SID;


            System.Net.ServicePointManager.CertificatePolicy = new BruteForcePolicy();
            // Set the login and credentials in the HTTP authorization header (these will also be written to all subsequent requests)
            // If Authentication is failing are we using proxy..
            sms.Credentials = new System.Net.NetworkCredential(this.login, this.pw);
            sms.PreAuthenticate = true;


            return true; // setup is good
        }

        // Step 2: submitRequest()
        // Submits the SOAP request defined in the SMSTest object to the SMS web service for processing.
        private Result submitRequest()
        {

            // l
            submitRequestType requestType = null;
            returnType returnT = null;
            Result result = null;

            // We track whether a fault was raised, because it affects whether we copy session info from the response header later
            faultRaised = false;

            // Send the SOAP request to SMS running on the AES server
            String mType = WEB_REFERENCE_NAMESPACE + "." + model + "Type";

            try
            {
                // Try to create model type
                Type modelType = Type.GetType(mType, true); // true fail on error - model doesnt exist!

                object[] modelArray = new object[1];
                modelArray[0] = System.Activator.CreateInstance(modelType);

                // Populate Fields
                modelArray[0] = populateFields(modelArray[0]);

                requestType = new submitRequestType();
                requestType.modelFields = new modelChoices();
                requestType.modelFields.Items = modelArray;

                requestType.objectname = objectname;  // objectname, not presently used
                requestType.operation = operation;
                requestType.qualifier = qualifier;

                returnT = sms.submitRequest(requestType);
                result = returnT.@return;

            }
            catch (TypeLoadException tle)
            {
                log.Info("ModelType: " + mType + " could not be loaded!  Please verify this is a valid model.");
                log.Info(tle.Message);
                log.Info(tle.StackTrace);
                faultRaised = true;
            }
            catch (SoapException soapE)
            {
                // A fault was raised.  The fault message will contain the explanation
                if (soapE.Message != null)
                { log.Info("Code: " + soapE.Code.Name); }
                if (soapE.Message != null)
                { log.Info("Message: " + soapE.Message); }
                if (soapE.Detail != null)
                { log.Info("Detail: " + soapE.Detail.InnerXml); }

                log.Info("A SOAP fault was raised: ");
                log.Info(soapE.Message);
                log.Info(soapE.StackTrace);
                faultRaised = true;
            }
            catch (Exception re)
            {
                // A fault was raised.  The fault message will contain the explanation
                log.Info("An unexpected Exception was raised: ");
                log.Info(re.Message);
                log.Info(re.StackTrace);
                faultRaised = true;
            }

            // If we got a result (not a fault) then we'll show it
            if (!faultRaised)
            {

                // The result code indicates success or failure (CM rejected the request)
                if (result.result_code == 0)
                {
                    log.Info("The request was successful (result_code == 0)");
                    modelChoices mc = result.result_data;

                    // How many items were returned
                    int resultLength = mc.Items.Length;
                    if (resultLength > 0)
                    {
                        // We'll use a routine to illustrate processing the return values
                        //prettyPrint(mc);
                    }
                }
                else
                {
                    faultRaised = true;
                    // In case CM rejected the request, the message text will contain CM's explanation

                    log.Info("The request returned an error (result_code == " + result.result_code + ")");
                    log.Info("result_data == " + '"' + result.result_data.Items + '"');
                    log.Info("message_text == " + result.message_text);
                }
            }
            return result;
        }

        // Step 3: manageSession()
        // Obtains the sessionID from a valid response and copies into the request header
        // sessionID element for the next request
        private void manageSession()
        {
            // If the last request raised a fault, then there is no session response header, and no change
            // to session management information
            if (!faultRaised)
            {
                // None of the exception conditions below should occur, except in case of programming error
                // In our sample code, we simply print and ignore such conditions.  In a real application
                // handling might involve stopping the application.  Repeated invocation of SMS requests
                // without propagating the sessionID information correctly could result in (temporarily)
                // exhausting all available SMS connections.
                try
                {
                    // retrieve resulting Session ID 
                    SID = sms.sessionID;

                    // Copy the sessionID from the response header back into our request header to be supplied
                    // with the next request.  This fulfills the client obligation with respect to session management
                    //sms.sessionID = ((SID != null) ? SID.Text : "");
                    sms.sessionID = SID;
                }
                catch (Exception se)
                {
                    log.Info(" Error setting the session ID:");
                    log.Info(se.Message);
                    log.Info(se.StackTrace);
                }
            }
        }

        // Step 4.  releaseSession()
        // Releases the SMS session
        public void releaseSession()
        {
            Result result = null;  // release also returns a result structure

            // display resulting Session ID
            string rtn = "";
            foreach (string s in SID.Text)
            {
                rtn += s;
            }
            log.Info("Releasing SessionID[" + rtn + "]...");

            try
            {
                // always send a null value for the argument
                result = sms.release(null).@return;
                // The actual result may be ignored.  If release returns (i.e., does not raise a fault),
                // then it always succeeds, and a result_code of zero is returned.
            }
            catch (Exception re)
            {
                // A fault was raised.  The fault message will contain the explanation
                log.Info("A SOAP fault was raised during the release call:");
                log.Info(re.Message);
                log.Info(re.StackTrace);

                return;
            }
            System.Threading.Thread.Sleep(1000);
            log.Info("The SMS session was released.");
        }

        // The rest of the SMSTest class consistes of helper methods for pretty-printing result data.
        // These methods serve to illustrate how the result data is formatted and how it may be processed.

        private object populateFields(object Obj)
        {
            try
            {
                Type T = Obj.GetType();
                String[] fieldArray = fields.Split(delimiter);
                String mStartsWith = ("set_");
                System.Collections.Hashtable objectTable = new System.Collections.Hashtable();

                foreach (String f in fieldArray)
                {
                    String fvalue = null;
                    String fname = null;
                    bool setField = false;
                    String[] fvalueArray = f.Split(new char[1] { '=' });

                    if (fvalueArray.Length == 2)
                    {
                        // Check if we have an array object
                        if (isArrayField(fvalueArray[0]))
                        {
                            // Find array index
                            uint idx = (uint)arrayIndex(fvalueArray[0]);
                            String fn = arrayFieldname(fvalueArray[0]);
                            fvalue = fvalueArray[1];

                            // The arrayType defined in SMS consists of a 'value' and 'postion'
                            arrayType arrayElement = new arrayType();
                            arrayElement.Value = fvalue;
                            arrayElement.position = idx;

                            // Check if we already have the field in the objectTable and add
                            if (objectTable.ContainsKey(fn))
                            {
                                arrayType[] afv = (arrayType[])objectTable[fn];
                                int currentLength = afv.Length;
                                // Resize the array to add new arrayElement
                                Array.Resize<arrayType>(ref afv, currentLength + 1);
                                afv[currentLength] = arrayElement;
                                objectTable[fn] = afv;

                            }
                            else
                            {
                                arrayType[] new_afv = new arrayType[1];
                                new_afv[0] = arrayElement;
                                objectTable.Add(fn, new_afv);
                            }
                        }

                        else
                        {
                            fname = fvalueArray[0];
                            fvalue = fvalueArray[1];
                            setField = true;
                        }

                    }
                    else
                    {
                        fname = fvalueArray[0];
                        fvalue = "";
                        setField = true;
                    }

                    if (setField)
                    {
                        // Go ahead and set the field since its not an array
                        String mName = (mStartsWith + fname);
                        MethodInfo mi = T.GetMethod(mName);

                        // Try to set the vaule if the field exists
                        if (mi != null)
                        {
                            mi.Invoke(Obj, new String[1] { fvalue });
                            //System.Console.WriteLine(mName + " = \"" + fvalue + '"');
                        }
                    }
                }

                // Now set the object arrays
                if (objectTable.Count > 0)
                {
                    foreach (String key in objectTable.Keys)
                    {
                        String mName = (mStartsWith + key);
                        MethodInfo mi = T.GetMethod(mName);
                        // Try to set the vaule if the field exists
                        if (mi != null)
                        {
                            arrayType[] val = (arrayType[])objectTable[key];
                            mi.Invoke(Obj, new object[] { val });
                            //System.Console.WriteLine("Setting " + key + " Array -- Length of " + val.Length );
                        }
                        else
                        {
                            //System.Console.WriteLine(key + " Element Does Not Exist!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Info("SMSService failed with an unexpected exception:");
                log.Info(e.Message);
                log.Info(e.StackTrace);
            }

            return Obj;

        }

        private bool isArrayField(string field)
        {
            return field.EndsWith("]");
        }

        private int arrayIndex(string arrayField)
        {
            int result = -1;
            try
            {
                int pos = arrayField.LastIndexOf("[") + 1;
                result = int.Parse(arrayField.Substring(pos, arrayField.Length - (pos + 1)));
            }
            catch (FormatException e)
            {
                log.Info("SMSService failed with an unexpected exception:");
                log.Info(e.Message);
                log.Info(e.StackTrace);
                result = -1;
            }
            return result;
        }

        private string arrayFieldname(string arrayField)
        {
            int pos = arrayField.LastIndexOf("[");
            return arrayField.Substring(0, pos);
        }

        public object[] DoOperation(string operation, string model, string objectname, string qualifier)
        {
            log.Info("->");
            this.model = model;
            this.operation = operation;
            this.objectname = objectname;
            this.qualifier = qualifier;
            this.fields = "*";
            try
            {
                //if (setup())
                //{
                    result = execRequest();
                    mc = result.result_data;
                    object[] resultObjectArray = mc.Items;

                    return resultObjectArray;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception e)
            {
                log.Info("SMSService failed with an unexpected exception:");
                log.Info(e.Message);
                log.Info(e.StackTrace);
                return null;
            }
            finally
            {
                //releaseSession();
                log.Info("<-");
            }
        }



    }
         
}