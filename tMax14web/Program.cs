using System.IO;
using System;
using Starcounter;


namespace tMax14web
{
	class Program
	{
		static void Main()
		{
			var html = @"<!DOCTYPE html>
				<html>
				<head>
					<meta charset=""utf-8"">
				    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=yes"">
					<title>{0}</title>
					
					<script src=""/sys/webcomponentsjs/webcomponents.min.js""></script>
					<link rel=""import"" href=""/sys/polymer/polymer.html"">
					<link rel=""import"" href=""/sys/starcounter.html"">
					<link rel=""import"" href=""/sys/starcounter-include/starcounter-include.html"">
					<script src=""/sys/thenBy.js""></script>
					
					<link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html"">
					<!--<link rel=""import"" href=""/sys/bootstrap.html"">
					<link rel=""import"" href=""/sys/iron-icons/maps-icons.html"">
					<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css"" integrity=""sha384-rwoIResjU2yc3z8GV/NPeZWAv56rSmLldC3R/AZzGRnGxQQKnKkoFVhFQhNUwEyJ"" crossorigin=""anonymous"">
					
					<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css"" crossorigin=""anonymous"">
					-->
					
					<link rel=""stylesheet"" href=""/sys/Stylesheet1.css"">
				
				</head>
				<body>
					<template is=""dom-bind"" id=""puppet-root"">
						<template is=""imported-template"" content$=""{{{{model.Html}}}}"" model=""{{{{model}}}}""></template>
					</template>
					<puppet-client ref=""puppet-root"" remote-url=""{1}"" use-web-socket=""true""></puppet-client>

					<starcounter-debug-aid></starcounter-debug-aid>
					<!--
					<script src=""https://code.jquery.com/jquery-3.1.1.slim.min.js"" integrity=""sha384-A7FZj7v+d/sdmMqp/nOQwliLvUsJfDHW+k9Omg/a/EheAdgtzNs3hpfag6Ed950n"" crossorigin=""anonymous""></script>
					<script src=""https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js"" integrity=""sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb"" crossorigin=""anonymous""></script>
					<script src=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js"" integrity=""sha384-vBWWzlZJ8ea9aCX4pEW3rVHjgjt7zpkNpZk+02D9phzyeVkE+jo0ieGizqPLForn"" crossorigin=""anonymous""></script>
					-->
				</body>
				</html>";

			Application.Current.Use(new HtmlFromJsonProvider());
			Application.Current.Use(new PartialToStandaloneHtmlProvider(html));

			int FrtID = 20509;
			DataSet1 ds = new DataSet1();
			DataSet1TableAdapters.OPHTableAdapter opha = new DataSet1TableAdapters.OPHTableAdapter();

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

				return master;


				/*
				opha.Fill(ds.OPH, FrtID);
				StringBuilder sb = new StringBuilder();
				object EXDs = null;
				foreach(DataSet1.OPHRow row in ds.OPH.Rows)
				{
					if(row.IsEXDNull())
						EXDs = null;
					else
						EXDs = row.EXD;
					//EXDs = string.Format("{0:dd.MM.yy HH:mm}", row.EXD);	
					//sb.AppendFormat("ID:{0} EXD:{1} Org:{2} Dst:{3} nStu:{4} {5}\n\f", row.HID, EXDs, row.ORG, row.DST, row.NSTU, row.NSTUT);
					sb.AppendFormat("ID:{0} EXD:{1} Org:{2} Dst:{3} nStu:{4} {5}\n\f", row.HID, row.IsEXDNull() ? "" : row.EXD.ToString("dd.MM.yy"), row.ORG, row.DST, row.NSTU, row.NSTUT);
				}

				return sb.ToString();  */
			});

			Handle.GET("/tMax14web/OphClient2", () => {
				var master = new OphClientPage();
				master.Data = null;

				return master;
			});
			//Db.SQL("DROP INDEX tmIndex_OPH_Opm ON OPH");
			//Db.SQL("DROP INDEX tmIndex_OPH_Shp ON OPH");
			//Db.SQL("DROP INDEX tmIndex_OPH_Cne ON OPH");
			//Db.SQL("DROP INDEX tmIndex_OPH_Acc ON OPH");

			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_FRT_FrtID").First == null)
				Db.SQL("CREATE INDEX tmIndex_FRT_FrtID ON FRT(FrtID)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPM_OpmID").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPM_OpmID ON OPM(OpmID)");
			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPH_OphID").First == null)
				Db.SQL("CREATE INDEX tmIndex_OPH_OphID ON OPH(OphID)");

