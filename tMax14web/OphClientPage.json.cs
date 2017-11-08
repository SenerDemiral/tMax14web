using System;
using Starcounter;
using Starcounter.Advanced.XSON;

namespace tMax14web
{
	partial class OphClientPage : Json
	{
        void Handle(Input.DownloadExcelTrigger action)
        {
            var aa = "dfadfad";
        }

        [OphClientPage_json]
		protected override void OnData()
		{
			base.OnData();
            var parent = (MasterPage)this.Parent;
            var fid = Convert.ToInt32(parent.fID);
			var std = Convert.ToDateTime(parent.StartDate);
            fID = parent.fID;
            StartDate = parent.StartDate;
            /*
			var fid = Convert.ToInt32(parent.fID);
			var fpw = parent.fPW;



            Ophs.Clear();
            var Frt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", fid).First;
            if (Frt == null || string.IsNullOrEmpty(fpw) || Frt.Pwd != fpw)
            {
                parent.fAdN = "Not Found";
                parent.fOnLine = false;
                return;
            }
            parent.fOnLine = true;
            parent.fAdN = Frt.AdN;
            parent.fAd = Frt.Ad;
            */

            if (!parent.fOnLine)
                return;

            //if (fpw != "can")
            //	return;

            Columns.Clear();
            //ColumnsElementJson c = new ColumnsElementJson();
            Columns.Add("aaaa");

            Cols.Clear();
            ColsElementJson c = new ColsElementJson()
            {
                Fld = "FldA",
                Hdr = "IslemTarihi",
                Ftr = "Alan A tooltip",
                Fmt = "YYYY.MM.DD"
            };
            Cols.Add(c);
            
			Ophs.Data = Db.SQL<TMDB.OPH>("select h from OPH h where (h.ShpID = ? or h.CneID = ? or h.AccID = ?) and h.EXD >= ?", fid, fid, fid, std);

			/*
			foreach(var h in ophs) 
			{
				var a = h.nStuTS;
				var b = h.nStuTS_t;
				var c = h.REOH_t;
			}
			*/
			
			/*
			DataSet1 ds = new DataSet1();
			DataSet1TableAdapters.OPHTableAdapter opha = new DataSet1TableAdapters.OPHTableAdapter();
			int nor = opha.Fill(ds.OPH, fid, std);
			foreach(DataSet1.OPHRow row in ds.OPH.Rows) 
			{
				OphsElementJson he = new OphsElementJson()
				{
					hID = row.HID,
					EXD = row.EXD,

					ROT = row.ROT,
					MOT = row.MOT,
					
					Org = row.ORG,
					Dst = row.DST,
					nStu = row.NSTU,
					nStuD = row.NSTUD,
					
					Shp = row.SHP,
					Cne = row.CNE,
					Acc = row.ACC,
					NOP = row.NOP,
					GrW = row.GRW,

					shBD = row.SHBD,
					shED = row.SHED,
					shND = row.SHND,
					shPD = row.SHPD,
					shTD = row.SHTD,
					
					DTM  = row.DTM,
					PTM	 = row.PTM,
					EOH	 = row.EOH,
					REOH = row.REOH,
					AOH	 = row.AOH,
					RTR	 = row.RTR,
					ROS	 = row.ROS,
					POD	 = row.POD,
					ETD	 = row.ETD,
					ATD	 = row.ATD,
					ETA	 = row.ETA,
					ATA  = row.ATA,

					CntNoS = row.CNTNOS

				};

				Ophs.Add(he);
			} */

		}
	}
}
