using System;
using System.Text;
using Starcounter;

namespace tMax14rest
{
	class Program
	{
		static void Main()
		{
			Handle.PUT("/tMax14rest/ResetAll", (OphMsg hJ) =>
			{
				Db.Transact(() =>
				{
					Db.SQL("DELETE FROM OPH");
					Db.SQL("DELETE FROM OPM");
					Db.SQL("DELETE FROM FRT");
				});

				return "OK";
			});
			Handle.GET("/tMax14rest/FRT", () =>
			{
				return "/tMax14rest/FRT";
			});

			Handle.WebSocket("ws", (String s, WebSocket ws) =>
			{
				// Handle s and send response
				ws.Send(s);
			});

			Handle.WebSocket("ws", (byte[] data, WebSocket ws) =>
			{
				FrtMsg jsn = new FrtMsg();
				jsn.Data = data;
				// Handle s and send response
				ws.Send("????");
			});

			Handle.GET("/wsConnect", (Request req) =>
			{
				// Checking if its a WebSocket upgrade request.
				if(req.WebSocketUpgrade)
				{
					WebSocket ws = req.SendUpgrade("ws");
					Console.WriteLine("ws Connected {0}", DateTime.Now);
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

			/*
			Handle.PUT("/tMax14rest/FRTold", (FrtMsg jsn) =>
			{
				Console.WriteLine("FRT: " + jsn.FrtID);
				string rMsg = "OK";
				// jsn'nin ilk field da FRT olmali, ikinci (Evnt) field bos olmamali
				if(jsn[0].ToString() != "FRT" || jsn[1].ToString() == "")
					return "!HATA-WrongMessageFormat";

				Db.Transact(() =>
				{
					try
					{
						int FrtID = int.Parse(jsn.FrtID);

						if(jsn.Evnt == "D")
						{
							var frts = Db.SQL<TMDB.FRT>("select f from FRT f where rec.FrtID = ?", FrtID);
							foreach(var rec in frts)
							{
								rec.Delete();
							}
						}
						else
						{
							TMDB.FRT rec = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", FrtID).First;
							if(jsn.Evnt == "I" && rec == null)
							{
								rec = new TMDB.FRT();
							}

							if(rec == null)
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
					catch(Exception ex)
					{
						rMsg = ex.Message;
					}
				});
				return rMsg;
			});
			*/
			Handle.PUT("/tMax14rest/OPM", (OpmMsg opmMsg) =>
			{
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
				return sb.ToString();
			});

			Handle.PUT("/tMax14rest/OPH", (OphMsg msg) =>
			{
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
				return sb.ToString();
			});

			/*
			Handle.PUT("/tMax14rest/OPHxxx", (OphMsg jsn) => 
			{

				string rMsg = "OK";
				// hM'nin ilk field da OPH olmali, ikinci (Evnt) field bos olmamali
				if(jsn[0].ToString() != "OPH" || jsn[1].ToString() == "")
					return "!HATA-WrongMessageFormat";

				Db.Transact(() =>
				{
					try
					{
						int OphID = int.Parse(jsn.OphID);

						if(jsn.Evnt == "D")
						{
							var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where rec.OphID = ?", OphID);
							foreach(var hr in ophs)
							{
								//Db.SQL("delete from OPH h where rec.OphID = ?", OphID);	Boyle yapamiyor!!
								hr.Delete();
							}
						}
						else
						{
							TMDB.OPH rec = Db.SQL<TMDB.OPH>("select h from OPH h where rec.OphID = ?", OphID).First;
							if(jsn.Evnt == "I"&& rec == null)
							{
								rec = new TMDB.OPH();
							}

							if(rec == null)
								rMsg = "NoHouse2Update";
							else
							{
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
								rec.GrW = jsn.GrW == "" ? (Double?)null : Convert.ToDouble(jsn.GrW);
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
								{
									rec.Opm = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", rec.OpmID).First;
									if(rec.Opm == null)
										rMsg += $"NoMaster:{rec.OpmID} ";
								}
								if(rec.ShpID != null)
									rec.Shp = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.ShpID).First;
								if(rec.CneID != null)
									rec.Cne = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.CneID).First;
								if(rec.AccID != null)
									rec.Acc = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", rec.AccID).First;
							}
						}
					}
					catch(Exception ex)
					{
						rMsg = ex.Message;
					}
				});
				return rMsg;
			}, new HandlerOptions() { SkipRequestFilters = true });
			*/
			/*
			Handle.PUT("/tMax14rest/OPMorg", (OpmMsg jsn) =>
			{
				
				string rMsg = "OK";
				// jsn'nin ilk field da OPM olmali, ikinci (Evnt) field bos olmamali
				if(jsn[0].ToString() != "OPM" || jsn[1].ToString() == "")
					return "!HATA-WrongMessageFormat";
				
				Db.Transact(() =>
				{
					try
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

							if(rec == null)
								rMsg = "NoHouse2Update";
							else
							{
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
					catch(Exception ex)
					{
						rMsg = ex.Message;
					}
				});
				return rMsg;
				
			}, new HandlerOptions() { SkipRequestFilters = true });
			*/

		}
	}
}