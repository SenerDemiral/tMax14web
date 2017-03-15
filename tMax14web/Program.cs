using System.IO;
using System.Text;
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
					<script src=""/sys/thenBy.js""></script>
					<link rel=""import"" href=""/sys/polymer/polymer.html"">
					<link rel=""import"" href=""/sys/starcounter.html"">
					<link rel=""import"" href=""/sys/starcounter-include/starcounter-include.html"">
					
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

					<!--
					<starcounter-debug-aid></starcounter-debug-aid>
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

			Handle.GET("/tMax14web/ophs2xlsx/{?}/{?}", (string frtID, string sDate) => {

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
		}
	}
}