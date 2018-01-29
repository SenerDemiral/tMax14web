using System.IO;
using System;
using System.Linq;
using Starcounter;


namespace tMax14web
{
	class Program
	{
		static void Main()
		{
			var HTML = @"<!DOCTYPE html>
				<html>
				<head>
					<meta charset=""utf-8"">
				    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=yes"">
					<title>{0}</title>
					
                    <script src=""/sys/webcomponentsjs/webcomponents-lite.js""></script>

                    <script>
                        window.Polymer = {{
                            dom: ""shadow""
                        }};
                    </script>

					<link rel=""import"" href=""/sys/polymer/polymer.html"">
					<link rel=""import"" href=""/sys/starcounter.html"">
					<link rel=""import"" href=""/sys/starcounter-include/starcounter-include.html"">
					<!--link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html""-->
					

                    <script src=""/sys/thenBy.js""></script>
					<link rel=""stylesheet"" href=""/sys/normalize.css"">
					<link rel=""stylesheet"" href=""/sys/Stylesheet1.css"">
					<link rel=""stylesheet"" href=""/sys/Stylesheet2.css"">
				
				</head>

                <body>
                    <dom-bind id=""palindrom-root"">
                        <template is=""dom-bind"">
                            <starcounter-include view-model=""{{{{model}}}}""></starcounter-include>
                        </template>
                    </dom-bind>
                    <palindrom-client ref=""palindrom-root"" remote-url=""{1}""></palindrom-client>
                </body>

				</html>";

            /*
					<!--
                    <script src=""/sys/redips-drag-min.js""></script>
                    <script src=""/sys/redips-script.js""></script>
                    <link rel=""import"" href=""/sys/bootstrap.html"">
					<link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html"">
					<script src=""/sys/webcomponentsjs/webcomponents.min.js""></script>
					<link rel=""import"" href=""/sys/iron-icons/maps-icons.html"">
					<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css"" integrity=""sha384-rwoIResjU2yc3z8GV/NPeZWAv56rSmLldC3R/AZzGRnGxQQKnKkoFVhFQhNUwEyJ"" crossorigin=""anonymous"">
					
					<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css"" crossorigin=""anonymous"">
					<link rel=""stylesheet"" href=""/sys/Stylesheet1.css"">
					<link rel=""import"" href=""/sys/github-browser-app.html"">
					-->

                <body>
                    <template is=""dom-bind"" id=""puppet-root"">
                        <starcounter-include view-model=""{{{{model}}}}""></starcounter-include>
                    </template>
                    <puppet-client ref=""puppet-root"" remote-url=""{1}""></puppet-client>
                    <starcounter-debug-aid></starcounter-debug-aid>
                </body>
                <body>
                    <template is=""dom-bind"" id=""puppet-root"">
                        <template is=""imported-template"" content$=""{{{{model.Html}}}}"" model=""{{{{model}}}}""></template>
                    </template>
                    <palindrom-client ref=""puppet-root"" remote-url=""{1}""></palindrom-client>
                    <starcounter-debug-aid></starcounter-debug-aid>
                </body>
                <body>
					<template is=""dom-bind"" id=""puppet-root"">
            		<starcounter-include view-model=""{{{{model}}}}""></starcounter-include>
                    </template>
					<palindrom-client ref=""puppet-root"" remote-url=""{1}""></palindrom-client>
					<starcounter-debug-aid></starcounter-debug-aid>
				</body>

                    < puppet-client ref=""puppet-root"" remote-url=""{1}"" use-web-socket=""true"" ></ puppet-client >
            		<template is=""imported-template"" content$=""{{{{model.Html}}}}"" model=""{{{{model}}}}""></template>
            		<starcounter-include view-model=""{{{{model}}}}"" partial-id$=""{{{{model.Html}}}}""></starcounter-include>
					<starcounter-debug-aid></starcounter-debug-aid>
					<script src=""https://code.jquery.com/jquery-3.1.1.slim.min.js"" integrity=""sha384-A7FZj7v+d/sdmMqp/nOQwliLvUsJfDHW+k9Omg/a/EheAdgtzNs3hpfag6Ed950n"" crossorigin=""anonymous""></script>
					<script src=""https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js"" integrity=""sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb"" crossorigin=""anonymous""></script>
					<script src=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js"" integrity=""sha384-vBWWzlZJ8ea9aCX4pEW3rVHjgjt7zpkNpZk+02D9phzyeVkE+jo0ieGizqPLForn"" crossorigin=""anonymous""></script>
            */

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider(HTML));

