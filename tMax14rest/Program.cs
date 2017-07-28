using System;
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

namespace tMax14rest
{
	class Program
	{
		// NOTE: Timer should be static, otherwise its garbage collected.
		static Timer WebSocketSessionsTimer = null;

        /// <summary>
        /// Rest'in 2 amaci var
        /// 1. Supply'dan ws ile gelen bilgiler ile SC Database'i guncellemek
        /// 2. SC deki bilgileri periyodik tarayip wsConnect olmus Clientlere ilgili bilgileri gondermek
        /// </summary>

        static void Main()
        {
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
                });

                return "OK";
            });

            Handle.GET("/wsFrtConnect", (Request req) =>
            {
                // Checking if its a WebSocket upgrade request.
                if (req.WebSocketUpgrade)
                {
                    Console.WriteLine("wsFRT Connected {0} {1}", DateTime.Now, req.GetWebSocketId());
                    req.SendUpgrade("wsFrt");
                    return HandlerStatus.Handled;
                }

                // We only support WebSockets upgrades in this HTTP handler
                // and not other ordinary HTTP requests.
                return new Response()
                {
                    StatusCode = 500,
                    StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
                };
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
                            TMDB.FRT rec = Db.SQL<TMDB.FRT>("select f from TMDB.FRT f where f.FrtID = ?", FrtID).First;
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
                    Console.WriteLine("wsFRC Connected {0} {1}", DateTime.Now, req.GetWebSocketId());
                    req.SendUpgrade("wsFrc");
                    return HandlerStatus.Handled;
                }

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
                            TMDB.FRC rec = Db.SQL<TMDB.FRC>("select f from TMDB.FRC f where f.FrcID = ?", FrcID).First;
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

                                rec.Frt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.FrtID).First;
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
                    Console.WriteLine("wsOPM Connected {0} {1}", DateTime.Now, req.GetWebSocketId());
                    req.SendUpgrade("wsOpm");
                    return HandlerStatus.Handled;
                }
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
                        TMDB.OPM rec = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", OpmID).First;
                        if (rec == null)
                            rec = new TMDB.OPM();

                        if (rec != null)
                        {
                            rec.MdfdOn = DateTime.Now;
                            rec.OpmID = OpmID;
                            rec.RefNo = jsn.RefNo;
                            rec.ROT = jsn.ROT;
                            rec.MOT = jsn.MOT;
                            rec.Org = jsn.Org;
                            rec.Dst = jsn.Dst;
                            rec.Vhc = jsn.Vhc;
                            rec.nStu = jsn.nStu;
                            rec.pStu = jsn.pStu;

                            //rec.CntNoS = jsn.CntNoS;

                            rec.ShpID = jsn.ShpID;// == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);
                            rec.CneID = jsn.CneID;// == "" ? (int?)null : Convert.ToInt32(jsn.CneID);
                            rec.AccID = jsn.AccID;// == "" ? (int?)null : Convert.ToInt32(jsn.AccID);
                            rec.CrrID = jsn.CrrID;// == "" ? (int?)null : Convert.ToInt32(jsn.CrrID);

                            rec.EXD = jsn.EXD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EXD);
                            rec.ETD = jsn.ETD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ETD);
                            rec.ATD = jsn.ATD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ATD);
                            rec.ETA = jsn.ETA;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ETA);
                            rec.ATA = jsn.ATA;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ATA);
                            rec.ACOT = jsn.ACOT;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ACOT);
                            rec.TPAD = jsn.TPAD;
                            rec.TPDD = jsn.TPDD;

                            if (rec.ShpID != null)
                                rec.Shp = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.ShpID).First;
                            if (rec.CneID != null)
                                rec.Cne = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CneID).First;
                            if (rec.AccID != null)
                                rec.Acc = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.AccID).First;
                            if (rec.CrrID != null)
                                rec.Crr = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CrrID).First;
                        }
                    }
                });

                //ws.Send(rMsg);
            });


            Handle.GET("/wsOphConnect", (Request req) =>
            {
                if (req.WebSocketUpgrade)
                {
                    Console.WriteLine("wsOPH Connected {0} {1}", DateTime.Now, req.GetWebSocketId());
                    req.SendUpgrade("wsOph");
                    return HandlerStatus.Handled;
                }
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
                        TMDB.OPH rec = Db.SQL<TMDB.OPH>("select h from OPH h where h.OphID = ?", OphID).First;
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
                            rec.Org = jsn.Org;
                            rec.Dst = jsn.Dst;
                            rec.nStu = jsn.nStu;
                            rec.pStu = jsn.pStu;
                            rec.DTM = jsn.DTM;
                            rec.PTM = jsn.PTM;
                            rec.NOP = jsn.NOP; // == "" ? (int?)null : Convert.ToInt32(jsn.NOP);
                            rec.GrW = jsn.GrW; // == "" ? (double?)null : Convert.ToDouble(jsn.GrW);
                            rec.VM3 = jsn.VM3; // == "" ? (double?)null : Convert.ToDouble(jsn.VM3);
                            rec.ChW = jsn.ChW; // == "" ? (int?)null : Convert.ToInt32(jsn.ChW);
                            rec.CntNoS = jsn.CntNoS;
                            rec.CABW = jsn.CABW;

                            rec.ShpID = jsn.ShpID;// == "" ? (int?)null : Convert.ToInt32(jsn.ShpID);
                            rec.CneID = jsn.CneID;// == "" ? (int?)null : Convert.ToInt32(jsn.CneID);
                            rec.AccID = jsn.AccID;// == "" ? (int?)null : Convert.ToInt32(jsn.AccID);
                            rec.MnfID = jsn.MnfID;// == "" ? (int?)null : Convert.ToInt32(jsn.MnfID);
                            rec.NfyID = jsn.NfyID;// == "" ? (int?)null : Convert.ToInt32(jsn.NfyID);
                            rec.CrrID = jsn.CrrID;// == "" ? (int?)null : Convert.ToInt32(jsn.NfyID);

                            rec.EXD = jsn.EXD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EXD);
                            rec.nStuTS = jsn.nStuTS;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.nStuTS);
                            rec.pStuTS = jsn.pStuTS;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.pStuTS);
                            rec.ROH = jsn.ROH;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ROH);
                            rec.EOH = jsn.EOH;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.EOH);
                            rec.AOH = jsn.AOH;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.AOH);
                            rec.RTR = jsn.RTR;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.RTR);
                            rec.ROS = jsn.ROS;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.ROS);
                            rec.POD = jsn.POD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.POD);
                            rec.REOH = jsn.REOH;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.REOH);
                            rec.DRBD = jsn.DRBD;// == "" ? (DateTime?)null : Convert.ToDateTime(jsn.REOH);

                            if (rec.OpmID != null)
                                rec.Opm = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", rec.OpmID).First;
                            if (rec.ShpID != null)
                                rec.Shp = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.ShpID).First;
                            if (rec.CneID != null)
                                rec.Cne = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CneID).First;
                            if (rec.AccID != null)
                                rec.Acc = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.AccID).First;
                            if (rec.MnfID != null)
                                rec.Mnf = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.MnfID).First;
                            if (rec.NfyID != null)
                                rec.Nfy = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.NfyID).First;
                            if (rec.CrrID != null)
                                rec.Crr = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CrrID).First;
                        }
                    }
                });
                //ws.Send(rMsg);
            });


            Handle.GET("/wsAfbConnect", (Request req) =>
            {
                if (req.WebSocketUpgrade)
                {
                    Console.WriteLine("wsAFB Connected {0} {1}", DateTime.Now, req.GetWebSocketId());
                    req.SendUpgrade("wsAfb");
                    return HandlerStatus.Handled;
                }
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
                        var rec = Db.SQL<TMDB.AFB>("select m from AFB m where m.AfbID = ?", AfbID).First;
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
                                rec.Frt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.FrtID).First;
                            if (rec.DknFrtID != null)
                                rec.DknFrt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.DknFrtID).First;
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
                            TMDB.FRT rec = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", FrtID).First;
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

                    TMDB.WebSocketId wsId = Db.SQL<TMDB.WebSocketId>("SELECT w FROM TMDB.WebSocketId w WHERE w.Id=?", ws.ToUInt64()).First;
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
        }

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
