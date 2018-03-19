
using System;
using System.Linq;
using Starcounter;

namespace TMDB
{
    [Database]
    public class TH
    {
        public string ID { get; set; }
        public string CntNo { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public DateTime LTS { get; set; }

        public TH()
        {
            ID = "864768011110084";
            CntNo = "ECBU5012836";
            Lat = "37.041987";
            Lng = "27.428861";
        }
    }

    [Database]
    public class LogStat
    {
        public int FrtID { get; set; }
        public string Pwd { get; set; }
        public DateTime ToI { get; set; }    // Time of Insert
        public bool OK { get; set; }

        public LogStat()
        {
            OK = false;
            ToI = DateTime.Now;
        }
    }

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
    public class LOC    // Locations
    {
		public DateTime MdfdOn { get; set; }
        public string LocID { get; set; }
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
    public class FRC
    {
        public DateTime MdfdOn { get; set; }
        public int FrcID { get; set; }
        public int FrtID { get; set; }
        public FRT Frt { get; set; }
        public string Ad { get; set; }
        public string eMail { get; set; }
        public string RptIDs { get; set; }

        public FRC()
        {
            FrcID = 0;
            FrtID = 0;
            Ad = "";
            eMail = "";
            RptIDs = "";
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
        public string FtrTrh_t => $"{FtrTrh:s}";
        public string OdmVde_t => $"{OdmVde:s}";

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

		//public string Org { get; set; }   // Dropped
        //public string Dst { get; set; }
        //public string POU { get; set; }

        public string OrgID { get; set; }
        public string DstID { get; set; }
        public string PolID { get; set; }
        public string PouID { get; set; }
        public LOC ORG { get; set; }
        public LOC DST { get; set; }
        public LOC POL { get; set; }
        public LOC POU { get; set; }

        public int? ShpID { get; set; }
		public int? CneID { get; set; }
		public int? AccID { get; set; }
        public int? CrrID { get; set; }

        public FRT SHP { get; set; }
        public FRT CNE { get; set; }
        public FRT ACC { get; set; }
        public FRT CRR { get; set; }

        public string nStu { get; set; }
        public string pStu { get; set; }
        public DateTime? ETD { get; set; }
		public DateTime? ATD { get; set; }
		public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? RETD { get; set; }
        public DateTime? RETA{ get; set; }
        public DateTime? ACOT { get; set; }
        public DateTime? TPAD { get; set; }
        public DateTime? TPDD { get; set; }
        public DateTime? ROH { get; set; }
        public DateTime? RTD { get; set; }

        public string Vhc { get; set; }
        public string Inf { get; set; }
        public string HndInf { get; set; }
        public string CntNoS { get; set; }
        public string SealNoS { get; set; }
        public string pInfoS { get; set; }

        public string ShpAd => SHP?.AdN ?? "";
        public string CneAd => CNE?.AdN ?? ""; // Cne == null ? "" : Cne.AdN;
        public string AccAd => ACC?.AdN ?? ""; // Acc == null ? "" : Acc.AdN;
        public string CrrAd => CRR?.AdN ?? ""; // Crr == null ? "" : Crr.AdN;

        public string nStuAd {
            get
            {
                if (nStu == null)
                    return ""; 
                var osn = Db.SQL<OSN>("select f from TMDB.OSN f where f.Stu = ?", this.nStu).FirstOrDefault();
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

        //public string Org { get; set; }
		//public string Dst { get; set; }

        public string OrgID { get; set; }
        public string DstID { get; set; }
        public LOC ORG { get; set; }
        public LOC DST { get; set; }

        public int? ShpID { get; set; }
		public int? CneID { get; set; }
		public int? AccID { get; set; }
		public int? MnfID { get; set; }
        public int? NfyID { get; set; }
        public int? CrrID { get; set; }

        public OPM OPM { get; set; }
        public FRT SHP { get; set; }
        public FRT CNE { get; set; }
        public FRT ACC { get; set; }
        public FRT MNF { get; set; }
        public FRT NFY { get; set; }
        public FRT CRR { get; set; }

        public string nStu { get; set; }
		public string pStu { get; set; }
        public string DTM { get; set; }
        public string PTM { get; set; }
        public string CntNoS { get; set; }

        public int? NOP { get; set; }
        public double? GrW { get; set; }
        public double? VM3 { get; set; }
        public int? ChW { get; set; }
        public string CABW { get; set; }

        public DateTime? nStuTS { get; set; }
        public DateTime? pStuTS { get; set; }
        public DateTime? ROH { get; set; }
        public DateTime? EOH { get; set; }
        public DateTime? REOH { get; set; }
        public DateTime? AOH { get; set; }
		public DateTime? RTR { get; set; }
		public DateTime? ROS { get; set; }
		public DateTime? POD { get; set; }
        public string PODinf { get; set; }
        public DateTime? DRBD { get; set; }
        public DateTime? DRCD { get; set; }
        public string CusLoc { get; set; }

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

        public string ShpAd => SHP?.AdN ?? ""; //Shp == null ? "" : Shp.AdN;
		public string CneAd => CNE?.AdN ?? ""; //Cne == null ? "" : Cne.AdN;
		public string AccAd => ACC?.AdN ?? ""; //Acc == null ? "" : Acc.AdN;
        public string MnfAd => MNF?.AdN ?? ""; //Mnf == null ? "" : Mnf.AdN;
        public string NfyAd => NFY?.AdN ?? ""; //Nfy == null ? "" : Nfy.AdN;
        public string CrrAd => CRR?.AdN ?? ""; // Crr == null ? "" : Crr.AdN;

        public DateTime? mETD => OPM?.ETD;
		public DateTime? mATD => OPM?.ATD;
		public DateTime? mETA => OPM?.ETA;
		public DateTime? mATA => OPM?.ATA;
        public DateTime? mRETD => OPM?.RETD;
        public DateTime? mRETA => OPM?.RETA;
        public DateTime? mACOT => OPM?.ACOT;
        public DateTime? mTPAD => OPM?.TPAD;
        public DateTime? mTPDD => OPM?.TPDD;

        public string mVhc => OPM?.Vhc ?? "";
        public string mInf => OPM?.Inf ?? "";
        public string mHndInf => OPM?.HndInf ?? "";
        public string mCntNoS => OPM?.CntNoS ?? "";
        public string mSealNoS => OPM?.SealNoS ?? "";
        public string mpInfoS => OPM?.pInfoS ?? "";

        public string mETD_t => $"{OPM?.ETD:s}";
		public string mATD_t => $"{OPM?.ATD:s}";
		public string mETA_t => $"{OPM?.ETA:s}";
        public string mATA_t => $"{OPM?.ATA:s}";
        public string mRETD_t => $"{OPM?.RETD:s}";
        public string mRETA_t => $"{OPM?.RETA:s}";
        public string mACOT_t => $"{OPM?.ACOT:s}";
        public string mTPAD_t => $"{OPM?.TPAD:s}";
        public string mTPDD_t => $"{OPM?.TPDD:s}";

        public string nStuAd
        {
            get
            {
                if (nStu == null)
                    return "";
                var osn = Db.SQL<OSN>("select f from TMDB.OSN f where f.Stu = ?", this.nStu).FirstOrDefault();
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