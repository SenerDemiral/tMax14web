using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FbLibrary
{
    public static class SendWithWebSocket
    {
        private static tMax14DataSet dts = new tMax14DataSet();
        private static tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter fta = new tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter();
        private static tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter mta = new tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter();
        private static tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter hta = new tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter();

        private static WebSocketSharp.WebSocket wsFrt = new WebSocketSharp.WebSocket("ws://rest.tMax.online/wsFrtConnect");
        private static WebSocketSharp.WebSocket wsOpm = new WebSocketSharp.WebSocket("ws://rest.tMax.online/wsOpmConnect");
        private static WebSocketSharp.WebSocket wsOph = new WebSocketSharp.WebSocket("ws://rest.tMax.online/wsOphConnect");

        // typ = F:Full else M:Modified 
        public static void FrtSend(string typ)
        {
            int nor = fta.Fill(dts.WEB_FRT_MDFD, typ);

            if (nor > 0)
            {
                Logs.WriteErrorLog("FRT #Rec: " + nor.ToString());
                if (wsFrt.ReadyState != WebSocketState.Open)
                    wsFrt.Connect();

                if (wsFrt.ReadyState == WebSocketState.Open)
                {
                    if (typ == "F")
                        Logs.WriteErrorLog("FRT Full " + nor.ToString());

                    dynamic jsn = new JObject();
                    foreach (tMax14DataSet.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
                    {
                        jsn.Evnt = row.EVNT;
                        jsn.FrtID = row.FRTID;
                        jsn.AdN = row.ADN;
                        jsn.LocID = row.LOCID;
                        jsn.Pwd = row.PWD;
                        /*
                        //jsn.AdN = Regex.Replace(row.ADN.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());  // Word first char to Upper
                        //Logs.WriteErrorLog(jsn.ToString());   // Formatted 

                        string str = JsonConvert.SerializeObject(jsn);
                        Logs.WriteErrorLog(str);

                        // String to JSON
                        //dynamic json = JValue.Parse(jsn.ToString());
                        dynamic json = JValue.Parse(str);
                        Logs.WriteErrorLog("From Json String :" + json.AdN);
                        */
                        wsFrt.Send(JsonConvert.SerializeObject(jsn));

                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("FRT Modified ID: " + row.FRTID);
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        fta.Update(dts.WEB_FRT_MDFD);
                }
                else
                {
                    Logs.WriteErrorLog("wsFRT can't connect");
                }

                wsFrt.Close();
            }
        }

        public static void OpmSend(string typ)
        {
            int nor = mta.Fill(dts.WEB_OPM_MDFD, typ);

            if (nor > 0)
            {
                Logs.WriteErrorLog("OPM #Rec: " + nor.ToString());
                if (wsOpm.ReadyState != WebSocketState.Open)
                    wsOpm.Connect();

                if (wsOpm.ReadyState == WebSocketState.Open)
                {
                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_OPM_MDFDRow row in dts.WEB_OPM_MDFD.Rows)
                    {
                        jsn.Tbl = "OPM";
                        jsn.Evnt = row.EVNT;
                        jsn.OpmID = row.OPMID.ToString();
                        jsn.RefNo = row["REFNO"];
                        jsn.EXD = row.EXD.ToString();

                        jsn.ROT = row.ROT;
                        jsn.MOT = row.MOT;
                        jsn.Org = row.ORG;
                        jsn.Dst = row.DST;
                        
                        jsn.ShpID = row.IsSHPIDNull() ? "" : row.SHPID.ToString();
                        jsn.CneID = row.IsCNEIDNull() ? "" : row.CNEID.ToString();
                        jsn.AccID = row.IsACCIDNull() ? "" : row.ACCID.ToString();
                        jsn.CrrID = row.IsCRRIDNull() ? "" : row.CRRID.ToString();

                        //jsn.ETD = row.IsETDNull() ? "" : row.ETD.ToString();
                        jsn.ATD = row.IsATDNull() ? "" : row.ATD.ToString();
                        jsn.ETA = row.IsETANull() ? "" : row.ETA.ToString();
                        jsn.ATA = row.IsATANull() ? "" : row.ATA.ToString();
                        jsn.ACOT = row.IsACOTNull() ? "" : row.ACOT.ToString();

                        jsn.Vhc = row.IsVHCNull() ? "" : row.VHC;

                        jsn.ETD = row["ETD"]; // Deneme

                        wsOpm.Send(JsonConvert.SerializeObject(jsn));
                        Logs.WriteErrorLog(jsn.ToString());

                        dynamic json = JValue.Parse(jsn.ToString());
                        if (json.REFNO == null)
                            Logs.WriteErrorLog("json.REFNO is null" + row["REFNO"]);

                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("OPM Modified: " + row.OPMID.ToString());
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        mta.Update(dts.WEB_OPM_MDFD);

                    wsOpm.Close();
                }
                else
                {
                    Logs.WriteErrorLog("wsOPM can't connect");
                }
            }
        }

        public static void OphSend(string typ)
        {
            int nor = hta.Fill(dts.WEB_OPH_MDFD, typ);

            if (nor > 0)
            {
                Logs.WriteErrorLog("OPH #Rec: " + nor.ToString());
                if (wsOph.ReadyState != WebSocketState.Open)
                    wsOph.Connect();

                if (wsOph.ReadyState == WebSocketState.Open)
                {
                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_OPH_MDFDRow row in dts.WEB_OPH_MDFD.Rows)
                    {
                        jsn.Tbl = "OPH";
                        jsn.Evnt = row.EVNT;
                        jsn.OphID = row.OPHID.ToString();
                        jsn.OpmID = row.OPMID.ToString();
                        jsn.RefNo = row.REFNO;
                        jsn.EXD = row.EXD.ToString();
                        jsn.ROT = row.ROT;
                        jsn.MOT = row.MOT;
                        jsn.Org = row.ORG;
                        jsn.Dst = row.DST;
                        jsn.ShpID = row.IsSHPIDNull() ? "" : row.SHPID.ToString();
                        jsn.CneID = row.IsCNEIDNull() ? "" : row.CNEID.ToString();
                        jsn.AccID = row.IsACCIDNull() ? "" : row.ACCID.ToString();
                        jsn.MnfID = row.IsMNFIDNull() ? "" : row.MNFID.ToString();
                        jsn.NfyID = row.IsNFYIDNull() ? "" : row.NFYID.ToString();
                        jsn.DTM = row.DTM;
                        jsn.PTM = row.PTM;
                        jsn.NOP = row.IsNOPNull() ? "" : row.NOP.ToString();
                        jsn.GrW = row.IsGRWNull() ? "" : row.GRW.ToString();
                        jsn.VM3 = row.IsVM3Null() ? "" : row.VM3.ToString();
                        jsn.ChW = row.IsCHWNull() ? "" : row.CHW.ToString();
                        jsn.CntNoS = row.CNTNOS;

                        jsn.nStu = row.NSTU;
                        jsn.pStu = row.PSTU;
                        jsn.nStuTS = row.IsNSTUTSNull() ? "" : row.NSTUTS.ToString();
                        jsn.pStuTS = row.IsPSTUTSNull() ? "" : row.PSTUTS.ToString();
                        jsn.ROH = row.IsROHNull() ? "" : row.ROH.ToString();
                        jsn.REOH = row.IsREOHNull() ? "" : row.REOH.ToString();
                        jsn.EOH = row.IsEOHNull() ? "" : row.EOH.ToString();
                        jsn.AOH = row.IsAOHNull() ? "" : row.AOH.ToString();
                        jsn.RTR = row.IsRTRNull() ? "" : row.RTR.ToString();
                        jsn.ROS = row.IsROSNull() ? "" : row.ROS.ToString();
                        jsn.POD = row.IsPODNull() ? "" : row.POD.ToString();
                        jsn.DRBD = row.IsDRBDNull() ? "" : row.DRBD.ToString();

                        wsOph.Send(JsonConvert.SerializeObject(jsn));

                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("OPH Modified: " + row.OPHID.ToString());
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        hta.Update(dts.WEB_OPH_MDFD);

                    wsOph.Close();
                }
            }
        }
    }
}
