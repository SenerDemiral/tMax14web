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
					
					<!--<link rel=""import"" href=""/sys/bootstrap.html"">
					<script src=""/sys/webcomponentsjs/webcomponents.min.js""></script>
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

			MainHandlers mh = new MainHandlers();
			mh.CreateIndex();
			mh.Register();



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

		}
	}
}