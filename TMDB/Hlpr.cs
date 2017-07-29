using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace TMDB
{
    public static class Hlpr
    {
        public static void Insert2LogStat(int FrtID, string Pwd, bool OK)
        {
            Db.Transact(() =>
            {
                new TMDB.LogStat()
                {
                    FrtID = FrtID,
                    Pwd = Pwd,
                    OK = OK
                };
            });

        }
    }
}
