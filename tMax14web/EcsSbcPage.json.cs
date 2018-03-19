using Starcounter;
using System;

namespace tMax14web
{
    partial class EcsSbcPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            Fields.Add(new FieldsElementJson
            {
                fN = "mRefNo",   // FieldName
                fC = "Ref#",     // FieldCaption
                fT = "ECS Ref#"  // FieldTooltip
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "OpmID",
                fC = "ID",
                fT = "Transorient Ref#"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "AccAd",
                fC = "Client",
                fT = "Client"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mCntNoS",
                fC = "Cntr#",
                fT = "Container#"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "Org",
                fC = "LP",
                fT = "Loading Place"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mPOU",
                fC = "POU",
                fT = ""
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mETA_t",
                fC = "FT ETA",
                fT = "Final Train ETA"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mATA_t",
                fC = "EfcTrnArv",
                fT = "Effective Train Arrival"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "Cusloc",
                fC = "CL",
                fT = "Customs Location"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "DRBD_t",
                fC = "CI",
                fT = "Customs In (Terminal)"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "DRCD_t",
                fC = "CO",
                fT = "Customs Out"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "Dst",
                fC = "DlvryLoc",
                fT = "Delivery Location"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "ROS_t",
                fC = "RD",
                fT = "Requested Delivery"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "ROH_t",
                fC = "BDS",
                fT = "Booked Delivery Slot"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "PODinf",
                fC = "EDTF",
                fT = "Effective Delivery Time From"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "POD_t",
                fC = "EDTT",
                fT = "Effective Delivery Time To"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mpInfoS",
                fC = "RC/D",
                fT = "Reason for Change/Delay"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mHndInf",
                fC = "ECD&A",
                fT = "Extra Cost Description and Amount"
            });

            var parent = (MasterPage)this.Parent;
            var fid = Convert.ToInt32(parent.fID);
            var std = Convert.ToDateTime(parent.StartDate);

            if (!parent.fOnLine)
                return;
            OphsElementJson oph;
            //Ophs.Data = Db.SQL<TMDB.OPH>("select h from OPH h where (h.ShpID = ? or h.CneID = ? or h.AccID = ?) and h.EXD >= ?", fid, fid, fid, std);

            foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where h.POD >= ? and ROT = ? and MOT = ?", std, "I", "R"))
            {
                oph = Ophs.Add();

                oph.mRefNo = h.OPM?.RefNo;
                oph.OpmID = h.OpmID ?? 0;
                oph.AccAd = h.ACC?.AdN;
                oph.mCntNoS = h.CntNoS;
                //oph.Org = h.Org;
                //oph.mPOU = h.OPM?.POU;
                oph.mETA_t = $"{h.OPM?.ETA:s}";
                oph.mATA_t = $"{h.OPM?.ATA:s}";
                oph.CusLoc = h.CusLoc;
                oph.DRBD_t = $"{h.DRBD:s}";
                oph.DRCD_t = $"{h.DRCD:s}";

                //oph.Dst = $"{h.Dst:s}";
                oph.ROS_t = $"{h.ROS:s}";
                oph.ROH_t = $"{h.ROH:s}";
                oph.PODinf = h.PODinf;
                oph.POD_t = $"{h.POD:s}";
                oph.mpInfoS = h.OPM?.pInfoS;
                oph.mHndInf = h.OPM?.HndInf;

            }

            //sener.NoR = DateTime.Now.Ticks;
            //int NOP = Ophs.Count;

        }
    }
}
