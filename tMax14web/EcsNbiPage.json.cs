using Starcounter;
using System;

namespace tMax14web
{
    partial class EcsNbiPage : Json
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
                fN = "EOH_t",
                fC = "RLD",
                fT = "Requested Loading Date"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "EOH2_t",
                fC = "BLD",
                fT = "Booked Loading Slot"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "EOH3_t",
                fC = "BLD",
                fT = "Effective Loading From"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "AOH_t",
                fC = "AOH",
                fT = "Effective Loading To"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mSealNoS",
                fC = "Seal#",
                fT = "Seal#"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "NOP",
                fC = "NoP",
                fT = "Number of Package"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "GrW",
                fC = "GrW",
                fT = "Gross Weight"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mInf",
                fC = "RC/D",
                fT = "Reason for Change/Delay"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "Cusloc",
                fC = "CL",
                fT = "Customs Location"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "AOC_t",
                fC = "AOC",
                fT = "Customs In"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "RTD_t",
                fC = "RTD",
                fT = "Customs Out"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mATD_t",
                fC = "RDD",
                fT = "Terminal Departure"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "hOthInf",
                fC = "ExDsc",
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
            foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where h.POD >= ? and ROT = ? and MOT = ?", std, "E", "R"))
            {
                oph = Ophs.Add();
                oph.mRefNo = h.OPM?.RefNo;
                oph.OpmID = h.OpmID ?? 0;
                oph.mCntNoS = h.CntNoS;
                oph.OrgAd = h.ORG?.Ad;
                oph.EOH_t = $"{h.EOH:s}";
                oph.EOH2_t = $"{h.EOH:s}";
                oph.EOH3_t = $"{h.EOH:s}";
                oph.AOH_t = $"{h.AOH:s}";
                oph.mSealNoS = h.OPM?.SealNoS;
                oph.NOP = (long)h.NOP;
                oph.GrW = (decimal)h.GrW;
                oph.mInf = h.OPM?.Inf;
                oph.CusLocAd = h.CUSLOC?.Ad;
                oph.RTD_t = $"{h.RTD:s}";
                oph.AOC_t = $"{h.AOC:s}";
                oph.mATD_t = $"{h.OPM?.ATD:s}";
                oph.OthInf = h.OthInf;
            }

            //sener.NoR = DateTime.Now.Ticks;
            //int NOP = Ophs.Count;

        }
    }
}