			if(Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "tmIndex_OPM_Shp").First == null)
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


			Handle.GET("/tMax14web/WEB_FRT_MDFD", () => {
				DataSet1 dts = new DataSet1();
				DataSet1TableAdapters.WEB_FRT_MDFDTableAdapter ta = new DataSet1TableAdapters.WEB_FRT_MDFDTableAdapter();
				int nor = ta.Fill(dts.WEB_FRT_MDFD, "F");
				Db.Transact(() =>
				{
					foreach(DataSet1.WEB_FRT_MDFDRow row in dts.WEB_FRT_MDFD.Rows)
					{
						new TMDB.FRT
						{
							FrtID = row.FRTID,
							AdN = row.ADN,
							LocID = row.LOCID
						};
					}
				});

				return "WEB-FRT-MDFD "+nor.ToString();
			});

			Handle.GET("/tMax14web/WEB_OPM_MDFD", () => {
			
				DataSet1 dts = new DataSet1();
				DataSet1TableAdapters.WEB_OPM_MDFDTableAdapter ta = new DataSet1TableAdapters.WEB_OPM_MDFDTableAdapter();
				int nor = ta.Fill(dts.WEB_OPM_MDFD, "F");
				Db.Transact(() =>
				{
					Nullable<Int32> nullInt = null;
					DateTime? nullDate = null;
					foreach(DataSet1.WEB_OPM_MDFDRow row in dts.WEB_OPM_MDFD.Rows)
					{
						var t = new TMDB.OPM
						{
							OpmID = row.OPMID,
							RefNo = row.IsREFNONull() ? null : row.REFNO,
							EXD = row.EXD,
							ROT = row.ROT,
							MOT = row.MOT,
							Org = row.ORG,
							Dst = row.DST,
							ShpID = row.IsSHPIDNull() ? nullInt : row.SHPID,
							CneID = row.IsCNEIDNull() ? nullInt : row.CNEID,
							AccID = row.IsACCIDNull() ? nullInt : row.ACCID,
							CrrID = row.IsCRRIDNull() ? nullInt : row.CRRID,
							nStu = row.NSTU,
							pStu = row.PSTU,
							ETD = row.IsETDNull() ? nullDate : row.ETD,
							ATD = row.IsATDNull() ? nullDate : row.ATD,
							ETA = row.IsETANull() ? nullDate : row.ETA,
							ATA = row.IsATANull() ? nullDate : row.ATA,

							Vhc = row.IsVHCNull() ? null : row.VHC,
							CntNoS = row.IsCNTNOSNull() ? null : row.CNTNOS
						};
						if(t.ShpID != null)
							t.Shp = Db.SQL<TMDB.FRT>("SELECT f FROM FRT f WHERE f.FrtID = ?", t.ShpID).First;
						if(t.CneID != null)
							t.Cne = Db.SQL<TMDB.FRT>("SELECT f FROM FRT f WHERE f.FrtID = ?", t.CneID).First;
						if(t.AccID != null)
							t.Acc = Db.SQL<TMDB.FRT>("SELECT f FROM FRT f WHERE f.FrtID = ?", t.AccID).First;
						if(t.CrrID != null)
							t.Crr = Db.SQL<TMDB.FRT>("SELECT f FROM FRT f WHERE f.FrtID = ?", t.CrrID).First;
					}
				});
				
				//var m = Db.SQL<TMDB.OPM>("SELECT m FROM OPM m WHERE m.OpmID = ?", 978348).First;

				return "WEB-OPM-MDFD " + nor.ToString();
			});

			Handle.GET("/tMax14web/WEB_OPH_MDFD", () => {

				DataSet1 dts = new DataSet1();
				DataSet1TableAdapters.WEB_OPH_MDFDTableAdapter ta = new DataSet1TableAdapters.WEB_OPH_MDFDTableAdapter();
				int nor = ta.Fill(dts.WEB_OPH_MDFD, "F");
				Db.Transact(() =>
				{
					Nullable<Int32> nullInt = null;
					DateTime? nullDate = null;
					foreach(DataSet1.WEB_OPH_MDFDRow row in dts.WEB_OPH_MDFD.Rows)
					{
						var t = new TMDB.OPH
						{
							OphID = row.OPHID,
							OpmID = row.OPMID,
							RefNo = row.IsREFNONull() ? null : row.REFNO,
							EXD = row.EXD,
							ROT = row.ROT,
							MOT = row.MOT,
							Org = row.ORG,
							Dst = row.DST,
							ShpID = row.IsSHPIDNull() ? nullInt : row.SHPID,
							CneID = row.IsCNEIDNull() ? nullInt : row.CNEID,
							AccID = row.IsACCIDNull() ? nullInt : row.ACCID,
							nStu = row.NSTU,
							pStu = row.PSTU,
							NOP = row.NOP,
							GrW = (double)row.GRW,
							DTM = row.DTM,
							PTM = row.PTM,

							nStuTS = row.IsNSTUTSNull() ? nullDate : row.NSTUTS,
							pStuTS = row.IsPSTUTSNull() ? nullDate : row.PSTUTS,
							EOH = row.IsEOHNull() ? nullDate : row.EOH,
							REOH = row.IsREOHNull() ? nullDate : row.REOH,
							AOH = row.IsAOHNull() ? nullDate : row.AOH,
							RTR = row.IsRTRNull() ? nullDate : row.RTR,
							ROS = row.IsROSNull() ? nullDate : row.ROS,
							POD = row.IsPODNull() ? nullDate : row.POD,

							CntNoS = row.IsCNTNOSNull() ? null : row.CNTNOS
						};
						if(t.OpmID != null)
							t.Opm = Db.SQL<TMDB.OPM>("SELECT f FROM OPM f WHERE f.OpmID = ?", t.OpmID).First;
						if(t.ShpID != null)
							t.Shp = Db.SQL<TMDB.FRT>("SELECT f FROM FRT f WHERE f.FrtID = ?", t.ShpID).First;
						if(t.CneID != null)
							t.Cne = Db.SQL<TMDB.FRT>("SELECT f FROM FRT f WHERE f.FrtID = ?", t.CneID).First;
						if(t.AccID != null)
							t.Acc = Db.SQL<TMDB.FRT>("SELECT f FROM FRT f WHERE f.FrtID = ?", t.AccID).First;

					}
				});

				//var m = Db.SQL<TMDB.OPM>("SELECT m FROM OPM m WHERE m.OpmID = ?", 978348).First;

				return "WEB-OPH-MDFD " + nor.ToString();
			});

			Handle.GET("/tMax14web/movie.xls", () => {

				DataSet1 dts = new DataSet1();
				DataSet1TableAdapters.OPHTableAdapter ophta = new DataSet1TableAdapters.OPHTableAdapter();
				int nor = opha.Fill(dts.OPH, "57036", "2016-01-01");

				Response r = new Response();

				var pck = new OfficeOpenXml.ExcelPackage();
				OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Demo");
				ws.Cells["A1"].LoadFromDataTable(dts.OPH, true);

				//var p = pck.GetAsByteArray();
				var oms = new MemoryStream();
				//FileStream strw = File.Open("c:\\transorient\\A1234-B.xlsx", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				FileStream strw = new FileStream("c:\\transorient\\A1234.xlsx", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

				//pck.SaveAs(strw);
				pck.SaveAs(oms);
				oms.WriteTo(strw);

				oms.Seek(0, SeekOrigin.Begin);
				r.StreamedBody = oms;

				strw.Close();
				//oms.Close();

				FileStream stream = File.Open("c:\\transorient\\A1234.xlsx", FileMode.Open, FileAccess.Read, FileShare.Read);
				r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				r.Headers["Content-Disposition"] = "attachment; filename=sener3.xlsx";

				//r.StreamedBody = stream;
				return r;

				/*
				using(var xlPackage = new OfficeOpenXml.ExcelPackage(ms))
				{
					var wb = xlPackage.Workbook;
					var ws = wb.Worksheets.Add("WS1");
					ws.Cells["A1"].LoadFromDataTable(dts.OPH, true);

					var oms = new MemoryStream();
					xlPackage.SaveAs(oms);
					//return ms.ToArray();

					r.StreamedBody = oms;
					r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

						//ContentType = "application/octet-stream"
					//ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
					r.Headers["Content-Disposition"] = "attachment; filename=sener2.xlsx";

				} */
				return r;

				/*
				FileStream stream = File.Open("c:\\transorient\\AFB950618.xls", FileMode.Open, FileAccess.Read, FileShare.Read);

				Response r = new Response()
				{
					StreamedBody = stream,
						
					ContentType = "application/xls"
				};
				//ContentType = "application/octet-stream"
				//ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
				r.Headers["Content-Disposition"] = "attachment; filename=sener.xls";
				
				return r;  */
			});

			Handle.GET("/tMax14web/ophs2xlsx222/{?}/{?}", (string frtID, string sDate) => {

				DataSet1 dts = new DataSet1();
				DataSet1TableAdapters.OPHTableAdapter ophta = new DataSet1TableAdapters.OPHTableAdapter();
				int nor = opha.Fill(dts.OPH, frtID, sDate);


				using(OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
				{
					//Create the worksheet
					OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Demo");
					ws.Cells["A1"].LoadFromDataTable(dts.OPH, true);

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

			Handle.GET("/tMax14web/ophs2xlsx33/{?}/{?}", (string frtID, string sDate) => {

				DataSet1 dts = new DataSet1();
				//DataSet1.XHRow row = new DataSet1.XHRow();
				//row.EXD = DBNull.Value;
				
				var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where h.ShpID = ? and h.EXD >= ?", Convert.ToInt32(frtID), Convert.ToDateTime(sDate));
				foreach(var h in ophs)
				{
					var r = dts.XH.NewXHRow();
					if(h.EXD != null) r["EXD"] = h.EXD;

					r.ID = h.OphID;
					r.ROT = h.ROT;
					r.MOT = h.MOT;
					r.Org = h.Org;
					r.Dst = h.Dst;
					r.Shp = h.ShpAd;
					r.Cne = h.CneAd;
					r.Acc = h.AccAd;
					r.Statu = h.nStu;
					r.Problem = h.pStu;
					if(h.nStuTS != null) r["StuDate"] = h.nStuTS;
					if(h.pStuTS != null) r["PrbDate"] = h.pStuTS;
					if(h.REOH != null) r["REOH"] = h.REOH;
					if(h.EOH != null) r["EOH"] = h.EOH;
					if(h.AOH != null) r["AOH"] = h.AOH;
					if(h.RTR != null) r["RTR"] = h.RTR;
					if(h.ROS != null) r["ROS"] = h.ROS;
					if(h.POD != null) r["POD"] = h.POD;
					if(h.EOH != null) r["EOH"] = h.EOH;
					r["CntNo"] = h.CntNoS;

					dts.XH.AddXHRow(r);

				}
				
				using(OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
				{
					//Create the worksheet
					
					OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("House");
					ws.Cells["A1"].LoadFromDataTable(dts.XH, true);

					var range = ws.Cells["A1:X1"];
					range.AutoFilter = true;

					ws.Column(2).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(11).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(13).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(14).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(15).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(16).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(17).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(18).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(19).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(20).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(21).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column(22).Style.Numberformat.Format = "dd.mm.yy";
					
					ws.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(11).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(13).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(14).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(15).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(16).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(17).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(18).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(19).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(20).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(21).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column(22).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

					ws.Row(1).Style.Font.Bold = true;
					ws.View.FreezePanes(2, 2);
					for(int c = 1; c <= ws.Dimension.Columns; c++)
					{
						ws.Column(c).AutoFit(5);

					}
					/*
					var abcd = ws.Cells["M2"].Value.GetType().ToString();
					var cdef = ws.Dimension.Columns;
					for (int c = 1; c <= ws.Dimension.Columns; c++)
					{
						// 1.Satir Baslik hepsi string
						if(ws.Cells[2, c].Value.GetType().ToString() == "DateTime")
							ws.Column(c).Style.Numberformat.Format = "dd.mm.yy";

					}
					*/

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

			Handle.GET("/tMax14web/ophs2xlsx/{?}/{?}", (string frtID, string sDate) => {

				DataSet1 dts = new DataSet1();
				//DataSet1.XHRow row = new DataSet1.XHRow();
				//row.EXD = DBNull.Value;

				using(OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
				{
					//Create the worksheet

					OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("House");
					var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where h.ShpID = ? and h.EXD >= ?", Convert.ToInt32(frtID), Convert.ToDateTime(sDate));
					int cr = 2;
					foreach(var h in ophs)
					{

						ws.Cells[cr, (int)hFlds.OphID].Value = h.OphID;
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
						ws.Cells[cr, (int)hFlds.DTM].Value = h.DTM;
						ws.Cells[cr, (int)hFlds.PTM].Value = h.PTM;
						ws.Cells[cr, (int)hFlds.NOP].Value = h.NOP;
						ws.Cells[cr, (int)hFlds.GrW].Value = h.GrW;
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
						ws.Cells[cr, (int)hFlds.CntNoS].Value = h.CntNoS;

						cr++;
					}
					// Header (first row)
					ws.Cells[1, (int)hFlds.OphID].Value = "ID";
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
					ws.Cells[1, (int)hFlds.DTM].Value = "DTM";
					ws.Cells[1, (int)hFlds.PTM].Value = "PTM";
					ws.Cells[1, (int)hFlds.NOP].Value = "NOP";
					ws.Cells[1, (int)hFlds.GrW].Value = "GrW";
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
					ws.Cells[1, (int)hFlds.CntNoS].Value = "Cnt#";

					ws.Column((int)hFlds.EXD).Style.Numberformat.Format = "dd.mm.yy";
					ws.Column((int)hFlds.nStuTS).Style.Numberformat.Format = "dd.mm.yy";
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

					ws.Column((int)hFlds.ROT).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column((int)hFlds.MOT).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column((int)hFlds.NOP).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					ws.Column((int)hFlds.GrW).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.EXD).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.nStuTS).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.REOH).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.EOH).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.AOH).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.RTR).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.ROS).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.POD).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.ETD).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.ATD).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.ETA).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					//ws.Column((int)hFlds.ATA).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

					ws.Row(1).Style.Font.Bold = true;
					var range = ws.Cells["A1:AB1"];
					range.AutoFilter = true;
					ws.View.FreezePanes(2, 2);

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

			Handle.GET("/tMax14web/dilara", () => {

				DataSet1 dts = new DataSet1();
				DataSet1TableAdapters.OPHTableAdapter ophta = new DataSet1TableAdapters.OPHTableAdapter();
				int nor = opha.Fill(dts.OPH, "57036", "2016-01-01");

				OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage();
				var oms = new MemoryStream();
				try
				{
					OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Demo");
					ws.Cells["A1"].LoadFromDataTable(dts.OPH, true);

					Response r = new Response();
					r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					r.Headers["Content-Disposition"] = "attachment; filename=sener6.xlsx";

					pck.SaveAs(oms);
					oms.Seek(0, SeekOrigin.Begin);

					r.StreamedBody = oms;
					return r;

				}
				finally
				{
					if(pck != null)
						pck.Dispose();
					//if(oms != null)
					//	oms.Dispose();
				}

			});

			Handle.GET("/tMax14web/deneme", () => {
				return "deneme";
			});
		}

		enum hFlds 
		{
			None,
			OphID,
			EXD,
			ROT,
			MOT,
			Org,
			Dst,
			nStu,
			nStuTS,
			pStu,
			pStuTS,
			Shp,
			Cne,
			Acc,
			DTM,
			PTM,
			NOP,
			GrW,
			EOH,
			REOH,
			AOH,
			RTR,
			ROS,
			POD,
			ETD,
			ATD,
			ETA,
			ATA,
			CntNoS
		};
	}
}