            Handle.GET("/tMax14web/init", () => {
                Db.Transact(() =>
                {
                    var f57036 = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", 57036).FirstOrDefault();
                    f57036.Pwd = "57036";
                    /*
                    foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h"))
                    {
                        h.VM3 = 1;
                        h.ChW = 2;
                    }*/
                });
                return "OK";
            });
            /*
            Handle.GET("/tMax14web/Mapxxxx", () =>
            {
                //var page = new MapPage();
                //page.Data = null;
                //return page; // new MapPage();
                
            var master = (MasterPage)Self.GET("/tMax14web");
                master.CurrentPage = new MapPage();
                master.CurrentPage.Data = null;

                return master;

            });
        */
            Handle.GET("/tMax14web/d2", () =>
            {
                var master = (MasterPage)Self.GET("/tMax14web");
                master.CurrentPage = new D2();
                master.CurrentPage.Data = null;

                return master;
            });

            MainHandlers mh = new MainHandlers();
			mh.CreateIndex();
            //mh.Register();

            Handle.GET("/tMax14web", () => { return Self.GET("/tMax14web/MainPage"); });

            Handle.GET("/tMax14web/partial/MainPage", () => new MainPage());
            Handle.GET("/tMax14web/MainPage", () => WrapPage<MainPage>("/tMax14web/partial/MainPage"));

            //Handle.GET("/tMax14web/partial/MasterPage", () => new MasterPage());
            //Handle.GET("/tMax14web/MasterPage", () => WrapPage<MasterPage>("/tMax14web/partial/MasterPage"));


            //Handle.GET("/tMax14web", () => { return Self.GET("/tMax14web/MasterPage"); });
            Handle.GET("/tMax14web/partial/OphClientTable", () => new OphClientPageTable());
            Handle.GET("/tMax14web/OphClientTable", () => WrapPage<OphClientPageTable>("/tMax14web/partial/OphClientTable"));

            Handle.GET("/tMax14web/partial/OphClient", () => new OphClientPage());
            Handle.GET("/tMax14web/OphClient", () => WrapPage<OphClientPage>("/tMax14web/partial/OphClient"));

            Handle.GET("/tMax14web/partial/gMap", () => new MapPage());
            Handle.GET("/tMax14web/gMap", () => WrapPage<MapPage>("/tMax14web/partial/gMap"));

            Handle.GET("/tMax14web/partial/deneme", () => new Deneme());
            Handle.GET("/tMax14web/deneme", () => WrapPage<Deneme>("/tMax14web/partial/deneme"));

            // Asagidaki gibi calismiyor (/tMax14web/Map), /tMax14web/MapX yapinca oluyor
            //Handle.GET("/tMax14web/partial/Map", () => new MapPage());
            //Handle.GET("/tMax14web/Map", () => WrapPage<MapPage>("/tMax14web/partial/Map"));

