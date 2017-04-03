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
		private static System.Timers.Timer timer = new System.Timers.Timer(10000);

		private static tMax14DataSet dts = new tMax14DataSet();
		private static tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter fta = new tMax14DataSetTableAdapters.WEB_FRT_MDFDTableAdapter();
		private static tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter mta = new tMax14DataSetTableAdapters.WEB_OPM_MDFDTableAdapter();
		private static tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter hta = new tMax14DataSetTableAdapters.WEB_OPH_MDFDTableAdapter();

		private static WebSocketSharp.WebSocket wsFrt = new WebSocketSharp.WebSocket("ws://masatenisi.online/wsFrtConnect");
		private static WebSocketSharp.WebSocket wsOpm = new WebSocketSharp.WebSocket("ws://masatenisi.online/wsOpmConnect");
		private static WebSocketSharp.WebSocket wsOph = new WebSocketSharp.WebSocket("ws://masatenisi.online/wsOphConnect");

		static void Main(string[] args)
		{
			Console.WriteLine("Basla1");
			wsFrt.OnMessage += (sndr, ev) =>
			{
				if(ev.Data != "OK")
					Console.WriteLine("wsMessage: " + ev.Data);
			};

			timer.AutoReset = true;
			timer.Elapsed += Timer_Elapsed;
			//timer.Start();	// Simdilik, 

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

		}

		static void task()
		{
			FrtCron("X");
			OpmCron("X");
			OphCron("X");
		}

		private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			FrtCron("X");
			OpmCron("X");
			OphCron("X");
		}


		static void FrtCron(string typ)
		{
			if(wsFrt.ReadyState != WebSocketState.Open)
				wsFrt.Connect();

			if(wsFrt.ReadyState == WebSocketState.Open)
			{
				int nor = fta.Fill(dts.WEB_FRT_MDFD, typ);
				foreach(tMax14DataSet.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
				{
					var jsn = new FrtMsg
					{
						Evnt = row.EVNT,
						FrtID = row.FRTID.ToString(),
						AdN = row.ADN,
						Pwd = row.PWD
					};
					wsFrt.Send(jsn.ToJson());

					if(typ == "X")
						row.Delete();
				}
				if(typ == "X")
					fta.Update(dts.WEB_FRT_MDFD);
			}
			wsFrt.Close();
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

		static void OpmCron2(string typ)
		{
			/*
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
			*/
		}

		static void OpmCron(string typ)
		{
			if(wsOpm.ReadyState != WebSocketState.Open)
				wsOpm.Connect();

			if(wsOpm.ReadyState == WebSocketState.Open)
			{
				int nor = mta.Fill(dts.WEB_OPM_MDFD, typ);
				foreach(tMax14DataSet.WEB_OPM_MDFDRow row in dts.WEB_OPM_MDFD.Rows)
				{
					OpmMsg jsn = new OpmMsg
					{
						Tbl = "OPM",
						Evnt = row.EVNT,
						OpmID = row.OPMID.ToString(),
						RefNo = row.REFNO,
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
					wsOpm.Send(jsn.ToJson());
					if(typ == "X")
						row.Delete();
				}
				if(typ == "X")
					fta.Update(dts.WEB_FRT_MDFD);
				wsOpm.Close();
			}
		}

		static void OphCron(string typ)
		{
			if(wsOph.ReadyState != WebSocketState.Open)
				wsOph.Connect();

			if(wsOph.ReadyState == WebSocketState.Open)
			{
				int nor = hta.Fill(dts.WEB_OPH_MDFD, typ);
				foreach(tMax14DataSet.WEB_OPH_MDFDRow row in dts.WEB_OPH_MDFD.Rows)
				{
					OphMsg jsn = new OphMsg
					{
						Tbl = "OPH",
						Evnt = row.EVNT,
						OphID = row.OPHID.ToString(),
						OpmID = row.OPMID.ToString(),
						RefNo = row.REFNO,
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
						EOH = row.IsEOHNull() ? "" : row.EOH.ToString(),
						AOH = row.IsAOHNull() ? "" : row.AOH.ToString(),
						RTR = row.IsRTRNull() ? "" : row.RTR.ToString(),
						ROS = row.IsROSNull() ? "" : row.ROS.ToString(),
						POD = row.IsPODNull() ? "" : row.POD.ToString(),
					};
					wsOph.Send(jsn.ToJson());
					if(typ == "X")
						row.Delete();
				}
				if(typ == "X")
					fta.Update(dts.WEB_FRT_MDFD);
				wsOph.Close();
			}
		}

		static void OphCron2(string typ)
		{
			/*
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
			}*/
		}


	}
}