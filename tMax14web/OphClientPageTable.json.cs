using Starcounter;
using System;

namespace tMax14web
{
    partial class OphClientPageTable : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var parent = (MasterPage)this.Parent;
            var fid = Convert.ToInt32(parent.fID);
            var std = Convert.ToDateTime(parent.StartDate);
            fID = parent.fID;
            StartDate = parent.StartDate;

            if (!parent.fOnLine)
                return;

            Ophs = Db.SQL<TMDB.OPH>("select h from OPH h where (h.ShpID = ? or h.CneID = ? or h.AccID = ?) and h.EXD >= ?", fid, fid, fid, std);

            sener.NoR = DateTime.Now.Ticks;
        }
    }
}