            Handle.GET("/tMax14web/ophs2xlsx/{?}/{?}", (string frtID, string sDate) =>
            {
                using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
                {
                    //Create the worksheet

                    OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("House");
                    int FRTID = Convert.ToInt32(frtID);

                    var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where (h.ShpID = ? or h.CneID = ? or h.AccID = ?) and h.EXD >= ?", FRTID, FRTID, FRTID, Convert.ToDateTime(sDate));
                    int cr = 2;
                    foreach (var h in ophs)
                    {

                        ws.Cells[cr, (int)hFlds.OphID].Value = h.OphID;
                        ws.Cells[cr, (int)hFlds.RefNo].Value = h.RefNo;
                        ws.Cells[cr, (int)hFlds.mVhc].Value = h.mVhc;
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
                        ws.Cells[cr, (int)hFlds.mETD].Value = h.mETD;
                        ws.Cells[cr, (int)hFlds.mATD].Value = h.mATD;
                        ws.Cells[cr, (int)hFlds.mETA].Value = h.mETA;
                        ws.Cells[cr, (int)hFlds.mATA].Value = h.mATA;
                        ws.Cells[cr, (int)hFlds.mRETD].Value = h.mRETD;
                        ws.Cells[cr, (int)hFlds.mRETA].Value = h.mRETA;
                        ws.Cells[cr, (int)hFlds.mACOT].Value = h.mACOT;
                        ws.Cells[cr, (int)hFlds.mTPAD].Value = h.mTPAD;
                        ws.Cells[cr, (int)hFlds.mTPDD].Value = h.mTPDD;
                        ws.Cells[cr, (int)hFlds.CntNoS].Value = h.CntNoS;

                        cr++;
                    }
                    // Header (first row)
                    ws.Cells[1, (int)hFlds.OphID].Value = "ID";
                    ws.Cells[1, (int)hFlds.RefNo].Value = "RefNo";
                    ws.Cells[1, (int)hFlds.mVhc].Value = "mVhc";
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
                    ws.Cells[1, (int)hFlds.mETD].Value = "mETD";
                    ws.Cells[1, (int)hFlds.mATD].Value = "mATD";
                    ws.Cells[1, (int)hFlds.mETA].Value = "mETA";
                    ws.Cells[1, (int)hFlds.mATA].Value = "mATA";
                    ws.Cells[1, (int)hFlds.mACOT].Value = "mACOT";
                    ws.Cells[1, (int)hFlds.mTPAD].Value = "mTPAD";
                    ws.Cells[1, (int)hFlds.mTPDD].Value = "mTPDD";
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
                        (int)hFlds.mETD,
                        (int)hFlds.mATD,
                        (int)hFlds.mETA,
                        (int)hFlds.mATA,
                        (int)hFlds.mRETD,
                        (int)hFlds.mRETA,
                        (int)hFlds.mACOT,
                        (int)hFlds.mTPAD,
                        (int)hFlds.mTPDD,
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

            Hook<TMDB.TH>.CommitUpdate += (s, obj) =>
            {
                var o = obj;
                TMDB.Hlpr.WriteLoginLog(obj.ID);

            };

        }
        enum hFlds : int
        {
            None, OphID, RefNo, mVhc, EXD, ROT, MOT, Org, Dst, nStu, nStuTS, pStu, pStuTS, Shp, Cne, Acc, Mnf, Nfy, DTM, PTM, NOP, GrW, VM3, ChW, ROH, EOH, REOH, AOH, RTR, ROS, POD, mETD, mATD, mETA, mATA, mRETD, mRETA, mACOT, mTPAD, mTPDD, CntNoS
        };

        public static MasterPage GetMasterPageFromSession()
        {
            /*
            if (Session.Current == null)
            {
                Session.Current = new Session(Session.Flags.PatchVersioning);
            }
            */
            //Session.Ensure();

            ////MasterPage master = Session.Current.Data as MasterPage;
            //MasterPage master = Session.Current.Store["App"] as MasterPage;

            var master = Session.Ensure().Store["App"] as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                //Session.Current.Data = master;
                Session.Current.Store["App"] = master;
            }

            return master;
        }

        private static Json WrapPage<T>(string partialPath) where T : Json
        {
            var master = GetMasterPageFromSession();

            /*
            if (master.CurrentPage != null && master.CurrentPage.GetType().Equals(typeof(T)))
            {
                return master;
            }
            */
            master.CurrentPage = Self.GET(partialPath);

            if (master.CurrentPage.Data == null)
            {
                master.CurrentPage.Data = null; //trick to invoke OnData in partial
            }

            return master;
        }

    }
}