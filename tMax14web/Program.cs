using System;
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
					
					<!--<link rel=""import"" href=""/sys/bootstrap.html"">
					<link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html"">
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
		}
	}
}