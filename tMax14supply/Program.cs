using System;
using Starcounter;
using Starcounter.Rest;
using System.Collections.Generic;
using Starcounter.Internal;
using CronNET;
using System.Threading;
using WebSocketSharp;
using System.IO;

namespace tMax14supply
{
	class Program
	{
		private static readonly CronDaemon cron_daemon = new CronDaemon();
		//private static Node localNode = new Node("127.0.0.1", 8080);
		private static Node localNode = new Node("tmax.masatenisi.online", 80, 600);
		private static tMax14DataSet dts = new tMax14DataSet();
		private static tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter fta = new tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter();
		private static tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter mta = new tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter();
		private static tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter hta = new tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter();
		private static System.Timers.Timer timer = new System.Timers.Timer(30000);
		
		private static WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket("ws://masatenisi.online/wsConnect");

		static void Main(string[] args)
		{
			Console.WriteLine("Basla1");
			ws.OnMessage += (sndr, ev) =>
			{
				if(ev.Data != "OK")
					Console.WriteLine("wsMessage: " + ev.Data);
			};

			timer.AutoReset = true;
			timer.Elapsed += Timer_Elapsed;
			
			//cron_daemon.AddJob("9 * * * * *", task);
			//cron_daemon.Start();

			Handle.GET("/tMax14supply/start/{?}", (string intrvlMinute) =>
			{
				timer.Start();

				timer.Interval = Convert.ToDouble(intrvlMinute) * 60 * 1000;	// 
				Console.WriteLine("Timer started. Interval ms: " + timer.Interval);
				return "Timer started. Interval ms: "+timer.Interval;
			});

			Handle.GET("/tMax14supply/stop", () =>
			{
				timer.Stop();
				Console.WriteLine("Stopped");

				return "Timer stopped";
			});

			Handle.GET("/tMax14supply/FRT", () =>
			{
				
				Console.WriteLine("Frt Push to Server");
				FrtMsg f = new FrtMsg();
				Response rrr = Http.GET(80, "http://tmax.masatenisi.online/tmax14rest/frt");
				//Response rrr = Http.GET(80, "http://tmax.masatenisi.online/tmax14rest/frt");
				Response res = localNode.GET("/tMax14rest/FRT");
//				Http.GET<string>()
				//FrtCron();

				return "FRT Pushed";
			});



			Handle.GET("/tMax14supply/FRTfull", () =>
			{
				Console.WriteLine("FRT Push to Server");
				FrtCron("F");
				return "FRT Pushed";
			});


			Handle.GET("/tMax14supply/OPMfull", () =>
			{
				Console.WriteLine("Opm Push to Server");
				OpmCron("F");
				return "OPM Pushed";
			});

			Handle.GET("/tMax14supply/OPHfull", () =>
			{
				Console.WriteLine("Oph Push to Server");
				OphCron("F");
				return "OPH Pushed";
			});

			Handle.PUT("/tMax14supply/task", (FrtMsg frt) =>
			{
				Console.WriteLine(frt.ToString());
				return "task";
			});
		}

		static void task()
		{
			Console.WriteLine("Task");

			FrtCron("X");
			OpmCron("X");
			OphCron("X");

			/*
				FrtMsg f = new FrtMsg() {
				Ad = DateTime.Now.ToString()
			};

			localNode.PUT("/tMax14rest/sener", f.ToJsonUtf8(), null, 10);
			//Self.PUT("/tMax14supply/task", "Body", null);

			//Http.PUT("/tMax14supply/task", "deneme");
			*/

		}

		private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			FrtCron("X");
			OpmCron("X");
			OphCron("X");
		}


