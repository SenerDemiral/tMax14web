using Starcounter;

namespace tMax14web
{
	partial class OphClientPage : Json
	{
		[OphClientPage_json]
		protected override void OnData()
		{
			base.OnData();

			var parent = (MasterPage)this.Parent;
			var fid = parent.fID;
			var std = parent.StartDate;


			DataSet1 ds = new DataSet1();
			DataSet1TableAdapters.OPHTableAdapter opha = new DataSet1TableAdapters.OPHTableAdapter();
			int nor = opha.Fill(ds.OPH, fid, std);
			Ophs.Clear();

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
			}

		}
	}
}
