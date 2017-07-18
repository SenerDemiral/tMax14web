using Starcounter;
using System;

namespace tMax14web
{
	partial class MasterPage : Json
	{
		void Handle(Input.DownloadTrigger action)
		{
			CurrentPage = Self.GET("/tMax14web/ophs2xlsx/57036/2016-01-01");
		}

        void Handle(Input.fID action)
        {
            fOnLine = false;
        }

        void Handle(Input.LoginTrigger action)
        {
            fOnLine = false;
            if (!string.IsNullOrEmpty(fPW))
            {
                var Frt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", Convert.ToInt32(fID)).First;
                if (Frt != null && Frt.Pwd == fPW)
                {
                    fOnLine = true;
                    fAdN = Frt.AdN;
                    fAd = Frt.Ad;

                }
            }
        }

    }
}
