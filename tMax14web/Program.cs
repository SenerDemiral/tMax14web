﻿using System.IO;
using System;
using System.Linq;
using Starcounter;


namespace tMax14web
{
	class Program
	{
		static void Main()
		{

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

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

            Handle.GET("/tMax14web/service/delFMH", () => {
                Db.Transact(() =>
                {
                    Db.SQL("DELETE FROM TMDB.OPH");
                    Db.SQL("DELETE FROM TMDB.OPM");
                    Db.SQL("DELETE FROM TMDB.FRT");
                });

                return "OK";
            });

            Handle.GET("/tMax14web/ECBU5018958", () => {
                TMDB.Hlpr.ECBU5018958();
                return "OK";
            });

            Handle.GET("/tMax14web/ECS", () => {
                Db.Transact(() =>
                {
                    var fECS = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", 29651).FirstOrDefault();
                    fECS.Pwd = "ECS2020";
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
			//mh.CreateIndex();
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


            Handle.GET("/tMax14web/partial/EcsNbcPage", () => new EcsNbcPage());
            Handle.GET("/tMax14web/EcsNbcPage", () => WrapPage<EcsNbcPage>("/tMax14web/partial/EcsNbcPage"));

            Handle.GET("/tMax14web/partial/EcsNbiPage", () => new EcsNbiPage());
            Handle.GET("/tMax14web/EcsNbiPage", () => WrapPage<EcsNbiPage>("/tMax14web/partial/EcsNbiPage"));

            Handle.GET("/tMax14web/partial/EcsSbcPage", () => new EcsSbcPage());
            Handle.GET("/tMax14web/EcsSbcPage", () => WrapPage<EcsSbcPage>("/tMax14web/partial/EcsSbcPage"));

            Handle.GET("/tMax14web/partial/EcsSbiPage", () => new EcsSbiPage());
            Handle.GET("/tMax14web/EcsSbiPage", () => WrapPage<EcsSbiPage>("/tMax14web/partial/EcsSbiPage"));


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
                        ws.Cells[cr, (int)hFlds.Org].Value = h.ORG?.Ad;
                        ws.Cells[cr, (int)hFlds.Dst].Value = h.DST?.Ad;
                        ws.Cells[cr, (int)hFlds.nStu].Value = h.nStu;
                        ws.Cells[cr, (int)hFlds.nStuTS].Value = h.nStuTS;
                        //ws.Cells[cr, (int)hFlds.pStu].Value = h.pStu;
                        //ws.Cells[cr, (int)hFlds.pStuTS].Value = h.pStuTS;
                        ws.Cells[cr, (int)hFlds.Shp].Value = h.SHP?.Ad;
                        ws.Cells[cr, (int)hFlds.Cne].Value = h.CNE?.Ad;
                        ws.Cells[cr, (int)hFlds.Acc].Value = h.ACC?.Ad;
                        ws.Cells[cr, (int)hFlds.Mnf].Value = h.MNF?.Ad;
                        ws.Cells[cr, (int)hFlds.Nfy].Value = h.NFY?.Ad;
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

            Handle.GET("/tMax14web/EcsNBC/{?}/{?}", (string frtID, string sDate) =>
            {
                using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
                {
                    //Create the worksheet

                    OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("NBC");
                    int FRTID = Convert.ToInt32(frtID);

                    // Header (first row)
                    ws.Cells[1, 1].Value = "ECS Ref#";
                    ws.Cells[1, 2].Value = "TO Ref#";
                    ws.Cells[1, 3].Value = "Client";
                    ws.Cells[1, 4].Value = "Container#";
                    ws.Cells[1, 5].Value = "Loading Place";
                    ws.Cells[1, 6].Value = "UnLoading Place";
                    ws.Cells[1, 7].Value = "Requested Loading Date";
                    ws.Cells[1, 8].Value = "Booked Loading Slot";
                    ws.Cells[1, 9].Value = "Actual On Hand";
                    ws.Cells[1, 10].Value = "Seal#";
                    ws.Cells[1, 11].Value = "Number of Packages";
                    ws.Cells[1, 12].Value = "Gross Weight";
                    ws.Cells[1, 13].Value = "Reason for Change/Delay";
                    ws.Cells[1, 14].Value = "Customs Location";
                    ws.Cells[1, 15].Value = "Customs In";
                    ws.Cells[1, 16].Value = "Customs Out";
                    ws.Cells[1, 17].Value = "Port of Loading";
                    ws.Cells[1, 18].Value = "Port of Arrival";
                    ws.Cells[1, 19].Value = "Terminal Departure";
                    ws.Cells[1, 20].Value = "Extra Cost Description";

                    ws.Column(7).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(8).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(9).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(15).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(16).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(19).Style.Numberformat.Format = "dd.mm.yy";

                    ws.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Row(1).Style.WrapText = true;

                    ws.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(15).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(16).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(19).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    int cr = 2;
                    foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where h.POD >= ? and ROT = ? and MOT = ?", Convert.ToDateTime(sDate), "E", "R"))
                    {
                        ws.Cells[cr, 1].Value = h.OPM?.RefNo;
                        ws.Cells[cr, 2].Value = h.OpmID;
                        ws.Cells[cr, 3].Value = h.AccAd;
                        ws.Cells[cr, 4].Value = h.CntNoS;
                        ws.Cells[cr, 5].Value = h.ORG?.Ad;
                        ws.Cells[cr, 6].Value = h.DST?.Ad;
                        ws.Cells[cr, 7].Value = h.EOH;
                        ws.Cells[cr, 8].Value = h.EOH;
                        ws.Cells[cr, 9].Value = h.AOH;
                        ws.Cells[cr, 10].Value = h.mSealNoS;
                        ws.Cells[cr, 11].Value = h.NOP;
                        ws.Cells[cr, 12].Value = h.GrW;
                        ws.Cells[cr, 13].Value = h.OPM?.Inf;
                        ws.Cells[cr, 14].Value = h.CUSLOC?.Ad;
                        ws.Cells[cr, 15].Value = h.AOC;
                        ws.Cells[cr, 16].Value = h.RTD;
                        ws.Cells[cr, 17].Value = h.OPM?.POL?.Ad;
                        ws.Cells[cr, 18].Value = h.OPM?.POU?.Ad;
                        ws.Cells[cr, 19].Value = h.OPM?.ATD;
                        ws.Cells[cr, 20].Value = h.OthInf;

                        cr++;
                    }


                    ws.Row(1).Style.Font.Bold = true;
                    var range = ws.Cells["A1:T1"];
                    range.AutoFilter = true;
                    ws.View.FreezePanes(2, 2);  // 1.Row ve 1.Col fixed

                    var aa = ws.Dimension;
                    var bb = ws.Dimension.Address;

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    Response r = new Response();
                    //r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    r.ContentType = "application/octet-stream";
                    //r.Headers["Content-Disposition"] = "attachment; filename=\"tMax14web-ophs.xlsx\"";

                    var oms = new MemoryStream();
                    pck.SaveAs(oms);
                    oms.Seek(0, SeekOrigin.Begin);

                    r.StreamedBody = oms;
                    return r;
                }
            });

            Handle.GET("/tMax14web/EcsNBI/{?}/{?}", (string frtID, string sDate) =>
            {
                using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
                {
                    //Create the worksheet

                    OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("NBI");
                    int FRTID = Convert.ToInt32(frtID);

                    // Header (first row)
                    ws.Cells[1, 1].Value = "ECS Ref#";
                    ws.Cells[1, 2].Value = "TO Ref#";
                    ws.Cells[1, 3].Value = "Container#";
                    ws.Cells[1, 4].Value = "Loading Place";
                    ws.Cells[1, 5].Value = "Requested Loading Date";
                    ws.Cells[1, 6].Value = "Booked Loading Slot";
                    ws.Cells[1, 7].Value = "Effective Loading Date From";
                    ws.Cells[1, 8].Value = "Effective Loading Date To";
                    ws.Cells[1, 9].Value = "Seal#";
                    ws.Cells[1, 10].Value = "Number of Packages";
                    ws.Cells[1, 11].Value = "Gross Weight";
                    ws.Cells[1, 12].Value = "Reason for Change/Delay";
                    ws.Cells[1, 13].Value = "Customs Location";
                    ws.Cells[1, 14].Value = "Customs In";
                    ws.Cells[1, 15].Value = "Customs Out";
                    ws.Cells[1, 16].Value = "Terminal Departure";
                    ws.Cells[1, 17].Value = "Extra Cost Description";

                    ws.Column(5).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(6).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(7).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(8).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(14).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(15).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(16).Style.Numberformat.Format = "dd.mm.yy";

                    //ws.Column(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    int cr = 2;
                    foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where h.POD >= ? and ROT = ? and MOT = ?", Convert.ToDateTime(sDate), "E", "R"))
                    {
                        ws.Cells[cr, 1].Value = h.OPM?.RefNo;
                        ws.Cells[cr, 2].Value = h.OpmID;
                        ws.Cells[cr, 3].Value = h.CntNoS;
                        ws.Cells[cr, 4].Value = h.ORG?.Ad;
                        ws.Cells[cr, 5].Value = h.EOH;
                        ws.Cells[cr, 6].Value = h.EOH;
                        ws.Cells[cr, 7].Value = h.EOH;
                        ws.Cells[cr, 8].Value = h.AOH;
                        ws.Cells[cr, 9].Value = h.mSealNoS;
                        ws.Cells[cr, 10].Value = h.NOP;
                        ws.Cells[cr, 11].Value = h.GrW;
                        ws.Cells[cr, 12].Value = h.OPM?.Inf;
                        ws.Cells[cr, 13].Value = h.CUSLOC?.Ad;
                        ws.Cells[cr, 14].Value = h.AOC;
                        ws.Cells[cr, 15].Value = h.RTD;
                        ws.Cells[cr, 16].Value = h.OPM?.ATD;
                        ws.Cells[cr, 27].Value = h.OthInf;

                        cr++;
                    }


                    ws.Row(1).Style.Font.Bold = true;
                    var range = ws.Cells["A1:P1"];
                    range.AutoFilter = true;
                    ws.View.FreezePanes(2, 2);  // 1.Row ve 1.Col fixed

                    var aa = ws.Dimension;
                    var bb = ws.Dimension.Address;

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    Response r = new Response();
                    //r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    r.ContentType = "application/octet-stream";
                    //r.Headers["Content-Disposition"] = "attachment; filename=\"tMax14web-ophs.xlsx\"";

                    var oms = new MemoryStream();
                    pck.SaveAs(oms);
                    oms.Seek(0, SeekOrigin.Begin);

                    r.StreamedBody = oms;
                    return r;
                }
            });

            Handle.GET("/tMax14web/EcsSBC/{?}/{?}", (string frtID, string sDate) =>
            {
                using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
                {
                    //Create the worksheet

                    OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SBC");
                    int FRTID = Convert.ToInt32(frtID);

                    // Header (first row)
                    ws.Cells[1, 1].Value = "ECS Ref#";
                    ws.Cells[1, 2].Value = "TO Ref#";
                    ws.Cells[1, 3].Value = "Client";
                    ws.Cells[1, 4].Value = "Container#";
                    ws.Cells[1, 5].Value = "Loading Place";
                    ws.Cells[1, 6].Value = "Final Location";
                    ws.Cells[1, 7].Value = "Final ETA";
                    ws.Cells[1, 8].Value = "Effective Arrival";
                    ws.Cells[1, 9].Value = "Customs Location";
                    ws.Cells[1, 10].Value = "Customs In Terminal";
                    ws.Cells[1, 11].Value = "Customs Out";
                    ws.Cells[1, 12].Value = "Delivery Location";
                    ws.Cells[1, 13].Value = "Requested Delivery";
                    ws.Cells[1, 14].Value = "Booked Delivery Slot";
                    ws.Cells[1, 15].Value = "Effective Delivery From";
                    ws.Cells[1, 16].Value = "Effective Delivery To";
                    ws.Cells[1, 17].Value = "Reason for Change/Delay";
                    ws.Cells[1, 18].Value = "Extra Cost Description";

                    ws.Column(7).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(8).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(10).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(11).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(13).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(14).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(16).Style.Numberformat.Format = "dd.mm.yy";

                    //ws.Column(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    int cr = 2;
                    foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where h.POD >= ? and ROT = ? and MOT = ?", Convert.ToDateTime(sDate), "I", "R"))
                    {
                        ws.Cells[cr, 1].Value = h.OPM?.RefNo;
                        ws.Cells[cr, 2].Value = h.OpmID;
                        ws.Cells[cr, 3].Value = h.AccAd;
                        ws.Cells[cr, 4].Value = h.CntNoS;
                        ws.Cells[cr, 5].Value = h.ORG?.Ad;
                        ws.Cells[cr, 6].Value = h.OPM?.POU?.Ad;
                        ws.Cells[cr, 7].Value = h.OPM?.ETA;
                        ws.Cells[cr, 8].Value = h.OPM?.ATA;
                        ws.Cells[cr, 9].Value = h.CUSLOC?.Ad;
                        ws.Cells[cr, 10].Value = h.DRBD;
                        ws.Cells[cr, 11].Value = h.DDT;
                        ws.Cells[cr, 12].Value = h.DST?.Ad;
                        ws.Cells[cr, 13].Value = h.ROS;
                        ws.Cells[cr, 14].Value = h.ROH;
                        ws.Cells[cr, 15].Value = h.PODinf;
                        ws.Cells[cr, 16].Value = h.POD;
                        ws.Cells[cr, 17].Value = h.OPM?.Inf;
                        ws.Cells[cr, 18].Value = h.OthInf;

                        cr++;
                    }


                    ws.Row(1).Style.Font.Bold = true;
                    var range = ws.Cells["A1:R1"];
                    range.AutoFilter = true;
                    ws.View.FreezePanes(2, 2);  // 1.Row ve 1.Col fixed

                    var aa = ws.Dimension;
                    var bb = ws.Dimension.Address;

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    Response r = new Response();
                    //r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    r.ContentType = "application/octet-stream";
                    //r.Headers["Content-Disposition"] = "attachment; filename=\"tMax14web-ophs.xlsx\"";

                    var oms = new MemoryStream();
                    pck.SaveAs(oms);
                    oms.Seek(0, SeekOrigin.Begin);

                    r.StreamedBody = oms;
                    return r;
                }
            });

            Handle.GET("/tMax14web/EcsSBI/{?}/{?}", (string frtID, string sDate) =>
            {
                using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
                {
                    //Create the worksheet

                    OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SBI");
                    int FRTID = Convert.ToInt32(frtID);

                    // Header (first row)
                    ws.Cells[1, 1].Value = "ECS Ref#";
                    ws.Cells[1, 2].Value = "TO Ref#";
                    ws.Cells[1, 3].Value = "Client";
                    ws.Cells[1, 4].Value = "Container#";
                    ws.Cells[1, 5].Value = "Customs Location";
                    ws.Cells[1, 6].Value = "Arrival Date";
                    ws.Cells[1, 7].Value = "Customs Clearance Finished";
                    ws.Cells[1, 8].Value = "Delivery Location";
                    ws.Cells[1, 9].Value = "Effective Delivery From";
                    ws.Cells[1, 10].Value = "Effective Delivery To";
                    ws.Cells[1, 11].Value = "Extra Cost Description";

                    ws.Column(7).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(8).Style.Numberformat.Format = "dd.mm.yy";
                    ws.Column(10).Style.Numberformat.Format = "dd.mm.yy";

                    ws.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Column(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    int cr = 2;
                    foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where h.POD >= ? and ROT = ? and MOT = ?", Convert.ToDateTime(sDate), "I", "R"))
                    {
                        ws.Cells[cr, 1].Value = h.OPM?.RefNo;
                        ws.Cells[cr, 2].Value = h.OpmID;
                        ws.Cells[cr, 3].Value = h.AccAd;
                        ws.Cells[cr, 4].Value = h.CntNoS;
                        ws.Cells[cr, 5].Value = h.CUSLOC?.Ad;
                        ws.Cells[cr, 6].Value = h.OPM?.RTD;
                        ws.Cells[cr, 7].Value = h.RTR;
                        ws.Cells[cr, 8].Value = h.DST?.Ad;
                        ws.Cells[cr, 9].Value = h.PODinf;
                        ws.Cells[cr, 10].Value = h.POD;
                        ws.Cells[cr, 11].Value = h.OthInf;

                        cr++;
                    }


                    ws.Row(1).Style.Font.Bold = true;
                    var range = ws.Cells["A1:K1"];
                    range.AutoFilter = true;
                    ws.View.FreezePanes(2, 2);  // 1.Row ve 1.Col fixed

                    var aa = ws.Dimension;
                    var bb = ws.Dimension.Address;

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    Response r = new Response();
                    //r.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    r.ContentType = "application/octet-stream";
                    //r.Headers["Content-Disposition"] = "attachment; filename=\"tMax14web-ophs.xlsx\"";

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