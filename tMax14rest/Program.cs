﻿using System;
using System.Text;
using Starcounter;
using Starcounter.Advanced.XSON;
using Starcounter.XSON.Serializer;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using TMDB;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace tMax14rest
{
	class Program
	{
        enum DW3 { TrckID, Cmnd, Trh, GPS, Lat, Lon, Speed, Zmn, Direction, Alt, Star };
        // NOTE: Timer should be static, otherwise its garbage collected.
        static Timer WebSocketSessionsTimer = null;
        static StreamWriter sw = null; // new StreamWriter(@"C:\Starcounter\MyLog\ScServerUDP-Log.txt", true);


        /// <summary>
        /// Rest'in 2 amaci var
        /// 1. Supply'dan ws ile gelen bilgiler ile SC Database'i guncellemek
        /// 2. SC deki bilgileri periyodik tarayip wsConnect olmus Clientlere ilgili bilgileri gondermek
        /// </summary>

        static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            // Removing existing objects from database.
            /*
			Db.Transact(() => {
				Db.SlowSQL("DELETE FROM WebSocketId");
			});
			*/

            Hook<TMDB.OPH>.CommitInsert += (s, obj) =>
            {
                Console.WriteLine("OBH Inserted: ", obj.OphID);
            };

            Hook<TMDB.OPH>.CommitUpdate += (s, obj) =>
            {

                Console.WriteLine("OBH Updated: ", obj.OphID);
            };


            Handle.GET("/tMax14rest/DENEME", () =>
            {
                Db.Transact(() =>
                {
                    Db.SlowSQL("DELETE FROM TMDB.WebSocketId");
                });
                /*
                WriteErrorLog("CAN ");
                for (int i = 0; i < 10000; i++)
                    WriteErrorLog("Sener DENEME " + i.ToString());
                */
                return "OK";
            });


            Handle.GET("/tMax14rest/InsStu", () =>
            {
                Db.Transact(() =>
                {
                    Db.SlowSQL("DELETE FROM TMDB.OSN");
                    OSN recA = new OSN() { Stu = "A", Ad = "AgentBooking" };
                    OSN recAB = new OSN() { Stu = "AB", Ad = "Aborted" };
                    OSN recB = new OSN() { Stu = "B", Ad = "Booked" };
                    OSN recC = new OSN() { Stu = "C", Ad = "Cancelled" };
                    OSN recE = new OSN() { Stu = "E", Ad = "InHand" };
                    OSN recK = new OSN() { Stu = "K", Ad = "@Carrier" };
                    OSN recN = new OSN() { Stu = "N", Ad = "Departed" };
                    OSN recP = new OSN() { Stu = "P", Ad = "@Destination" };
                    OSN recQC = new OSN() { Stu = "QC", Ad = "QC Failed" };
                    OSN recR = new OSN() { Stu = "R", Ad = "Custom Clearance in Process" };
                    OSN recRE = new OSN() { Stu = "RE", Ad = "Custom Cleared" };
                    OSN recT = new OSN() { Stu = "T", Ad = "Closed" };
                });

                return "OK";
            });

            Handle.GET("/tMax14rest/ResetAll", (OphMsg hJ) =>
            {
                Db.Transact(() =>
                {
                    Db.SlowSQL("DELETE FROM TMDB.AFB");
                    Db.SlowSQL("DELETE FROM TMDB.OPH");
                    Db.SlowSQL("DELETE FROM TMDB.OPM");
                    Db.SlowSQL("DELETE FROM TMDB.FRC");
                    Db.SlowSQL("DELETE FROM TMDB.FRT");
                    Db.SlowSQL("DELETE FROM TMDB.LOC");
                });

                return "OK";
            });

            Handle.WebSocketDisconnect("wsFrt", (WebSocket ws) =>
            {
                Console.WriteLine("wsFrt DisConnected {0}", DateTime.Now);
            });

            Handle.WebSocketDisconnect("wsOph", (WebSocket ws) =>
            {
                Console.WriteLine("wsOph DisConnected {0}", DateTime.Now);
            });

            Handle.WebSocketDisconnect("wsOpm", (WebSocket ws) =>
            {
                Console.WriteLine("wsOpm DisConnected {0}", DateTime.Now);
            });

            Handle.GET("/wsLocConnect", (Request req) =>
            {
                // Checking if its a WebSocket upgrade request.
                if (req.WebSocketUpgrade)
                {
                    Hlpr.WriteRestLog("wsLOC Connected");
                    req.SendUpgrade("wsLoc");
                    return HandlerStatus.Handled;
                }
                Hlpr.WriteRestLog("wsLOC Not Connected");

                // We only support WebSockets upgrades in this HTTP handler
                // and not other ordinary HTTP requests.
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });

            Handle.WebSocket("wsLoc", (string str, WebSocket ws) =>
            {
                dynamic jsn = JValue.Parse(str);

                string rMsg = "OK";

                Console.WriteLine(str);

                Db.Transact(() =>
                {
                    try
                    {
                        string LocID = jsn.LocID;

                        if (jsn.Evnt == "D")
                        {
                            var locs = Db.SQL<TMDB.LOC>("select f from TMDB.LOC f where f.LocID = ?", LocID);
                            foreach (var rec in locs)
                            {
                                rec.Delete();
                            }
                        }
                        else
                        {
                            TMDB.LOC rec = Db.SQL<TMDB.LOC>("select f from TMDB.LOC f where f.LocID = ?", LocID).FirstOrDefault();
                            if (rec == null)
                                rec = new TMDB.LOC();

                            if (rec == null)
                                rMsg = "NoLoc2Update";
                            else
                            {
                                rec.MdfdOn = DateTime.Now;
                                rec.LocID = jsn.LocID;
                                rec.Ad = jsn.Ad;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rMsg = ex.Message;
                    }
                });

                //ws.Send(rMsg);
            });

            Handle.GET("/wsFrtConnect", (Request req) =>
            {
                // Checking if its a WebSocket upgrade request.
                if (req.WebSocketUpgrade)
                {
                    Hlpr.WriteRestLog("wsFRT Connected");
                    req.SendUpgrade("wsFrt");
                    return HandlerStatus.Handled;
                }
                Hlpr.WriteRestLog("wsFRT Not Connected");

                // We only support WebSockets upgrades in this HTTP handler
                // and not other ordinary HTTP requests.
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });

            Handle.WebSocket("wsFrt", (string str, WebSocket ws) =>
            {
                // Handle str and send response
                //FrtMsg jsn = new FrtMsg();
                //jsn.PopulateFromJson(str);
                //var settings = new JsonSerializerSettings();
                //settings.MissingMemberHandling = MissingMemberHandling.Ignore;

                Console.WriteLine("DENEME");
                dynamic jsn = JValue.Parse(str);

                string rMsg = "OK";

                Console.WriteLine(str);

                Db.Transact(() =>
                {
                    try
                    {
                        //int FrtID = int.Parse(jsn.FrtID);
                        int FrtID = jsn.FrtID;
                        Console.WriteLine(FrtID.ToString());

                        if (jsn.Evnt == "D")
                        {
                            var frts = Db.SQL<TMDB.FRT>("select f from TMDB.FRT f where f.FrtID = ?", FrtID);
                            foreach (var rec in frts)
                            {
                                rec.Delete();
                            }
                        }
                        else
                        {
                            TMDB.FRT rec = Db.SQL<TMDB.FRT>("select f from TMDB.FRT f where f.FrtID = ?", FrtID).FirstOrDefault();
                            if (rec == null)
                                rec = new TMDB.FRT();

                            if (rec == null)
                                rMsg = "NoFirma2Update";
                            else
                            {
                                rec.MdfdOn = DateTime.Now;
                                rec.FrtID = FrtID; // Convert.ToInt32(jsn.FrtID);
                                rec.AdN = jsn.AdN;
                                rec.Ad = jsn.Ad;
                                rec.LocID = jsn.LocID;
                                rec.Pwd = jsn.Pwd;
                                Console.WriteLine(FrtID.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rMsg = ex.Message;
                        Console.WriteLine(rMsg);
                    }
                });

                //ws.Send(rMsg);
            });


            Handle.GET("/wsFrcConnect", (Request req) =>
            {
                // Checking if its a WebSocket upgrade request.
                if (req.WebSocketUpgrade)
                {
                    Hlpr.WriteRestLog("wsFRC Connected");
                    req.SendUpgrade("wsFrc");
                    return HandlerStatus.Handled;
                }
                Hlpr.WriteRestLog("wsFRC Not Connected");

                // We only support WebSockets upgrades in this HTTP handler
                // and not other ordinary HTTP requests.
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });

            Handle.WebSocket("wsFrc", (string str, WebSocket ws) =>
            {
                // Handle str and send response
                //FrtMsg jsn = new FrtMsg();
                //jsn.PopulateFromJson(str);
                //var settings = new JsonSerializerSettings();
                //settings.MissingMemberHandling = MissingMemberHandling.Ignore;

                dynamic jsn = JValue.Parse(str);

                string rMsg = "OK";

                Console.WriteLine(str);

                Db.Transact(() =>
                {
                    try
                    {
                        //int FrtID = int.Parse(jsn.FrtID);
                        int FrcID = jsn.FrcID;

                        if (jsn.Evnt == "D")
                        {
                            var frcs = Db.SQL<TMDB.FRC>("select f from TMDB.FRC f where f.FrcID = ?", FrcID);
                            foreach (var rec in frcs)
                            {
                                rec.Delete();
                            }
                        }
                        else
                        {
                            TMDB.FRC rec = Db.SQL<TMDB.FRC>("select f from TMDB.FRC f where f.FrcID = ?", FrcID).FirstOrDefault();
                            if (rec == null)
                                rec = new TMDB.FRC();

                            if (rec == null)
                                rMsg = "NoFirmaContact2Update";
                            else
                            {
                                rec.MdfdOn = DateTime.Now;
                                rec.FrcID = FrcID; // Convert.ToInt32(jsn.FrtID);
                                rec.FrtID = jsn.FrtID;
                                rec.Ad = jsn.Ad;
                                rec.eMail = jsn.eMail;
                                rec.RptIDs = jsn.RptIDs;

                                rec.Frt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.FrtID).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rMsg = ex.Message;
                        Console.WriteLine(rMsg);
                    }
                });

                //ws.Send(rMsg);
            });


            Handle.GET("/wsOpmConnect", (Request req) =>
            {
                if (req.WebSocketUpgrade)
                {
                    Hlpr.WriteRestLog("wsOPM Connected");
                    req.SendUpgrade("wsOpm");
                    return HandlerStatus.Handled;
                }
                Hlpr.WriteRestLog("wsOPM Not Connected");
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });

            Handle.WebSocket("wsOpm", (string str, WebSocket ws) =>
            {
                //OpmMsg jsn = new OpmMsg();
                //jsn.PopulateFromJson(str);
                dynamic jsn = JValue.Parse(str);
                string rMsg = "OK";

                Db.Transact(() =>
                {
                    //int OpmID = int.Parse((string)jsn.OpmID);
                    int OpmID = jsn.OpmID;

                    if (jsn.Evnt == "D")
                    {
                        var opms = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", OpmID);
                        foreach (var rec in opms)
                        {
                            //Db.SQL("delete from OPH h where rec.OphID = ?", OphID);	Boyle yapamiyor!!
                            rec.Delete();
                        }
                    }
                    else
                    {
                        TMDB.OPM rec = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", OpmID).FirstOrDefault();
                        if (rec == null)
                            rec = new TMDB.OPM();

                        if (rec != null)
                        {
                            rec.MdfdOn = DateTime.Now;
                            rec.OpmID = OpmID;
                            rec.RefNo = jsn.RefNo;
                            rec.ROT = jsn.ROT;
                            rec.MOT = jsn.MOT;
                            rec.OrgID = jsn.Org;
                            rec.DstID = jsn.Dst;
                            rec.PolID = jsn.POL;
                            rec.PouID = jsn.POU;
                            rec.Vhc = jsn.Vhc;
                            rec.nStu = jsn.nStu;
                            rec.pStu = jsn.pStu;

                            rec.CntNoS = jsn.CntNoS;
                            rec.SealNoS = jsn.SealNoS;
                            rec.pInfoS = jsn.pInfoS;

                            rec.ShpID = jsn.ShpID;// == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);
                            rec.CneID = jsn.CneID;
                            rec.AccID = jsn.AccID;
                            rec.CrrID = jsn.CrrID;

                            rec.EXD = jsn.EXD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EXD);
                            rec.ETD = jsn.ETD;
                            rec.ATD = jsn.ATD;
                            rec.ETA = jsn.ETA;
                            rec.ATA = jsn.ATA;
                            rec.RETD = jsn.RETD;
                            rec.RETA = jsn.RETA;
                            rec.ACOT = jsn.ACOT;
                            rec.TPAD = jsn.TPAD;
                            rec.TPDD = jsn.TPDD;
                            rec.ROH = jsn.ROH;

                            if (rec.ShpID != null)
                                rec.SHP = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.ShpID).FirstOrDefault();
                            if (rec.CneID != null)
                                rec.CNE = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CneID).FirstOrDefault();
                            if (rec.AccID != null)
                                rec.ACC = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.AccID).FirstOrDefault();
                            if (rec.CrrID != null)
                                rec.CRR = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CrrID).FirstOrDefault();

                            if (rec.OrgID != null)
                                rec.ORG = Db.SQL<TMDB.LOC>("select f from LOC f where f.LocID = ?", rec.OrgID).FirstOrDefault();
                            if (rec.DstID != null)
                                rec.DST = Db.SQL<TMDB.LOC>("select f from LOC f where f.LocID = ?", rec.DstID).FirstOrDefault();
                            if (rec.PolID != null)
                                rec.POL = Db.SQL<TMDB.LOC>("select f from LOC f where f.LocID = ?", rec.PolID).FirstOrDefault();
                            if (rec.PouID != null)
                                rec.POU = Db.SQL<TMDB.LOC>("select f from LOC f where f.LocID = ?", rec.PouID).FirstOrDefault();
                        }
                    }
                });

                //ws.Send(rMsg);
            });


            Handle.GET("/wsOphConnect", (Request req) =>
            {
                if (req.WebSocketUpgrade)
                {
                    Hlpr.WriteRestLog("wsOPH Connected");
                    req.SendUpgrade("wsOph");
                    return HandlerStatus.Handled;
                }
                Hlpr.WriteRestLog("wsOPH Not Connected");
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });

            Handle.WebSocket("wsOph", (string str, WebSocket ws) =>
            {
                //OphMsg jsn = new OphMsg();
                //jsn.PopulateFromJson(s);
                dynamic jsn = JValue.Parse(str);

                string rMsg = "OK";

                Db.Transact(() =>
                {
                    //int OphID = int.Parse(jsn.OphID);
                    int OphID = jsn.OphID;

                    if (jsn.Evnt == "D")
                    {
                        var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where h.OphID = ?", OphID);
                        foreach (var rec in ophs)
                        {
                            //Db.SQL("delete from OPH h where rec.OphID = ?", OphID);	Boyle yapamiyor!!
                            rec.Delete();
                        }
                    }
                    else
                    {
                        TMDB.OPH rec = Db.SQL<TMDB.OPH>("select h from OPH h where h.OphID = ?", OphID).FirstOrDefault();
                        if (rec == null)
                            rec = new TMDB.OPH();

                        if (rec != null)
                        {
                            rec.MdfdOn = DateTime.Now;
                            rec.OphID = OphID;
                            rec.OpmID = jsn.OpmID;// == "" ? (int?)null : Convert.ToInt32(jsn.OpmID);
                            rec.RefNo = jsn.RefNo;
                            rec.ROT = jsn.ROT;
                            rec.MOT = jsn.MOT;
                            rec.OrgID = jsn.Org;
                            rec.DstID = jsn.Dst;
                            rec.CusLocID = jsn.CusLoc;
                            rec.nStu = jsn.nStu;
                            rec.pStu = jsn.pStu;
                            rec.DTM = jsn.DTM;
                            rec.PTM = jsn.PTM;
                            rec.NOP = jsn.NOP; // == "" ? (int?)null : Convert.ToInt32(jsn.NOP);
                            rec.GrW = jsn.GrW;
                            rec.VM3 = jsn.VM3;
                            rec.ChW = jsn.ChW;
                            rec.CntNoS = jsn.CntNoS;
                            rec.CABW = jsn.CABW;

                            rec.ShpID = jsn.ShpID;// == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);
                            rec.CneID = jsn.CneID;
                            rec.AccID = jsn.AccID;
                            rec.MnfID = jsn.MnfID;
                            rec.NfyID = jsn.NfyID;
                            rec.CrrID = jsn.CrrID;

                            rec.EXD = jsn.EXD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EXD);
                            rec.nStuTS = jsn.nStuTS;
                            rec.pStuTS = jsn.pStuTS;
                            rec.ROH = jsn.ROH;
                            rec.EOH = jsn.EOH;
                            rec.AOH = jsn.AOH;
                            rec.AOH = jsn.AOH;
                            rec.RTD = jsn.RTD;
                            rec.RTR = jsn.RTR;
                            rec.ROS = jsn.ROS;
                            rec.POD = jsn.POD;
                            rec.REOH = jsn.REOH;
                            rec.DRBD = jsn.DRBD;
                            rec.DDT = jsn.DDT;
                            rec.OthInf = jsn.OthInf;

                            if (rec.OpmID != null)
                                rec.OPM = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", rec.OpmID).FirstOrDefault();
                            if (rec.ShpID != null)
                                rec.SHP = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.ShpID).FirstOrDefault();
                            if (rec.CneID != null)
                                rec.CNE = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CneID).FirstOrDefault();
                            if (rec.AccID != null)
                                rec.ACC = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.AccID).FirstOrDefault();
                            if (rec.MnfID != null)
                                rec.MNF = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.MnfID).FirstOrDefault();
                            if (rec.NfyID != null)
                                rec.NFY = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.NfyID).FirstOrDefault();
                            if (rec.CrrID != null)
                                rec.CRR = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CrrID).FirstOrDefault();

                            if (rec.OrgID != null)
                                rec.ORG = Db.SQL<TMDB.LOC>("select f from LOC f where f.LocID = ?", rec.OrgID).FirstOrDefault();
                            if (rec.DstID != null)
                                rec.DST = Db.SQL<TMDB.LOC>("select f from LOC f where f.LocID = ?", rec.DstID).FirstOrDefault();
                            if (rec.CusLocID != null)
                                rec.CUSLOC = Db.SQL<TMDB.LOC>("select f from LOC f where f.LocID = ?", rec.CusLocID).FirstOrDefault();
                        }
                    }
                });
                //ws.Send(rMsg);
            });


            Handle.GET("/wsAfbConnect", (Request req) =>
            {
                if (req.WebSocketUpgrade)
                {
                    Hlpr.WriteRestLog("wsAFB Connected");
                    req.SendUpgrade("wsAfb");
                    return HandlerStatus.Handled;
                }
                Hlpr.WriteRestLog("wsAFB Not Connected");

                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });

            Handle.WebSocket("wsAfb", (string str, WebSocket ws) =>
            {
                dynamic jsn = JValue.Parse(str);

                Db.Transact(() =>
                {
                    int AfbID = jsn.AfbID;

                    if (jsn.Evnt == "D")
                    {
                        var afbs = Db.SQL<TMDB.AFB>("select m from AFB m where m.AfbID = ?", AfbID);
                        foreach (var rec in afbs)
                        {
                            rec.Delete();
                        }
                    }
                    else
                    {
                        var rec = Db.SQL<TMDB.AFB>("select m from AFB m where m.AfbID = ?", AfbID).FirstOrDefault();
                        if (rec == null)
                            rec = new TMDB.AFB();

                        if (rec != null)
                        {
                            rec.MdfdOn = DateTime.Now;
                            rec.AfbID = AfbID;
                            rec.Tur = jsn.Tur;
                            rec.FrtID = jsn.FrtID;
                            rec.FtrNo = jsn.FtrNo;
                            rec.FtrTrh = jsn.FtrTrh;
                            rec.OdmVde = jsn.OdmVde;
                            rec.bDvz = jsn.bDvz;
                            rec.bTutBrt = jsn.bTutBrt;
                            rec.DknFrtID = jsn.DknFrtID;
                            rec.DknNo = jsn.DknNo;
                            rec.DknDvz = jsn.DknDvz;

                            if (rec.FrtID != null)
                                rec.Frt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.FrtID).FirstOrDefault();
                            if (rec.DknFrtID != null)
                                rec.DknFrt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.DknFrtID).FirstOrDefault();
                        }
                    }
                });

                //ws.Send(rMsg);
            });




            Handle.PUT("/tMax14rest/Denemeput", (Request request) =>
            {
                //Console.WriteLine("DenemePut: " + request.Body);
                //dynamic jsn = JValue.Parse(request.Body);
                //Console.WriteLine(jsn.FrtID);
                return "OK";
            });

            Handle.PUT("/tMax14rest/Denemeput2", (FrtMsg jsn) =>
            {
                Console.WriteLine("DenemePut: ", jsn);
                return "OK";
            });

            Handle.PUT("/tMax14rest/FRT", (FrtMsg jsn) =>
            {
                Console.WriteLine("FRT: " + jsn.FrtID);
                string rMsg = "OK";
                // jsn'nin ilk field da FRT olmali, ikinci (Evnt) field bos olmamali
                if (jsn[0].ToString() != "FRT" || jsn[1].ToString() == "")
                    return "!HATA-WrongMessageFormat";

                Db.Transact(() =>
                {
                    try
                    {
                        int FrtID = int.Parse(jsn.FrtID);

                        if (jsn.Evnt == "D")
                        {
                            var frts = Db.SQL<TMDB.FRT>("select f from FRT f where rec.FrtID = ?", FrtID);
                            foreach (var rec in frts)
                            {
                                rec.Delete();
                            }
                        }
                        else
                        {
                            TMDB.FRT rec = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", FrtID).FirstOrDefault();
                            if (jsn.Evnt == "I" && rec == null)
                            {
                                rec = new TMDB.FRT();
                            }

                            if (rec == null)
                                rMsg = "NoFirma2Update";
                            else
                            {
                                rec.MdfdOn = DateTime.Now;
                                rec.FrtID = Convert.ToInt32(jsn.FrtID);
                                rec.AdN = jsn.AdN;
                                rec.LocID = jsn.LocID;
                                rec.Pwd = jsn.Pwd;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rMsg = ex.Message;
                    }
                });
                return rMsg;
            });

            Handle.PUT("/tMax14rest/OPM", (OpmMsg opmMsg) =>
            {
                /*
				StringBuilder sb = new StringBuilder();
				Console.WriteLine("/tMax14rest/OPM");
				// jsn'nin ilk field da OPM olmali
				if(opmMsg[0].ToString() != "OPM")
					return "!HATA-WrongMessageFormat";

				Db.Transact(() =>
				{
					foreach(var jsn in opmMsg.OpmA)
					{
						int OpmID = int.Parse(jsn.OpmID);

						if(jsn.Evnt == "D")
						{
							var opms = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", OpmID);
							foreach(var rec in opms)
							{
								//Db.SQL("delete from OPH h where rec.OphID = ?", OphID);	Boyle yapamiyor!!
								rec.Delete();
							}
						}
						else
						{
							TMDB.OPM rec = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", OpmID).First;
							if(jsn.Evnt == "I" && rec == null)
							{
								rec = new TMDB.OPM();
							}

							if(rec != null)
							{
								sb.Append(jsn.OpmID).Append(",");
								rec.MdfdOn = DateTime.Now;
								rec.OpmID = OpmID;
								rec.ROT = jsn.ROT;
								rec.MOT = jsn.MOT;
								rec.Org = jsn.Org;
								rec.Dst = jsn.Dst;

								//rec.CntNoS = jsn.CntNoS;

								rec.ShpID = jsn.ShpID == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);
								rec.CneID = jsn.CneID == "" ? (int?)null : Convert.ToInt32(jsn.CneID);
								rec.AccID = jsn.AccID == "" ? (int?)null : Convert.ToInt32(jsn.AccID);
								rec.CrrID = jsn.CrrID == "" ? (int?)null : Convert.ToInt32(jsn.CrrID);

								rec.EXD = jsn.EXD == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EXD);
								rec.ETD = jsn.ETD == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ETD);
								rec.ATD = jsn.ATD == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ATD);
								rec.ETA = jsn.ETA == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ETA);
								rec.ATA = jsn.ATA == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ATA);

								if(rec.ShpID != null)
									rec.Shp = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.ShpID).First;
								if(rec.CneID != null)
									rec.Cne = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CneID).First;
								if(rec.AccID != null)
									rec.Acc = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.AccID).First;
								if(rec.CrrID != null)
									rec.Crr = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CrrID).First;
							}
						}
					}
				});	  
				return sb.ToString();*/
                return "";
            });

            Handle.PUT("/tMax14rest/OPH", (OphMsg msg) =>
            {
                /*
				StringBuilder sb = new StringBuilder();
				Console.WriteLine("/tMax14rest/OPH");
				// jsn'nin ilk field da OPH olmali
				if(msg[0].ToString() != "OPH")
					return "!HATA-WrongMessageFormat";

				Db.Transact(() =>
				{
					foreach(var jsn in msg.OphA)
					{
						int OphID = int.Parse(jsn.OphID);

						if(jsn.Evnt == "D")
						{
							var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where h.OphID = ?", OphID);
							foreach(var rec in ophs)
							{
								//Db.SQL("delete from OPH h where rec.OphID = ?", OphID);	Boyle yapamiyor!!
								rec.Delete();
							}
						}
						else
						{
							TMDB.OPH rec = Db.SQL<TMDB.OPH>("select h from OPH h where h.OphID = ?", OphID).First;
							if(jsn.Evnt == "I" && rec == null)
							{
								rec = new TMDB.OPH();
							}

							if(rec != null)
							{
								sb.Append(jsn.OpmID).Append(",");
								rec.MdfdOn = DateTime.Now;
								rec.OphID = OphID;
								rec.OpmID = jsn.OpmID == "" ? (int?)null : Convert.ToInt32(jsn.OpmID);
								rec.ROT = jsn.ROT;
								rec.MOT = jsn.MOT;
								rec.Org = jsn.Org;
								rec.Dst = jsn.Dst;
								rec.nStu = jsn.nStu;
								rec.pStu = jsn.pStu;
								rec.DTM = jsn.DTM;
								rec.PTM = jsn.PTM;
								rec.NOP = jsn.NOP == "" ? (int?)null : Convert.ToInt32(jsn.NOP);
								rec.GrW = jsn.GrW == "" ? (double?)null : Convert.ToDouble(jsn.GrW);
								rec.CntNoS = jsn.CntNoS;

								rec.ShpID = jsn.ShpID == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);
								rec.CneID = jsn.ShpID == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);
								rec.AccID = jsn.ShpID == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);

								rec.EXD = jsn.EXD == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EXD);
								rec.nStuTS = jsn.nStuTS == "" ? (DateTime?)null : Convert.ToDateTime(jsn.nStuTS);
								rec.pStuTS = jsn.pStuTS == "" ? (DateTime?)null : Convert.ToDateTime(jsn.pStuTS);
								rec.REOH = jsn.REOH == "" ? (DateTime?)null : Convert.ToDateTime(jsn.REOH);
								rec.EOH = jsn.EOH == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EOH);
								rec.AOH = jsn.AOH == "" ? (DateTime?)null : Convert.ToDateTime(jsn.AOH);
								rec.RTR = jsn.RTR == "" ? (DateTime?)null : Convert.ToDateTime(jsn.RTR);
								rec.POD = jsn.POD == "" ? (DateTime?)null : Convert.ToDateTime(jsn.POD);

								if(rec.OpmID != null)
									rec.Opm = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", rec.OpmID).First;
								if(rec.ShpID != null)
									rec.Shp = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.ShpID).First;
								if(rec.CneID != null)
									rec.Cne = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CneID).First;
								if(rec.AccID != null)
									rec.Acc = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.AccID).First;
							}
						}
					}
				});
				return sb.ToString();*/
                return "";
            });

            ////DENEME////
            int wsc1 = 0;
            int wsc2 = 0;
            ConcurrentDictionary<UInt64, string> cd = new ConcurrentDictionary<UInt64, string>();

            Handle.GET("/wss", (Request req) =>
            {
                StringBuilder sb = new StringBuilder();

                var list = cd.Keys.ToList();
                list.Sort();

                // Loop through keys.
                foreach (var key in list)
                {
                    sb.AppendLine($"{key} - {cd[key]}");
                    //Console.WriteLine("{0}: {1}", key, cd[key]);
                }
                /*
				foreach(var pair in cd)
				{
					sb.AppendLine($"{pair.Key} - {pair.Value}");
					//Console.WriteLine("{0}, {1}", pair.Key, pair.Value);
				}*/
                //return cd.Count.ToString();
                //return $"{wsc1} - {wsc2}";
                return sb.ToString();
            });

            Handle.GET("/wsConnect/{?}", (string p1, Request req) =>
            {
                if (req.WebSocketUpgrade)
                {
                    // Save wsIdlower, wsIdUpper in database.
                    Db.Transact(() =>
                    {
                        new TMDB.WebSocketId()
                        {
                            Id = req.GetWebSocketId(),  //ws.ToUInt64()
                            ToU = DateTime.Now
                        };
                    });


                    cd.TryAdd(req.GetWebSocketId(), p1);

                    var hd = req.HeadersDictionary;
                    string prm = string.Empty;
                    if (req.HeadersDictionary.ContainsKey("Sec-WebSocket-Protocol"))
                        prm = hd["Sec-WebSocket-Protocol"];

                    wsc1++;

                    if (req.HeadersDictionary.TryGetValue("Sec-WebSocket-Protocol", out prm))
                    {
                        //cd.TryAdd(req.GetWebSocketId(), prm);
                        var id = req.GetWebSocketId();
                        // Use prm
                    }

                    //WebSocket ws = req.SendUpgrade("ws");
                    req.SendUpgrade("ws");
                    //Console.WriteLine("ws Connected {0}", DateTime.Now);
                    return HandlerStatus.Handled;
                }
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });


            Handle.WebSocket("ws", (String s, WebSocket ws) =>
            {
                //Console.WriteLine("ws received {0} {1}", s, DateTime.Now);
                ws.Send("echo: " + s + "   " + ws.ToUInt64().ToString());

            });

            Handle.GET("/wsConnect2", (Request req) =>
            {
                if (req.WebSocketUpgrade)
                {
                    var hd = req.HeadersDictionary;
                    string prm = string.Empty;
                    if (req.HeadersDictionary.ContainsKey("Sec-WebSocket-Protocol"))
                        prm = hd["Sec-WebSocket-Protocol"];

                    if (req.HeadersDictionary.TryGetValue("Sec-WebSocket-Protocol", out prm))
                    {
                        //cd.TryAdd(req.GetWebSocketId(), prm);
                        wsc2++;
                        var id = req.GetWebSocketId();
                        // Use prm
                    }

                    //WebSocket ws = req.SendUpgrade("ws");
                    req.SendUpgrade("ws2");
                    Console.WriteLine("ws Connected {0}", DateTime.Now);
                    return HandlerStatus.Handled;
                }
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
            });

            Handle.WebSocket("ws2", (String s, WebSocket ws) =>
            {
                Console.WriteLine("ws2 received {0} {1}", s, DateTime.Now);
                ws.Send("echo: " + s + "   " + ws.ToUInt64().ToString());

            });

            Handle.GET("/wsMsg/{?}", (String msg) =>
            {
                Db.Transact(() =>
                {
                    new TMDB.WsMsg()
                    {
                        Msg = msg,
                        ToU = DateTime.Now
                    };
                });
                return "OK";
            });

            WebSocketSessionsTimer = new Timer((state) =>
            {

                // Getting sessions for current scheduler.
                Scheduling.ScheduleTask(() =>
                {

                    Db.Transact(() =>
                    {
                        foreach (TMDB.WebSocketId wsDb in Db.SQL<TMDB.WebSocketId>("SELECT w FROM TMDB.WebSocketId w"))
                        {
                            foreach (TMDB.WsMsg wm in Db.SQL<TMDB.WsMsg>("select m from TMDB.WsMsg m where m.ToU > ?", wsDb.ToU))
                            {
                                WebSocket ws = new WebSocket(wsDb.Id);
                                ws.Send(wm.Msg);

                                wsDb.ToU = DateTime.Now;
                            }

                            /*
								WebSocket ws = new WebSocket(wsDb.Id);

							// Checking if ready to disconnect after certain amount of sends.
							if(wsDb.NumBroadcasted == 100)
							{
								ws.Disconnect();
								wsDb.Delete();

								// Proceeding to next active WebSocket.
								continue;
							}

							String sendMsg = "Broadcasting id: " + wsDb.Id + "  " + wsDb.NumBroadcasted;
							ws.Send(sendMsg+wsc1);
							wsDb.NumBroadcasted++;
							wsDb.ToU = DateTime.Now; */
                        }
                    });

                });

            }, null, 1000, 10000);

            Handle.WebSocketDisconnect("ws", (WebSocket ws) =>
            {

                Db.Transact(() =>
                {

                    TMDB.WebSocketId wsId = Db.SQL<TMDB.WebSocketId>("SELECT w FROM TMDB.WebSocketId w WHERE w.Id=?", ws.ToUInt64()).FirstOrDefault();
                    if (wsId != null)
                    {
                        wsId.Delete();
                    }
                });
            });

            Handle.GET("/SendMail", () =>
            {

                sendMail("sener.demiral@gmail.com", "DENEME", "deneme");
                return "OK";
            });
            /*
            Handle.Tcp(8585, (TcpSocket tcpSocket, Byte[] incomingData) =>
            {
                UInt64 socketId = tcpSocket.ToUInt64();
                //Console.WriteLine("socketId: {0}", socketId);

                // Checking if we have socket disconnect here.
                if (null == incomingData)
                {
                    // One can use "socketId" to dispose resources associated with this socket.
                    return;
                }

                // One can check if there are any resources associated with "socketId" and otherwise create them.
                // Db.SQL("...");

                // Sending the echo back.
                tcpSocket.Send(incomingData);

                // Or even more data, if its needed: tcpSocket.Send(someMoreData);
            });
            */
            /*
            Handle.Udp(6000, (IPAddress clientIp, UInt16 clientPort, Byte[] datagram) =>
            {
                string text = Encoding.ASCII.GetString(datagram);

                Hlpr.WriteTrackingLog("Received on: " + clientIp.ToString() + " " + clientPort.ToString() + " " + text);

                // One can update any resources associated with "clientIp" and "clientPort".
                //UdpSocket.Send(clientIp, clientPort, 6000, Encoding.ASCII.GetBytes("ok"));
                UdpSocket.Send(clientIp, clientPort, 6000, "ok");

                if (text.StartsWith("(") && text.EndsWith(")"))
                    processMessages(text);
            });
            */
            Handle.GET("/tmax14rest/denemeMsj", () => {
                string msg = "(864768011110084,DW3C,081117,A,4105.4317N,02830.1093E,0.75,132043,0.00,80.00,12,0)";
                /*
                processMessages(msg);
                msg = "(864768011110084,DW3C,291017,A,4104.1656N,02852.4349E,0.06,052805,0.00,1.00,13,0)";
                processMessages(msg);
                msg = "(864768011110084,DW3C,291017,A,4104.1565N,02852.3095E,0.41,075052,0.00,161.40,11,0)";
                processMessages(msg);
                msg = "(864768011110084,DW3C,291017,A,4104.1456N,02852.3263E,0.01,095159,0.00,145.30,11,0)";*/
                processMessages(msg);
                return "OK";
            });
        }

        static void processMessages(string txt)
        {
            string[] msgss = txt.Split(new char[] { ')' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string msg in msgss)
            {
                processSingleMessage(msg.Substring(1)); // Ilk karakter '(' gec
            }
        }

        static void processSingleMessage(string msg)
        {
            string[] items = msg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string trckID = items[0];
            string cmnd = items[1];

            if (cmnd == "DW30" || cmnd == "DW3B" || cmnd == "DW3C")
            {
                string Trh = items[(int)DW3.Trh];   // ddMMyy
                string Zmn = items[(int)DW3.Zmn];   // HHmmss
                DateTime EXD = new DateTime(Convert.ToInt32(Trh.Substring(4)) + 2000, Convert.ToInt32(Trh.Substring(2, 2)), Convert.ToInt32(Trh.Substring(0, 2)), Convert.ToInt32(Zmn.Substring(0, 2)), Convert.ToInt32(Zmn.Substring(2, 2)), Convert.ToInt32(Zmn.Substring(4)));
                string GPS = items[(int)DW3.GPS];   // A:GPS valid data, V:GPS invalid data
                string LatDMC = items[(int)DW3.Lat];   // ddmm.mmmmC  C:N+/S-
                double LatDD = LatDMCtoDD(LatDMC);
                string LonDMC = items[(int)DW3.Lon];   // dddmm.mmmmC  C:E+/W-
                double LonDD = LonDMCtoDD(LonDMC);

                Hlpr.WriteTrackingLog(string.Format("Cmnd:{0} TrckID:{1} EXD:{2} Lat,Lon:{3},{4}", cmnd, trckID, EXD, LatDD, LonDD));

                Db.Transact(() =>
                {
                    //var th = Db.FromId<TMDB.TH>(ulong.Parse(trckID));
                    var th = Db.SQL<TMDB.TH>("select h from TMDB.TH h where h.ID = ?", trckID).FirstOrDefault();
                    if (th == null)
                    {
                        new TMDB.TH
                        {
                            ID = trckID,
                            Lat = LatDD.ToString(),
                            Lng = LonDD.ToString(),
                            LTS = EXD,
                            CntNo = "ECBU 500112 7"
                        };
                    }
                    else
                    {
                        th.Lat = LatDD.ToString();
                        th.Lng = LonDD.ToString();
                        th.LTS = EXD;
                        th.CntNo = "ECBU 500112 7";

                    }
                });
            }
        }

        static double LatDMCtoDD(string DMC)
        {
            // ddmm.mmmmC  C:N+/S-
            double D = Convert.ToDouble(DMC.Substring(0, 2));
            double M = Convert.ToDouble(DMC.Substring(2, 7));
            double Sgn = DMC.EndsWith("N") ? 1 : -1;   // Nort+ South-
            double DD = Math.Round(Sgn * (D + M / 60.0), 6);
            return DD;
        }

        static double LonDMCtoDD(string DMC)
        {
            // dddmm.mmmmC  C:E+/W-
            double D = Convert.ToDouble(DMC.Substring(0, 3));
            double M = Convert.ToDouble(DMC.Substring(3, 7));
            double Sgn = DMC.EndsWith("E") ? 1 : -1;   // East+ West-
            double DD = Math.Round(Sgn * (D + M / 60.0), 6);
            return DD;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (sw != null)
                sw.Close();
        }
        /*
        public static void WriteErrorLog(string Msg)
        {
            //StreamWriter sw = null;
            if (sw == null)
                sw = new StreamWriter(@"C:\Starcounter\MyLog\ScServerUDP-Log.txt", true);

            try
            {
                //sw = new StreamWriter(@"C:\Starcounter\MyLog\ScServerUDP-Log.txt", true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
                sw.Flush();
                //sw.Close();
            }
            catch
            {
            }
        }
        */
        public static void sendMail(string mailTo, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.To.Add(mailTo);

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = false;

                // gMail
                mail.From = new MailAddress("sener.demiral@gmail.com", "Sener");  // gMail
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");   // gMail
                smtp.Credentials = new System.Net.NetworkCredential("sener.demiral", "CanDilSen09");  // gMail
                smtp.EnableSsl = true;    // gMail
                smtp.Port = 587;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
