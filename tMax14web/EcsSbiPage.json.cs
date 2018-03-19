using Starcounter;
using System;

namespace tMax14web
{
    partial class EcsSbiPage : Json
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
                fN = "Cusloc",
                fC = "CL",
                fT = "Customs Location"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "RTD_t",
                fC = "AD2UP",
                fT = "Arrival Date Time to Unloading Point/Customer"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "RTR_t",
                fC = "CCF",
                fT = "Custom Clearance Finished"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "Dst",
                fC = "DlvryLoc",
                fT = "Delivery Location"
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
                fN = "mInfoS",
                fC = "RC/D",
                fT = "Reason for Change/Delay"
            });
            Fields.Add(new FieldsElementJson
            {
                fN = "mInf",
                fC = "ECD",
                fT = "Extra Cost Description"
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
                oph.CusLoc = h.CusLoc;
                oph.mRTD_t = $"{h.OPM?.RTD:s}";
                oph.RTR_t = $"{h.EOH:s}";

                //oph.Dst = $"{h.Dst:s}";
                oph.PODinf = h.PODinf;
                oph.POD_t = $"{h.POD:s}";
                oph.mpInfoS = h.OPM?.pInfoS;
                oph.mInf = h.OPM?.Inf;
                oph.mHndInf = h.OPM?.HndInf;

            }

            //sener.NoR = DateTime.Now.Ticks;
            //int NOP = Ophs.Count;

        }
    }
}
