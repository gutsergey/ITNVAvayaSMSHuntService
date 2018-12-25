using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ITNVAvayaSMSHuntService
{

    public static class Main
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void HuntsProcessing()
        {
            log.Info("--->");
            try
            {
                HuntsDictionary hd = new HuntsDictionary();
                hd.Dic.Clear();

                Hunts hs = new Hunts();
                hs.Run();

                var zz = hs.Dic.Where(z => z.Value.acd == false);

                Hunt h = new Hunt();

                if (h.setup())
                {
                    foreach (KeyValuePair<string, Hunts.Element> k in zz)
                    {
                        List<string> extensions = h.Run(k.Value.number);
                        if (extensions != null)
                            hd.BuildDic(k, extensions);
                    }
                    h.releaseSession();
                }

                if (hd != null)
                {
                    log.Info("before ---> SendToHuntsDic");
                    string s = JsonConvert.SerializeObject(hd.Dic, Formatting.None);
                    log.Info(s);

                    ITNVPlayMsgManager.ITNVPlayMsgManager pmm = new ITNVPlayMsgManager.ITNVPlayMsgManager();
                    bool rc = pmm.SendToHuntsDic(s);
                    log.Info("after  ---> SendToHuntsDic, rc: " + rc);
                }
                else
                {
                    log.Info("HuntsDictionary is null, there is nothing to send to tehe ITNVPlayMsgManager service.");
                }
                //Dictionary<string, Element> ddd = new Dictionary<string, Element>();
                //Dictionary<string, Element>  ddd = JsonConvert.DeserializeObject<Dictionary<string, Element>>(s);
                //var e = ddd.Select(x => x.Value.Extensions);

                //List<string> mergedList = new List<string>(); 
                //foreach (List<string> el in e)
                //{
                //    mergedList = mergedList.Union(el).ToList();
                //}


                //hd.Dic = JsonConvert.DeserializeObject<Dictionary<string, Element>>(s);
            }
            catch(Exception exc)
            {
                log.Error(exc.Message);
                log.Error(exc.StackTrace);
            }
            log.Info("<---");
        }

        public static long HeartbeatProcessing()
        {
            long rc = 0;
            //log.Info("--->");

            try
            {
                ITNVPlayMsgManager.ITNVPlayMsgManager pmm = new ITNVPlayMsgManager.ITNVPlayMsgManager();
                long cnt = pmm.Heartbeat();
                if (cnt == 0)
                    log.Info( " =====================>  cnt: " + cnt);
                rc = cnt;
            }
            catch (Exception exc)
            {
                log.Error(exc.Message);
                log.Error(exc.StackTrace);
                rc = -1;
            }

            //log.Info("<--- rc: " + rc);
            return rc;
        }
    }
}
