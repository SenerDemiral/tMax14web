using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FB2SCservice
{
    public static class Library
    {
        private static tMax14DataSet dts = new tMax14DataSet();
        private static tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter fta = new tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter();

        private static WebSocketSharp.WebSocket wsFrt = new WebSocketSharp.WebSocket("ws://rest.tMax.online/wsFrtConnect");
        private static WebSocketSharp.WebSocket wsOpm = new WebSocketSharp.WebSocket("ws://rest.tMax.online/wsOpmConnect");
        private static WebSocketSharp.WebSocket wsOph = new WebSocketSharp.WebSocket("ws://rest.tMax.online/wsOphConnect");

        public static void FrtCron(string typ)
        {
            if (wsFrt.ReadyState != WebSocketState.Open)
                wsFrt.Connect();

            if (wsFrt.ReadyState == WebSocketState.Open)
            {
                int nor = fta.Fill(dts.WEB_FRT_MDFD, typ);
                WriteErrorLog("FRT " + nor.ToString());
                foreach (tMax14DataSet.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
                {
                    /*
                    var jsn = new FrtMsg
                    {
                        Evnt = row.EVNT,
                        FrtID = row.FRTID.ToString(),
                        AdN = row.ADN,
                        Pwd = row.PWD
                    };
                    wsFrt.Send(jsn.ToJson());
                    */
                    if (typ == "X")
                        row.Delete();
                }
                if (typ == "X")
                    fta.Update(dts.WEB_FRT_MDFD);
            }
            else
            {
                WriteErrorLog("wsFRT can't connect");
            }
            //wsFrt.Close();
        }

        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + ex.Source.ToString() + ": " + ex.Message.ToString());
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }

        public static void WriteErrorLog(string Msg)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }
    }
}
