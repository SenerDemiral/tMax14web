using Starcounter;
using System;

namespace tMax14web
{
    partial class AfbClientPage : Json
    {
        [AfbClientPage_json]
        protected override void OnData()
        {
            base.OnData();
            var parent = (MasterPage)this.Parent;
            var fid = Convert.ToInt32(parent.fID);
            var std = Convert.ToDateTime(parent.StartDate);
            fID = parent.fID;
            StartDate = parent.StartDate;

            Afbs = Db.SQL<TMDB.AFB>("select a from AFB a where a.FrtID = ? and a.FtrTrh >= ?", fid, std);
        }
    }
}
