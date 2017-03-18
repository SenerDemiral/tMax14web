
using System;
using Starcounter;

namespace TMDB
{

	[Database]
	public class FRT
	{
		public int FrtID;
		public string AdN;
		public string LocID;

		public FRT()
		{
			FrtID = 0;
			AdN = "";
			LocID = "";
		}
	}

	[Database]
	public class OPM
	{
		public int OpmID;
		public string RefNo;
		public DateTime? EXD;
		public string ROT;
		public string MOT;
		public string Org;
		public string Dst;
		public int? ShpID;
		public int? CneID;
		public int? AccID;
		public int? CrrID;
		public string nStu;
		public string pStu;
		public DateTime? ETD;
		public DateTime? ATD;
		public DateTime? ETA;
		public DateTime? ATA;
		public string Vhc;
		public string CntNoS;
		public FRT Shp;
		public FRT Cne;
		public FRT Acc;
		public FRT Crr;
		public string ShpAd => Shp == null ? "" : Shp.AdN;
		public string CneAd => Cne == null ? "" : Cne.AdN;
		public string AccAd => Acc == null ? "" : Acc.AdN;
		public string CrrAd => Crr == null ? "" : Crr.AdN;

	}

	[Database]
	public class OPH
	{
		public int OphID;
		public int? OpmID;
		public string RefNo;
		public DateTime? EXD;
		public string ROT;
		public string MOT;
		public string Org;
		public string Dst;
		public int? ShpID;
		public int? CneID;
		public int? AccID;
		public string nStu;
		public string pStu;
		public string DTM;
		public string PTM;
		public int? NOP;
		public double? GrW;
		public DateTime? nStuTS;
		public DateTime? pStuTS;
		public DateTime? EOH;
		public DateTime? REOH;
		public DateTime? AOH;
		public DateTime? RTR;
		public DateTime? ROS;
		public DateTime? POD;
		public string CntNoS;

		public string EXD_t => $"{EXD:s}";
		public string nStuTS_t => $"{nStuTS:s}";
		public string pStuTS_t => $"{pStuTS:s}";
		public string EOH_t => $"{EOH:s}";
		public string REOH_t => $"{REOH:s}";
		public string AOH_t => $"{AOH:s}";
		public string RTR_t => $"{RTR:s}";
		public string ROS_t => $"{ROS:s}";
		public string POD_t => $"{POD:s}";

		public OPM Opm;
		public FRT Shp;
		public FRT Cne;
		public FRT Acc;
		public string ShpAd => Shp == null ? "" : Shp.AdN;
		public string CneAd => Cne == null ? "" : Cne.AdN;
		public string AccAd => Acc == null ? "" : Acc.AdN;

		public DateTime? ETD => Opm == null ? null : Opm.ETD;
		public DateTime? ATD => Opm == null ? null : Opm.ATD;
		public DateTime? ETA => Opm == null ? null : Opm.ETA;
		public DateTime? ATA => Opm == null ? null : Opm.ATA;

		public string ETD_t => Opm == null ? "" : $"{Opm.ETD:s}";
		public string ATD_t => Opm == null ? "" : $"{Opm.ATD:s}";
		public string ETA_t => Opm == null ? "" : $"{Opm.ETA:s}";
		public string ATA_t => Opm == null ? "" : $"{Opm.ATA:s}";

		public OPH()
		{
			OphID = 0;
			EXD = null;

		}
	}
}