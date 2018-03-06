using Starcounter;
using System;
using System.Linq;

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

        void Handle(Input.LeaveTrigger action)
        {
        }

        void Handle(Input.LoginTrigger action)
        {
            fOnLine = false;
            int FrtID = Convert.ToInt32(fID);

            if (!string.IsNullOrEmpty(fPW))
            {
                var Frt = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", FrtID).FirstOrDefault();
                if (Frt != null && Frt.Pwd == fPW)
                {
                    fOnLine = true;
                    fAdN = Frt.AdN;
                    fAd = Frt.Ad;

                    if (Frt.FrtID == 29651)  // ECS menu: NBC, NBI, SBC, SBI
                        fECS = true;
                }
            }
            TMDB.Hlpr.Insert2LogStat(FrtID, fPW, fOnLine);
            TMDB.Hlpr.WriteLoginLog($"{FrtID} {fPW} {fOnLine}");
        }

    }
}
