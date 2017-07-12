
using System;
using Starcounter;

namespace TMDB
{

	[Database]
	public class WebSocketId
	{
		public UInt64 Id;
		public UInt32 NumBroadcasted;
		public DateTime ToU;	// Time of Update
	}

	[Database]
	public class WsMsg
	{
		public string Msg;
		public DateTime ToU;    // Time of Update
	}

    [Database]
    public class OSN    // OpsStuNormal
    {
        public string Stu { get; set; }
        public string Ad { get; set; }
    }

    [Database]
    public class OSP    // OpsStuProblem
    {
        public string Stu { get; set; }
        public string Ad { get; set; }
    }

    [Database]
	public class FRT
	{
		public DateTime MdfdOn { get; set; }
		public int FrtID { get; set; }
		public string AdN { get; set; }
        public string Ad { get; set; }
        public string LocID { get; set; }
		public string Pwd { get; set; }

		public FRT()
		{
			FrtID = 0;
			AdN = "";
			Ad = "";
            LocID = "";
			Pwd = "";
		}
	}

    [Database]
    public class AFB
    {
		public DateTime MdfdOn { get; set; }
        public int AfbID { get; set; }
        public int? FrtID { get; set; }
        public FRT Frt { get; set; }
        public string Tur { get; set; }

        public string FtrNo { get; set; }
        public DateTime? FtrTrh { get; set; }
        public DateTime? OdmVde { get; set; }
        public string bDvz { get; set; }
        public double? bTutBrt { get; set; }

        public int? DknFrtID { get; set; }
        public FRT DknFrt { get; set; }
        public string DknNo { get; set; }
        public string DknDvz { get; set; }

        public string FrtAd => Frt == null ? "" : Frt.AdN;
        public string DknFrtAd => DknFrt == null ? "" : DknFrt.AdN;
    }

    [Database]
    public class OPM
	{
		public DateTime MdfdOn { get; set; }
        public int OpmID { get; set; }
        public string RefNo { get; set; }
        public DateTime? EXD { get; set; }
        public string ROT { get; set; }
		public string MOT { get; set; }
		public string Org { get; set; }
		public string Dst { get; set; }

		public int? ShpID { get; set; }
		public int? CneID { get; set; }
		public int? AccID { get; set; }
        public int? CrrID { get; set; }

        public FRT Shp { get; set; }
        public FRT Cne { get; set; }
        public FRT Acc { get; set; }
        public FRT Crr { get; set; }

        public string nStu { get; set; }
        public string pStu { get; set; }
        public DateTime? ETD { get; set; }
		public DateTime? ATD { get; set; }
		public DateTime? ETA { get; set; }
		public DateTime? ATA { get; set; }
        public DateTime? ACOT { get; set; }
        public string Vhc { get; set; }
        public string CntNoS { get; set; }

        public string ShpAd => Shp == null ? "" : Shp.AdN;
		public string CneAd => Cne == null ? "" : Cne.AdN;
		public string AccAd => Acc == null ? "" : Acc.AdN;
        public string CrrAd => Crr == null ? "" : Crr.AdN;

        public string nStuAd {
            get
            {
                if (nStu == null)
                    return ""; 
                var osn = Db.SQL<OSN>("select f from TMDB.OSN f where f.Stu = ?", this.nStu).First;
                if (osn == null)
                    return "";
                return osn.Ad;
            }
        }
    }


    [Database]
	public class OPH
	{
		public DateTime MdfdOn { get; set; }
        public int OphID { get; set; }
        public int? OpmID { get; set; }
        public string RefNo { get; set; }
        public DateTime? EXD { get; set; }

        public string ROT { get; set; }
		public string MOT { get; set; }
		public string Org { get; set; }
		public string Dst { get; set; }

        public int? ShpID { get; set; }
		public int? CneID { get; set; }
		public int? AccID { get; set; }
		public int? MnfID { get; set; }
        public int? NfyID { get; set; }
        public int? CrrID { get; set; }

        public OPM Opm { get; set; }
        public FRT Shp { get; set; }
        public FRT Cne { get; set; }
        public FRT Acc { get; set; }
        public FRT Mnf { get; set; }
        public FRT Nfy { get; set; }
        public FRT Crr { get; set; }

        public string nStu { get; set; }
		public string pStu { get; set; }
        public string DTM { get; set; }
        public string PTM { get; set; }
        public string CntNoS { get; set; }

        public int? NOP { get; set; }
        public double? GrW { get; set; }
        public double? VM3 { get; set; }
        public int? ChW { get; set; }

        public DateTime? nStuTS { get; set; }
        public DateTime? pStuTS { get; set; }
        public DateTime? ROH { get; set; }
        public DateTime? EOH { get; set; }
        public DateTime? REOH { get; set; }
        public DateTime? AOH { get; set; }
		public DateTime? RTR { get; set; }
		public DateTime? ROS { get; set; }
		public DateTime? POD { get; set; }
        public DateTime? DRBD { get; set; }
        public DateTime? CABW { get; set; }

        public string EXD_t => $"{EXD:s}";
		public string nStuTS_t => $"{nStuTS:s}";
		public string pStuTS_t => $"{pStuTS:s}";
        public string ROH_t => $"{ROH:s}";
        public string EOH_t => $"{EOH:s}";
		public string REOH_t => $"{REOH:s}";
		public string AOH_t => $"{AOH:s}";
		public string RTR_t => $"{RTR:s}";
		public string ROS_t => $"{ROS:s}";
        public string POD_t => $"{POD:s}";
        public string DRBD_t => $"{DRBD:s}";

        public string ShpAd => Shp == null ? "" : Shp.AdN;
		public string CneAd => Cne == null ? "" : Cne.AdN;
		public string AccAd => Acc == null ? "" : Acc.AdN;
        public string MnfAd => Mnf == null ? "" : Mnf.AdN;
        public string NfyAd => Nfy == null ? "" : Nfy.AdN;
        public string CrrAd => Crr == null ? "" : Crr.AdN;

        public DateTime? ETD => Opm == null ? null : Opm.ETD;
		public DateTime? ATD => Opm == null ? null : Opm.ATD;
		public DateTime? ETA => Opm == null ? null : Opm.ETA;
		public DateTime? ATA => Opm == null ? null : Opm.ATA;
        public DateTime? ACOT => Opm == null ? null : Opm.ACOT;

        public string ETD_t => Opm == null ? "" : $"{Opm.ETD:s}";
		public string ATD_t => Opm == null ? "" : $"{Opm.ATD:s}";
		public string ETA_t => Opm == null ? "" : $"{Opm.ETA:s}";
        public string ATA_t => Opm == null ? "" : $"{Opm.ATA:s}";
        public string ACOT_t => Opm == null ? "" : $"{Opm.ACOT:s}";

        public string nStuAd
        {
            get
            {
                if (nStu == null)
                    return "";
                var osn = Db.SQL<OSN>("select f from TMDB.OSN f where f.Stu = ?", this.nStu).First;
                if (osn == null)
                    return "";
                return osn.Ad;
            }
        }

        public OPH()
		{
			OphID = 0;
			EXD = null;

		}
	}
}