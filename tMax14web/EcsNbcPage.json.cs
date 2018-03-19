using Starcounter;
using System;

namespace tMax14web
{
    partial class EcsNbcPage : Json
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
                fT = "Booked Loading Date"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "AOH_t",
                fC = "AOH",
                fT = "Actual On Hand"
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
                fN = "mInfoS",
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
                fN = "mRTD_t",
                fC = "AD2UP",
                fT = "Arrival Date Customer"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "RTR_t",
                fC = "CCF",
                fT = "Customs Clearance Finished"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mATD_t",
                fC = "RDD",
                fT = "Departure Date"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mHndInf",
                fC = "ExDsc",
                fT = "Extra Cost Description and Amount"
            });

            var parent = (MasterPage)this.Parent;
            var fid = Convert.ToInt32(parent.fID);
            var std = Convert.ToDateTime(parent.StartDate);
            fID = parent.fID;
            StartDate = parent.StartDate;

            if (!parent.fOnLine)
                return;
            OphsElementJson oph;
            //Ophs.Data = Db.SQL<TMDB.OPH>("select h from OPH h where (h.ShpID = ? or h.CneID = ? or h.AccID = ?) and h.EXD >= ?", fid, fid, fid, std);

            foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h where ROT = ? and MOT = ? and (h.POD >= ? or h.POD is null)", "E", "R", std))
            {
                oph = Ophs.Add();
                oph.mRefNo = h.OPM?.RefNo;
                oph.OpmID = h.OpmID ?? 0;
                oph.mCntNoS = h.CntNoS;
                oph.OrgAd = h.ORG?.Ad;
                oph.DstAd = h.DST?.Ad;
                oph.EOH_t = $"{h.EOH:s}";
                oph.EOH2_t = $"{h.EOH:s}";
                oph.AOH_t = $"{h.AOH:s}";
                oph.mSealNoS = h.OPM?.SealNoS;
                oph.NOP = (long)h.NOP;
                oph.GrW = (decimal)h.GrW;
                oph.mpInfoS = h.OPM?.pInfoS;
                oph.CusLoc = h.CusLoc;
                oph.mRTD_t = $"{h.OPM?.RTD:s}";
                oph.RTR_t = $"{h.RTR:s}";
                oph.mATD_t = $"{h.OPM?.ATD:s}";
                oph.mHndInf = h.OPM?.HndInf;

            }

            //sener.NoR = DateTime.Now.Ticks;
            //int NOP = Ophs.Count;
        }
    }
}
