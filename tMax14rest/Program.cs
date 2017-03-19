using System;
using Starcounter;

namespace tMax14rest
{
	class Program
	{
		static void Main()
		{
			Handle.PUT("/tMax14rest/ResetAll", (OphMsg hJ) =>
			{
				Db.Transact(() =>
				{
					Db.SQL("DELETE FROM OPH");
					Db.SQL("DELETE FROM OPM");
					Db.SQL("DELETE FROM FRT");
				});

				return "OK";
			});

			Handle.PUT("/tMax14rest/OPH", (OphMsg hJ) => 
			{
				string rMsg = "OK";
				// hM'nin ilk field da OPH olmali
				if(hJ[0].ToString() != "OPH")
					return "!HATA";

				Db.Transact(() =>
				{
					try
					{
						int OphID = int.Parse(hJ.OphID);

						if(hJ.Evnt == "D")
						{
							var ophs = Db.SQL<TMDB.OPH>("select h from OPH h where h.OphID = ?", OphID);
							foreach(var hr in ophs)
							{
								//Db.SQL("delete from OPH h where h.OphID = ?", OphID);	Boyle yapamiyor!!
								hr.Delete();
							}
						}
						else
						{
							TMDB.OPH h = Db.SQL<TMDB.OPH>("select h from OPH h where h.OphID = ?", OphID).First;
							if(hJ.Evnt == "I"&& h == null)
							{
								h = new TMDB.OPH();
							}

							if(h == null)
								rMsg = "NoHouse2Update";
							else
							{
								h.OphID = OphID;
								h.OpmID = hJ.OpmID == "" ? (int?)null : Convert.ToInt32(hJ.OpmID);
								h.ROT = hJ.ROT;
								h.MOT = hJ.MOT;
								h.Org = hJ.Org;
								h.Dst = hJ.Dst;
								h.ShpID = hJ.ShpID == "" ? (int?)null : Convert.ToInt32(hJ.ShpID);
								h.CneID = hJ.ShpID == "" ? (int?)null : Convert.ToInt32(hJ.ShpID);
								h.AccID = hJ.ShpID == "" ? (int?)null : Convert.ToInt32(hJ.ShpID);

								h.EXD = hJ.EXD == "" ? (DateTime?)null : Convert.ToDateTime(hJ.EXD);
								h.nStuTS = hJ.nStuTS == "" ? (DateTime?)null : Convert.ToDateTime(hJ.nStuTS);
								h.pStuTS = hJ.pStuTS == "" ? (DateTime?)null : Convert.ToDateTime(hJ.pStuTS);
								h.REOH = hJ.REOH == "" ? (DateTime?)null : Convert.ToDateTime(hJ.REOH);
								h.EOH = hJ.EOH == "" ? (DateTime?)null : Convert.ToDateTime(hJ.EOH);
								h.AOH = hJ.AOH == "" ? (DateTime?)null : Convert.ToDateTime(hJ.AOH);
								h.RTR = hJ.RTR == "" ? (DateTime?)null : Convert.ToDateTime(hJ.RTR);
								h.POD = hJ.POD == "" ? (DateTime?)null : Convert.ToDateTime(hJ.POD);

								if(h.OpmID != null)
								{
									h.Opm = Db.SQL<TMDB.OPM>("select m from OPM m where m.OpmID = ?", h.OpmID).First;
									if(h.Opm == null)
										rMsg += $"NoMaster:{h.OpmID} ";
								}
								if(h.ShpID != null)
									h.Shp = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", h.ShpID).First;
								if(h.CneID != null)
									h.Cne = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", h.CneID).First;
								if(h.AccID != null)
									h.Acc = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", h.AccID).First;
							}
						}
					}
					catch(Exception ex)
					{
						rMsg = ex.Message;
					}
				});
				return rMsg;
			});

		}
	}
}