		static void FrtCron(string typ)
		{
			int nor = fta.Fill(dts.WEB_FRT_MDFD, typ);
			/*
			FrtMsg.FrtAElementJson jsnE = new FrtMsg.FrtAElementJson();
			jsnE.Evnt = "event";
			jsnE.FrtID = "1111";
			jsnE.AdN = "adn";
			jsnE.Pwd = "pwd";
			jsn.FrtA.Add(jsnE);
			*/
			if(ws.ReadyState != WebSocketState.Open)
				ws.Connect();


			foreach(tMax14DataSet.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
			{
				var jsn = new FrtMsg
				{
					Evnt = row.EVNT,
					FrtID = row.FRTID.ToString(),
					AdN = row.ADN,
					Pwd = row.PWD
				};
				ws.Send(jsn.ToJson());
			}
			//var aaa = jsn.ToJson();
			//var oms = new MemoryStream();
			//byte[] buf = jsn.ToJsonUtf8();
			//oms.Read(buf, 0, buf.Length);
			//oms.Seek(0, SeekOrigin.Begin);

		}

		static void FrtCron2()
		{	/*
			int nor = fta.Fill(dts.WEB_FRT_MDFD, "X");
			foreach(tMax14DataSet.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
			{
				FrtMsg jsn = new FrtMsg
				{
					Tbl = "FRT",
					Evnt = row.EVNT,
					FrtID = row.FRTID.ToString(),
					AdN = row.ADN,
					Pwd = row.PWD
				};
				Response res = localNode.PUT("/tMax14rest/FRT", jsn.ToJsonUtf8(), null, 10);
				if(res.StatusCode == 200)
				{
					row.Delete();
				}
			}
			fta.Update(dts.WEB_FRT_MDFD);
			*/
		}

		static void OpmCron(string typ)
		{
			int nor = mta.Fill(dts.WEB_OPM_MDFD, typ);
			tMax14DataSet.WEB_OPM_MDFDRow row;
			int bs = 1000;	// records chunk
			int ek = nor / bs;
			int ei = 0, ri = 0;
			for(int k = 0; k <= ek; k++)
			{
				OpmMsg jsn = new OpmMsg();
				jsn.Tbl = "OPM";

				ei = nor - (k * bs);
				if(ei > bs)
					ei = bs;
				
				for(int i = 0; i < ei; i++)
				{
					ri = k * bs + i;
					row = (tMax14DataSet.WEB_OPM_MDFDRow)dts.WEB_OPM_MDFD.Rows[ri];
					OpmMsg.OpmAElementJson abcd = new OpmMsg.OpmAElementJson
					{
						Evnt = row.EVNT,
						OpmID = row.OPMID.ToString(),
						EXD = row.EXD.ToString(),
						ROT = row.ROT,
						MOT = row.MOT,
						Org = row.ORG,
						Dst = row.DST,
						ShpID = row.IsSHPIDNull() ? "" : row.SHPID.ToString(),
						CneID = row.IsCNEIDNull() ? "" : row.CNEID.ToString(),
						AccID = row.IsACCIDNull() ? "" : row.ACCID.ToString(),
						CrrID = row.IsCRRIDNull() ? "" : row.CRRID.ToString(),
						ETD = row.IsETDNull() ? "" : row.ETD.ToString(),
						ATD = row.IsATDNull() ? "" : row.ATD.ToString(),
						ETA = row.IsETANull() ? "" : row.ETA.ToString(),
						ATA = row.IsATANull() ? "" : row.ATA.ToString(),
					};
					jsn.OpmA.Add(abcd);
				}
				var aaa = jsn.ToJsonUtf8().Length;
				Response res = localNode.PUT("/tMax14rest/OPM", jsn.ToJsonUtf8(), null, 600);
			}
		}

		static void OpmCronOrg()
		{
		/*
			int nor = mta.Fill(dts.WEB_OPM_MDFD, "X");
			foreach(tMax14DataSet.WEB_OPM_MDFDRow row in dts.WEB_OPM_MDFD.Rows)
			{
				OpmMsg jsn = new OpmMsg
				{
					Tbl = "OPM",
					Evnt = row.EVNT,
					OpmID = row.OPMID.ToString(),
					EXD = row.EXD.ToString(),
					ROT = row.ROT,
					MOT = row.MOT,
					Org = row.ORG,
					Dst = row.DST,
					ShpID = row.IsSHPIDNull() ? "" : row.SHPID.ToString(),
					CneID = row.IsCNEIDNull() ? "" : row.CNEID.ToString(),
					AccID = row.IsACCIDNull() ? "" : row.ACCID.ToString(),
					CrrID = row.IsCRRIDNull() ? "" : row.CRRID.ToString(),
					ETD = row.IsETDNull() ? "" : row.ETD.ToString(),
					ATD = row.IsATDNull() ? "" : row.ATD.ToString(),
					ETA = row.IsETANull() ? "" : row.ETA.ToString(),
					ATA = row.IsATANull() ? "" : row.ATA.ToString(),
				};
				Response res = localNode.PUT("/tMax14rest/OPM", jsn.ToJsonUtf8(), null, 10);
				if(res.StatusCode == 200)
				{
					row.Delete();
				}
			}
			mta.Update(dts.WEB_OPM_MDFD);
			*/
		}

		static void OphCron(string typ)
		{
			int nor = hta.Fill(dts.WEB_OPH_MDFD, typ);
			tMax14DataSet.WEB_OPH_MDFDRow row;
			int bs = 1000;  // records chunk
			int ek = nor / bs;
			int ei = 0, ri = 0;
			for(int k = 0; k <= ek; k++)
			{
				OphMsg jsn = new OphMsg();
				jsn.Tbl = "OPH";

				ei = nor - (k * bs);
				if(ei > bs)
					ei = bs;

				for(int i = 0; i < ei; i++)
				{
					ri = k * bs + i;
					row = (tMax14DataSet.WEB_OPH_MDFDRow)dts.WEB_OPH_MDFD.Rows[ri];
					OphMsg.OphAElementJson abcd = new OphMsg.OphAElementJson
					{
						Evnt = row.EVNT,
						OphID = row.OPHID.ToString(),
						OpmID = row.OPMID.ToString(),
						EXD = row.EXD.ToString(),
						ROT = row.ROT,
						MOT = row.MOT,
						Org = row.ORG,
						Dst = row.ORG,
						ShpID = row.IsSHPIDNull() ? "" : row.SHPID.ToString(),
						CneID = row.IsCNEIDNull() ? "" : row.CNEID.ToString(),
						AccID = row.IsACCIDNull() ? "" : row.ACCID.ToString(),
						DTM = row.DTM,
						PTM = row.PTM,
						NOP = row.IsNOPNull() ? "" : row.NOP.ToString(),
						GrW = row.IsGRWNull() ? "" : row.GRW.ToString(),
						CntNoS = row.CNTNOS,

						nStu = row.NSTU,
						pStu = row.PSTU,
						nStuTS = row.IsNSTUTSNull() ? "" : row.NSTUTS.ToString(),
						pStuTS = row.IsPSTUTSNull() ? "" : row.PSTUTS.ToString(),
						REOH = row.IsREOHNull() ? "" : row.REOH.ToString(),
						AOH = row.IsAOHNull() ? "" : row.AOH.ToString(),
						RTR = row.IsRTRNull() ? "" : row.RTR.ToString(),
						POD = row.IsPODNull() ? "" : row.POD.ToString(),
					};
					jsn.OphA.Add(abcd);
				}
				var aaa = jsn.ToJsonUtf8().Length;
				Response res = localNode.PUT("/tMax14rest/OPH", jsn.ToJsonUtf8(), null, 600);
			}
		}

		static void OphCronOrg()
		{	/*
			int nor = hta.Fill(dts.WEB_OPH_MDFD, "X");
			foreach(tMax14DataSet.WEB_OPH_MDFDRow row in dts.WEB_OPH_MDFD.Rows)
			{
				OphMsg jsn = new OphMsg
				{
					Tbl = "OPH",
					Evnt = row.EVNT,
					OpmID = row.OPMID.ToString(),
					EXD = row.EXD.ToString(),
					ROT = row.ROT,
					MOT = row.MOT,
					Org = row.ORG,
					Dst = row.ORG,
					ShpID = row.IsSHPIDNull() ? "" : row.SHPID.ToString(),
					CneID = row.IsCNEIDNull() ? "" : row.CNEID.ToString(),
					AccID = row.IsACCIDNull() ? "" : row.ACCID.ToString(),
					DTM = row.DTM,
					PTM = row.PTM,
					CntNoS = row.CNTNOS,

					nStu = row.NSTU,
					pStu = row.PSTU,
					nStuTS = row.IsNSTUTSNull() ? "" : row.NSTUTS.ToString(),
					pStuTS = row.IsPSTUTSNull() ? "" : row.PSTUTS.ToString(),
					REOH = row.IsREOHNull() ? "" : row.REOH.ToString(),
					AOH = row.IsAOHNull() ? "" : row.AOH.ToString(),
					RTR = row.IsRTRNull() ? "" : row.RTR.ToString(),
					POD = row.IsPODNull() ? "" : row.POD.ToString(),
				};
				Response res = localNode.PUT("/tMax14rest/OPH", jsn.ToJsonUtf8(), null, 10);
				if(res.StatusCode == 200)
				{
					row.Delete();
				}
			}
			hta.Update(dts.WEB_OPH_MDFD);
			*/
		}

	}
}