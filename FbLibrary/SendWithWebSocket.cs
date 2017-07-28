using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FbLibrary
{
    public class Hlpr
    {
        public async Task<string> PostAsync(string uri, string data)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(uri, new StringContent(data));

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            return content; // await Task.Run(() => JsonObject.Parse(content));
        }

        public void DenemeSend()
        {
            /*
            using (var client = new HttpClient())
            {
                person p = new person { name = "Sourav", surname = "Kayal" };
                client.BaseAddress = new Uri("http://localhost:1565/");
                var response = client.PutAsJsonAsync("api/person", p).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                }
                else
                    Console.Write("Error");
            }   
            */
            //string content = await PostAsync("", "");

            //objectContent = new StringContent(content);
            //objectContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            //var response = await httpClient.PostAsync(request, objectContent);
        }

    }

    public static class SendWithWebSocket
    {
        private static tMax14DataSet dts = new tMax14DataSet();

        private static tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter fta = new tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter();
        private static tMax14DataSetTableAdapters.WEB_FRC_MDFDTableAdapter cta = new tMax14DataSetTableAdapters.WEB_FRC_MDFDTableAdapter();
        private static tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter mta = new tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter();
        private static tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter hta = new tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter();
        private static tMax14DataSetTableAdapters.WEB_AFB_MDFDTableAdapter ata = new tMax14DataSetTableAdapters.WEB_AFB_MDFDTableAdapter();

        private static WebSocket wsFrt = new WebSocket("ws://rest.tMax.online/wsFrtConnect");
        private static WebSocket wsFrc = new WebSocket("ws://rest.tMax.online/wsFrcConnect");
        private static WebSocket wsOpm = new WebSocket("ws://rest.tMax.online/wsOpmConnect");
        private static WebSocket wsOph = new WebSocket("ws://rest.tMax.online/wsOphConnect");
        private static WebSocket wsAfb = new WebSocket("ws://rest.tMax.online/wsAfbConnect");


        public static void FrtSendWebClient(string typ)
        {
            int nor = fta.Fill(dts.WEB_FRT_MDFD, typ);

            if (nor > 0)
            {
                Logs.WriteErrorLog("FRT #Rec: " + nor.ToString());

                var cli = new WebClient();
                cli.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                //if (wsFrt.ReadyState == WebSocketState.Open)
                {
                    if (typ == "F")
                        Logs.WriteErrorLog("FRT Full " + nor.ToString());

                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
                    {
                        jsn.Evnt = row["EVNT"];
                        jsn.FrtID = row["FRTID"];
                        jsn.AdN = row["ADN"];
                        jsn.Ad = row["AD"];
                        jsn.LocID = row["LOCID"];
                        jsn.Pwd = row["PWD"];
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
                        //Uri u = new Uri("http://rest.tmax.online/tmax14rest/denemeput");
                        //cli.UploadStringAsync(u, "PUT", JsonConvert.SerializeObject(jsn));
                        string response = cli.UploadString("http://rest.tmax.online/tmax14rest/denemeput", "PUT", JsonConvert.SerializeObject(jsn));
                        Logs.WriteErrorLog("FRT Modified ID: " + row.FRTID);

                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("FRT Modified ID: " + row.FRTID);
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        fta.Update(dts.WEB_FRT_MDFD);
                }
            }
        }


        // typ = F:Full else M:Modified 
        public static void FrtSend(string typ)
        {
            int nor = fta.Fill(dts.WEB_FRT_MDFD, typ);

            if (nor > 0)
            {
                //Logs.WriteErrorLog("FRT #Rec: " + nor.ToString());
                if (wsFrt.ReadyState != WebSocketState.Open)
                    wsFrt.Connect();

                if (wsFrt.ReadyState == WebSocketState.Open)
                {
                    if (typ == "F")
                        Logs.WriteErrorLog("FRT Full " + nor.ToString());

                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
                    {
                        jsn.Evnt = row["EVNT"];
                        jsn.FrtID = row["FRTID"];
                        jsn.AdN = row["ADN"];
                        jsn.Ad = row["AD"];
                        jsn.LocID = row["LOCID"];
                        jsn.Pwd = row["PWD"];
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
                        //Logs.WriteErrorLog("Send " + wsFrt.ReadyState.ToString());
                        wsFrt.Send(JsonConvert.SerializeObject(jsn));
                        //Logs.WriteErrorLog("Sent ");

                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("FRT " + row.FRTID);
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        fta.Update(dts.WEB_FRT_MDFD);

                    wsFrt.Close();
                }
                else
                {
                    Logs.WriteErrorLog("FRT can't connect");
                }
            }
        }

        public static void FrcSend(string typ)
        {
            int nor = cta.Fill(dts.WEB_FRC_MDFD, typ);

            if (nor > 0)
            {
                if (wsFrc.ReadyState != WebSocketState.Open)
                    wsFrc.Connect();

                if (wsFrc.ReadyState == WebSocketState.Open)
                {
                    if (typ == "F")
                        Logs.WriteErrorLog("FRC Full " + nor.ToString());

                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_FRC_MDFDRow row in dts.WEB_FRC_MDFD.Rows)
                    {
                        jsn.Evnt = row["EVNT"];
                        jsn.FrcID = row["FRCID"];
                        jsn.FrtID = row["FRTID"];
                        jsn.Ad = row["AD"];
                        jsn.eMail = row["EMAIL"];
                        jsn.RptIDs = row["RPTIDS"];

                        wsFrc.Send(JsonConvert.SerializeObject(jsn));

                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("FRC " + row.FRCID);
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        cta.Update(dts.WEB_FRC_MDFD);

                    wsFrt.Close();
                }
                else
                {
                    Logs.WriteErrorLog("FRC can't connect");
                }
            }
        }

        public static void OpmSend(string typ)
        {
            int nor = mta.Fill(dts.WEB_OPM_MDFD, typ);

            if (nor > 0)
            {
                //Logs.WriteErrorLog("OPM #Rec: " + nor.ToString());
                if (wsOpm.ReadyState != WebSocketState.Open)
                    wsOpm.Connect();

                if (wsOpm.ReadyState == WebSocketState.Open)
                {
                    if (typ == "F")
                        Logs.WriteErrorLog("OPM Full " + nor.ToString());

                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_OPM_MDFDRow row in dts.WEB_OPM_MDFD.Rows)
                    {
                        jsn.Tbl = "OPM";
                        jsn.Evnt = row["EVNT"];
                        jsn.OpmID = row["OPMID"]; // row.OPMID.ToString();
                        jsn.RefNo = row["REFNO"];
                        jsn.EXD = row["EXD"];// row.EXD.ToString();
                        jsn.nStu = row["NSTU"];
                        jsn.pStu = row["PSTU"];

                        jsn.ROT = row["ROT"];
                        jsn.MOT = row["MOT"];
                        jsn.Org = row["ORG"];
                        jsn.Dst = row["DST"];

                        jsn.ShpID = row["SHPID"]; // row.IsSHPIDNull() ? "" : row.SHPID.ToString();
                        jsn.CneID = row["CNEID"];
                        jsn.AccID = row["ACCID"];
                        jsn.CrrID = row["CRRID"];

                        jsn.ETD = row["ETD"]; //row.IsETDNull() ? "" : row.ETD.ToString();
                        jsn.ATD = row["ATD"];
                        jsn.ETA = row["ETA"];
                        jsn.ATA = row["ATA"];
                        jsn.ACOT = row["ACOT"];
                        jsn.TPAD = row["TPAD"];
                        jsn.TPDD = row["TPDD"];

                        jsn.Vhc = row["VHC"];

                        wsOpm.Send(JsonConvert.SerializeObject(jsn));


                        /*
                        Logs.WriteErrorLog(jsn.ToString());
                        dynamic json = JValue.Parse(jsn.ToString());
                        if (json.REFNO == null)
                            Logs.WriteErrorLog("json.REFNO is null" + row["REFNO"]);
                        */
                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("OPM " + row.OPMID.ToString());
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        mta.Update(dts.WEB_OPM_MDFD);

                    wsOpm.Close();
                }
                else
                {
                    Logs.WriteErrorLog("OPM can't connect");
                }
            }
        }

        public static void OphSend(string typ)
        {
            int nor = hta.Fill(dts.WEB_OPH_MDFD, typ);

            if (nor > 0)
            {
                //Logs.WriteErrorLog("OPH #Rec: " + nor.ToString());
                if (wsOph.ReadyState != WebSocketState.Open)
                    wsOph.Connect();

                if (wsOph.ReadyState == WebSocketState.Open)
                {
                    if (typ == "F")
                        Logs.WriteErrorLog("OPH Full " + nor.ToString());

                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_OPH_MDFDRow row in dts.WEB_OPH_MDFD.Rows)
                    {
                        jsn.Tbl = "OPH";
                        jsn.Evnt = row["EVNT"];
                        jsn.OphID = row["OPHID"];
                        jsn.OpmID = row["OPMID"];
                        jsn.RefNo = row["REFNO"];
                        jsn.EXD = row["EXD"];  // row.EXD.ToString();

                        jsn.ROT = row["ROT"];
                        jsn.MOT = row["MOT"];
                        jsn.Org = row["Org"];
                        jsn.Dst = row["Dst"];

                        jsn.ShpID = row["ShpID"];
                        jsn.CneID = row["CneID"];
                        jsn.AccID = row["AccID"];
                        jsn.MnfID = row["MnfID"];
                        jsn.NfyID = row["NfyID"];
                        jsn.CrrID = row["CrrID"];

                        jsn.DTM = row["DTM"];
                        jsn.PTM = row["PTM"];
                        jsn.NOP = row["NOP"];
                        jsn.GrW = row["GRW"];
                        jsn.VM3 = row["VM3"];
                        jsn.ChW = row["CHW"];
                        jsn.CntNoS = row["CNTNOS"];
                        jsn.CABW = row["CABW"];

                        jsn.nStu = row["NSTU"]; // row.NSTU;
                        jsn.pStu = row["PSTU"];
                        jsn.nStuTS = row["NSTUTS"]; // row.IsNSTUTSNull() ? "" : row.NSTUTS.ToString();
                        jsn.pStuTS = row["PSTUTS"];
                        jsn.ROH = row["ROH"];
                        jsn.EOH = row["EOH"];
                        jsn.AOH = row["AOH"];
                        jsn.RTR = row["RTR"];
                        jsn.ROS = row["ROS"];
                        jsn.POD = row["POD"];
                        jsn.REOH = row["REOH"];
                        jsn.DRBD = row["DRBD"];

                        wsOph.Send(JsonConvert.SerializeObject(jsn));

                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("OPH " + row.OPHID.ToString());
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        hta.Update(dts.WEB_OPH_MDFD);

                    wsOph.Close();
                }
                else
                {
                    Logs.WriteErrorLog("OPH can't connect");
                }
            }
        }

        public static void AfbSend(string typ)
        {
            int nor = ata.Fill(dts.WEB_AFB_MDFD, typ);

            if (nor > 0)
            {
                //Logs.WriteErrorLog("AFB #Rec: " + nor.ToString());
                if (wsAfb.ReadyState != WebSocketState.Open)
                    wsAfb.Connect();

                if (wsAfb.ReadyState == WebSocketState.Open)
                {
                    if (typ == "F")
                        Logs.WriteErrorLog("AFB Full " + nor.ToString());

                    dynamic jsn = new JObject();

                    foreach (tMax14DataSet.WEB_AFB_MDFDRow row in dts.WEB_AFB_MDFD.Rows)
                    {
                        jsn.Tbl = "OPM";
                        jsn.Evnt = row["EVNT"];

                        jsn.AfbID = row["AFBID"];
                        jsn.Tur = row["TUR"];
                        jsn.FrtID = row["FRTID"];
                        jsn.FtrNo = row["FTRNO"];
                        jsn.FtrTrh = row["FTRTRH"];
                        jsn.OdmVde = row["ODMVDE"];

                        jsn.bDvz = row["BDVZ"];
                        jsn.bTutBrt = row["BTUTBRT"];

                        jsn.DknFrtID = row["DKNFRTID"];
                        jsn.DknNo = row["DKNNO"];
                        jsn.DknDvz = row["DKNDVZ"];

                        wsAfb.Send(JsonConvert.SerializeObject(jsn));


                        /*
                        Logs.WriteErrorLog(jsn.ToString());
                        dynamic json = JValue.Parse(jsn.ToString());
                        if (json.REFNO == null)
                            Logs.WriteErrorLog("json.REFNO is null" + row["REFNO"]);
                        */
                        if (typ == "M")
                        {
                            Logs.WriteErrorLog("AFB " + row.AFBID.ToString());
                            row.Delete();
                        }
                    }
                    if (typ == "M")
                        ata.Update(dts.WEB_AFB_MDFD);

                    wsAfb.Close();
                }
                else
                {
                    Logs.WriteErrorLog("AFB can't connect");
                }
            }
        }

    }
}
