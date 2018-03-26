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
                fT = "TO Ref#"
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
                fT = "Final Location"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mETA_t",
                fC = "FT ETA",
                fT = "Final ETA"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mATA_t",
                fC = "EfcTrnArv",
                fT = "Effective Arrival"
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
                fT = "Customs In Terminal"
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
                fT = "Effective Delivery From"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "POD_t",
                fC = "EDTT",
                fT = "Effective Delivery To"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mInf",
                fC = "RC/D",
                fT = "Reason for Change/Delay"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "OthInf",
                fC = "ECD&A",
                fT = "Extra Cost Description"
            });

            var parent = (MasterPage)this.Parent;
            var fid = Convert.ToInt32(parent.fID);
            var std = Convert.ToDateTime(parent.StartDate);
            fID = parent.fID;
            StartDate = parent.StartDate;

            if (!parent.fOnLine)
                return;

            OphsElementJson oph;
            foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where h.POD >= ? and ROT = ? and MOT = ?", std, "I", "R"))
            {
                oph = Ophs.Add();
                oph.mRefNo = h.OPM?.RefNo;
                oph.OpmID = h.OpmID ?? 0;
                oph.AccAd = h.AccAd;
                oph.mCntNoS = h.CntNoS;
                oph.OrgAd = h.ORG?.Ad;
                oph.mPouAd = h.OPM?.POU?.Ad;
                oph.mETA_t = $"{h.OPM?.ETA:s}";
                oph.mATA_t = $"{h.OPM?.ATA:s}";
                oph.CusLocAd = h.CUSLOC?.Ad;
                oph.DRBD_t = $"{h.DRBD:s}";
                oph.DDT_t = $"{h.DDT:s}";
                oph.DstAd = h.DST?.Ad;
                oph.ROS_t = $"{h.ROS:s}";
                oph.ROH_t = $"{h.ROH:s}";
                oph.PODinf = h.PODinf;
                oph.POD_t = $"{h.POD:s}";
                oph.mInf = h.OPM?.Inf;
                oph.OthInf = h.OthInf;

            }

            //sener.NoR = DateTime.Now.Ticks;
            //int NOP = Ophs.Count;

        }
    }
}
