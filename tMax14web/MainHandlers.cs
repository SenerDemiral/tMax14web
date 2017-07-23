using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace tMax14web
{
	internal class MainHandlers
	{
		public void CreateIndex() {
			//Db.SQL("DROP INDEX tmIndex_OPH_Opm ON OPH");
			//Db.SQL("DROP INDEX tmIndex_OPH_Shp ON OPH");
			//Db.SQL("DROP INDEX tmIndex_OPH_Cne ON OPH");
			//Db.SQL("DROP INDEX tmIndex_OPH_Acc ON OPH");

			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_FRT_FrtID").First == null)
				Db.SQL("CREATE INDEX tmIndex_FRT_FrtID ON FRT(FrtID)");
            if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_FRC_FrcID").First == null)
                Db.SQL("CREATE INDEX tmIndex_FRC_FrcID ON FRC(FrcID)");
            if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPM_OpmID").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPM_OpmID ON OPM(OpmID)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPH_OphID").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPH_OphID ON OPH(OphID)");

            if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_FRC_Frt").First == null)
                Db.SQL("CREATE INDEX tmIndex_FRC_Frt ON FRC(Frt)");
            if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPM_Shp").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPM_Shp ON OPM(Shp)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPM_Cne").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPM_Cne ON OPM(Cne)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPM_Acc").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPM_Acc ON OPM(Acc)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPM_Crr").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPM_Crr ON OPM(Crr)");

			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPH_Opm").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPH_Opm ON OPH(Opm)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPH_Shp").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPH_Shp ON OPH(Shp)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPH_Cne").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPH_Cne ON OPH(Cne)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPH_Acc").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPH_Acc ON OPH(Acc)");

            if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPH_EXD").First == null)
                Db.SQL("CREATE INDEX tmIndex_OPH_EXD ON OPH(EXD)");
        }

        public void Register() {
			Handle.GET("/tMax14web", (Request req) => {
				return Db.Scope(() => {
					//MasterPage master;

					if(Session.Current != null)
					{
						return (MasterPage)Session.Current.Data;
					}
					else
					{
						var master = new MasterPage();
						var cv = Starcounter.Internal.CurrentVersion.Version;
						master.Session = new Session(Session.Flags.PatchVersioning);
						return master;
					}
				});
			});

			Handle.GET("/tMax14web/OphClient", () => {
				var master = (MasterPage)Self.GET("/tMax14web");
				master.CurrentPage = new OphClientPage();
				master.CurrentPage.Data = null;

                if (master.fAdN == "Not Found")
                    master.CurrentPage = null;
				return master;
			});

			Handle.GET("/tMax14web/ophs2xlsx/{?}/{?}", (string frtID, string sDate) =>
			{
				using(OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
				{
					//Create the worksheet

					OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("House");
                    int FRTID = Convert.ToInt32(frtID);

                    var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where (h.ShpID = ? or h.CneID = ? or h.AccID = ?) and h.EXD >= ?", FRTID, FRTID, FRTID, Convert.ToDateTime(sDate));
					int cr = 2;
					foreach(var h in ophs)
					{

						ws.Cells[cr, (int)hFlds.OphID].Value = h.OphID;
						ws.Cells[cr, (int)hFlds.RefNo].Value = h.RefNo;
						ws.Cells[cr, (int)hFlds.EXD].Value = h.EXD;
						ws.Cells[cr, (int)hFlds.ROT].Value = h.ROT;
						ws.Cells[cr, (int)hFlds.MOT].Value = h.MOT;
						ws.Cells[cr, (int)hFlds.Org].Value = h.Org;
						ws.Cells[cr, (int)hFlds.Dst].Value = h.Dst;
						ws.Cells[cr, (int)hFlds.nStu].Value = h.nStu;
						ws.Cells[cr, (int)hFlds.nStuTS].Value = h.nStuTS;
						//ws.Cells[cr, (int)hFlds.pStu].Value = h.pStu;
						//ws.Cells[cr, (int)hFlds.pStuTS].Value = h.pStuTS;
						ws.Cells[cr, (int)hFlds.Shp].Value = h.ShpAd;
						ws.Cells[cr, (int)hFlds.Cne].Value = h.CneAd;
						ws.Cells[cr, (int)hFlds.Acc].Value = h.AccAd;
						ws.Cells[cr, (int)hFlds.Mnf].Value = h.MnfAd;
						ws.Cells[cr, (int)hFlds.Nfy].Value = h.NfyAd;
						ws.Cells[cr, (int)hFlds.DTM].Value = h.DTM;
						ws.Cells[cr, (int)hFlds.PTM].Value = h.PTM;
						ws.Cells[cr, (int)hFlds.NOP].Value = h.NOP;
						ws.Cells[cr, (int)hFlds.GrW].Value = h.GrW;
						ws.Cells[cr, (int)hFlds.VM3].Value = h.VM3;
						ws.Cells[cr, (int)hFlds.ChW].Value = h.ChW;
						ws.Cells[cr, (int)hFlds.ROH].Value = h.ROH;
						ws.Cells[cr, (int)hFlds.REOH].Value = h.REOH;
						ws.Cells[cr, (int)hFlds.EOH].Value = h.EOH;
						ws.Cells[cr, (int)hFlds.AOH].Value = h.AOH;
						ws.Cells[cr, (int)hFlds.RTR].Value = h.RTR;
						ws.Cells[cr, (int)hFlds.ROS].Value = h.ROS;
						ws.Cells[cr, (int)hFlds.POD].Value = h.POD;
						ws.Cells[cr, (int)hFlds.ETD].Value = h.ETD;
						ws.Cells[cr, (int)hFlds.ATD].Value = h.ATD;
						ws.Cells[cr, (int)hFlds.ETA].Value = h.ETA;
						ws.Cells[cr, (int)hFlds.ATA].Value = h.ATA;
						ws.Cells[cr, (int)hFlds.ACOT].Value = h.ACOT;
						ws.Cells[cr, (int)hFlds.CntNoS].Value = h.CntNoS;

						cr++;
					}
					// Header (first row)
					ws.Cells[1, (int)hFlds.OphID].Value = "ID";
					ws.Cells[1, (int)hFlds.RefNo].Value = "RefNo";
					ws.Cells[1, (int)hFlds.EXD].Value = "EXD";
					ws.Cells[1, (int)hFlds.ROT].Value = "ROT";
					ws.Cells[1, (int)hFlds.MOT].Value = "MOT";
					ws.Cells[1, (int)hFlds.Org].Value = "Org";
					ws.Cells[1, (int)hFlds.Dst].Value = "Dst";
					ws.Cells[1, (int)hFlds.nStu].Value = "Statu";
					ws.Cells[1, (int)hFlds.nStuTS].Value = "sDate";
					ws.Cells[1, (int)hFlds.pStu].Value = "Problem";
					ws.Cells[1, (int)hFlds.pStuTS].Value = "pDate";
					ws.Cells[1, (int)hFlds.Shp].Value = "Shp";
					ws.Cells[1, (int)hFlds.Cne].Value = "Cne";
					ws.Cells[1, (int)hFlds.Acc].Value = "Acc";
					ws.Cells[1, (int)hFlds.Mnf].Value = "Mnf";
					ws.Cells[1, (int)hFlds.Nfy].Value = "Nfy";
					ws.Cells[1, (int)hFlds.DTM].Value = "DTM";
					ws.Cells[1, (int)hFlds.PTM].Value = "PTM";
					ws.Cells[1, (int)hFlds.NOP].Value = "NOP";
					ws.Cells[1, (int)hFlds.GrW].Value = "GrW";
					ws.Cells[1, (int)hFlds.VM3].Value = "VM3";
					ws.Cells[1, (int)hFlds.ChW].Value = "ChW";
					ws.Cells[1, (int)hFlds.ROH].Value = "ROH";
					ws.Cells[1, (int)hFlds.EOH].Value = "EOH";
					ws.Cells[1, (int)hFlds.REOH].Value = "REOH";
					ws.Cells[1, (int)hFlds.AOH].Value = "AOH";
					ws.Cells[1, (int)hFlds.RTR].Value = "RTR";
					ws.Cells[1, (int)hFlds.ROS].Value = "ROS";
					ws.Cells[1, (int)hFlds.POD].Value = "POD";
					ws.Cells[1, (int)hFlds.ETD].Value = "ETD";
					ws.Cells[1, (int)hFlds.ATD].Value = "ATD";
					ws.Cells[1, (int)hFlds.ETA].Value = "ETA";
					ws.Cells[1, (int)hFlds.ATA].Value = "ATA";
                    ws.Cells[1, (int)hFlds.ACOT].Value = "ACOT";
                    ws.Cells[1, (int)hFlds.CntNoS].Value = "Cnt#";

					//var values = Enum.GetValues(typeof(hFlds));		// Array of values
					int[] df = new int[] //DateFields
					{
						(int)hFlds.EXD,
						(int)hFlds.nStuTS,
						(int)hFlds.pStuTS,
						(int)hFlds.ROH,
						(int)hFlds.REOH,
						(int)hFlds.EOH,
						(int)hFlds.AOH,
						(int)hFlds.RTR,
						(int)hFlds.ROS,
						(int)hFlds.POD,
						(int)hFlds.ETD,
						(int)hFlds.ATD,
						(int)hFlds.ETA,
						(int)hFlds.ATA,
						(int)hFlds.ACOT
					};

                    foreach (int c in df)
					{
						ws.Column(c).Style.Numberformat.Format = "dd.mm.yy";
					}
					/*
					ws.Column((int)hFlds.EXD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.nStuTS).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.pStuTS).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.REOH).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.EOH).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.AOH).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.RTR).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ROS).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.POD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ETD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ATD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ETA).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ATA).Style.Numberformat.Format = "dd.mm.yy";
					*/
					ws.Column((int)hFlds.ROT).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column((int)hFlds.MOT).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column((int)hFlds.NOP).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column((int)hFlds.GrW).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

					ws.Row(1).Style.Font.Bold = true;
					var range = ws.Cells["A1:AB1"];
					range.AutoFilter = true;
					ws.View.FreezePanes(2, 2);	// 1.Row ve 1.Col fixed

					var aa = ws.Dimension;
					var bb = ws.Dimension.Address;

					ws.Cells[ws.Dimension.Address].AutoFitColumns();
					/*
					for(int c = 1; c <= ws.Dimension.Columns; c++)
					{
						ws.Column(c).AutoFit(6);
					}*/

					Response r = new Response();
					//r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					r.ContentType = "application/octet-stream";
                    r.Headers["Content-Disposition"] = "attachment; filename=\"tMax14web-ophs.xlsx\"";

					var oms = new MemoryStream();
					pck.SaveAs(oms);
					oms.Seek(0, SeekOrigin.Begin);

					r.StreamedBody = oms;
					return r;
				}
			});

            Handle.GET("/tMax14web/deneme", () =>
            {
                
                using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
                {
                    //Create the worksheet

                    OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("House");
                    int FRTID = 57036;

                    var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where (h.ShpID = ? or h.CneID = ? or h.AccID = ?)", FRTID, FRTID, FRTID);
                    int cr = 2;
                    foreach (var h in ophs)
                    {

                        ws.Cells[cr, (int)hFlds.OphID].Value = h.OphID;
                        ws.Cells[cr, (int)hFlds.RefNo].Value = h.RefNo;
                        ws.Cells[cr, (int)hFlds.EXD].Value = h.EXD;
                        ws.Cells[cr, (int)hFlds.ROT].Value = h.ROT;
                        ws.Cells[cr, (int)hFlds.MOT].Value = h.MOT;
                        ws.Cells[cr, (int)hFlds.Org].Value = h.Org;
                        ws.Cells[cr, (int)hFlds.Dst].Value = h.Dst;
                        ws.Cells[cr, (int)hFlds.nStu].Value = h.nStu;
                        ws.Cells[cr, (int)hFlds.nStuTS].Value = h.nStuTS;
                        ws.Cells[cr, (int)hFlds.pStu].Value = h.pStu;
                        ws.Cells[cr, (int)hFlds.pStuTS].Value = h.pStuTS;
                        ws.Cells[cr, (int)hFlds.Shp].Value = h.ShpAd;
                        ws.Cells[cr, (int)hFlds.Cne].Value = h.CneAd;
                        ws.Cells[cr, (int)hFlds.Acc].Value = h.AccAd;
                        ws.Cells[cr, (int)hFlds.Mnf].Value = h.MnfAd;
                        ws.Cells[cr, (int)hFlds.Nfy].Value = h.NfyAd;
                        ws.Cells[cr, (int)hFlds.DTM].Value = h.DTM;
                        ws.Cells[cr, (int)hFlds.PTM].Value = h.PTM;
                        ws.Cells[cr, (int)hFlds.NOP].Value = h.NOP;
                        ws.Cells[cr, (int)hFlds.GrW].Value = h.GrW;
                        ws.Cells[cr, (int)hFlds.VM3].Value = h.VM3;
                        ws.Cells[cr, (int)hFlds.ChW].Value = h.ChW;
                        ws.Cells[cr, (int)hFlds.ROH].Value = h.ROH;
                        ws.Cells[cr, (int)hFlds.REOH].Value = h.REOH;
                        ws.Cells[cr, (int)hFlds.EOH].Value = h.EOH;
                        ws.Cells[cr, (int)hFlds.AOH].Value = h.AOH;
                        ws.Cells[cr, (int)hFlds.RTR].Value = h.RTR;
                        ws.Cells[cr, (int)hFlds.ROS].Value = h.ROS;
                        ws.Cells[cr, (int)hFlds.POD].Value = h.POD;
                        ws.Cells[cr, (int)hFlds.ETD].Value = h.ETD;
                        ws.Cells[cr, (int)hFlds.ATD].Value = h.ATD;
                        ws.Cells[cr, (int)hFlds.ETA].Value = h.ETA;
                        ws.Cells[cr, (int)hFlds.ATA].Value = h.ATA;
                        ws.Cells[cr, (int)hFlds.ACOT].Value = h.ACOT;
                        ws.Cells[cr, (int)hFlds.CntNoS].Value = h.CntNoS;

                        cr++;
                    }
                    // Header (first row)
                    ws.Cells[1, (int)hFlds.OphID].Value = "ID";
                    ws.Cells[1, (int)hFlds.RefNo].Value = "RefNo";
                    ws.Cells[1, (int)hFlds.EXD].Value = "EXD";
                    ws.Cells[1, (int)hFlds.ROT].Value = "ROT";
                    ws.Cells[1, (int)hFlds.MOT].Value = "MOT";
                    ws.Cells[1, (int)hFlds.Org].Value = "Org";
                    ws.Cells[1, (int)hFlds.Dst].Value = "Dst";
                    ws.Cells[1, (int)hFlds.nStu].Value = "Statu";
                    ws.Cells[1, (int)hFlds.nStuTS].Value = "sDate";
                    ws.Cells[1, (int)hFlds.pStu].Value = "Problem";
                    ws.Cells[1, (int)hFlds.pStuTS].Value = "pDate";
                    ws.Cells[1, (int)hFlds.Shp].Value = "Shp";
                    ws.Cells[1, (int)hFlds.Cne].Value = "Cne";
                    ws.Cells[1, (int)hFlds.Acc].Value = "Acc";
                    ws.Cells[1, (int)hFlds.Mnf].Value = "Mnf";
                    ws.Cells[1, (int)hFlds.Nfy].Value = "Nfy";
                    ws.Cells[1, (int)hFlds.DTM].Value = "DTM";
                    ws.Cells[1, (int)hFlds.PTM].Value = "PTM";
                    ws.Cells[1, (int)hFlds.NOP].Value = "NOP";
                    ws.Cells[1, (int)hFlds.GrW].Value = "GrW";
                    ws.Cells[1, (int)hFlds.VM3].Value = "VM3";
                    ws.Cells[1, (int)hFlds.ChW].Value = "ChW";
                    ws.Cells[1, (int)hFlds.ROH].Value = "ROH";
                    ws.Cells[1, (int)hFlds.EOH].Value = "EOH";
                    ws.Cells[1, (int)hFlds.REOH].Value = "REOH";
                    ws.Cells[1, (int)hFlds.AOH].Value = "AOH";
                    ws.Cells[1, (int)hFlds.RTR].Value = "RTR";
                    ws.Cells[1, (int)hFlds.ROS].Value = "ROS";
                    ws.Cells[1, (int)hFlds.POD].Value = "POD";
                    ws.Cells[1, (int)hFlds.ETD].Value = "ETD";
                    ws.Cells[1, (int)hFlds.ATD].Value = "ATD";
                    ws.Cells[1, (int)hFlds.ETA].Value = "ETA";
                    ws.Cells[1, (int)hFlds.ATA].Value = "ATA";
                    ws.Cells[1, (int)hFlds.ACOT].Value = "ACOT";
                    ws.Cells[1, (int)hFlds.CntNoS].Value = "Cnt#";

                    //var values = Enum.GetValues(typeof(hFlds));		// Array of values
                    int[] df = new int[] //DateFields
					{
                        (int)hFlds.EXD,
                        (int)hFlds.nStuTS,
                        (int)hFlds.pStuTS,
                        (int)hFlds.ROH,
                        (int)hFlds.REOH,
                        (int)hFlds.EOH,
                        (int)hFlds.AOH,
                        (int)hFlds.RTR,
                        (int)hFlds.ROS,
                        (int)hFlds.POD,
                        (int)hFlds.ETD,
                        (int)hFlds.ATD,
                        (int)hFlds.ETA,
                        (int)hFlds.ATA,
                        (int)hFlds.ACOT
                    };

                    foreach (int c in df)
                    {
                        ws.Column(c).Style.Numberformat.Format = "dd.mm.yy";
                    }
                    /*
					ws.Column((int)hFlds.EXD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.nStuTS).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.pStuTS).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.REOH).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.EOH).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.AOH).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.RTR).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ROS).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.POD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ETD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ATD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ETA).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.ATA).Style.Numberformat.Format = "dd.mm.yy";
					*/
                    ws.Column((int)hFlds.ROT).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column((int)hFlds.MOT).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column((int)hFlds.NOP).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column((int)hFlds.GrW).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    ws.Row(1).Style.Font.Bold = true;
                    var range = ws.Cells["A1:AB1"];
                    range.AutoFilter = true;
                    ws.View.FreezePanes(2, 2);  // 1.Row ve 1.Col fixed

                    var aa = ws.Dimension;
                    var bb = ws.Dimension.Address;

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    /*
					for(int c = 1; c <= ws.Dimension.Columns; c++)
					{
						ws.Column(c).AutoFit(6);
					}*/

                    Response r = new Response();
                    r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    r.Headers["Content-Disposition"] = "attachment; filename=tMax14web-ophs.xlsx";

                    var oms = new MemoryStream();
                    pck.SaveAs(oms);
                    oms.Seek(0, SeekOrigin.Begin);

                    r.StreamedBody = oms;
                    return r;
                }
            });

        }
        enum hFlds : int
		{
			None, OphID, RefNo, EXD, ROT, MOT, Org, Dst, nStu, nStuTS, pStu, pStuTS, Shp, Cne, Acc, Mnf, Nfy, DTM, PTM, NOP, GrW, VM3, ChW, ROH, EOH, REOH, AOH, RTR, ROS, POD, ETD, ATD, ETA, ATA, ACOT, CntNoS
		};
	}
}
