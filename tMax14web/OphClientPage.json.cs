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
			opha.Fill(ds.OPH, fid, std);

			foreach(DataSet1.OPHRow row in ds.OPH.Rows) 
			{
				OphsElementJson he = new OphsElementJson()
				{
					hID = row.HID,
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
					shND = row.SHND,
					shPD = row.SHPD,
					shTD = row.SHTD,

				};

				Ophs.Add(he);
			}

		}
	}
}
