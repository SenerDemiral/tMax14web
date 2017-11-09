using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.IO;

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
        //static StreamWriter sw = null; // new StreamWriter(@"C:\Starcounter\MyLog\tMaxLogin-Log.txt", true);

        public static void WriteLoginLog(string Msg)
        {
            //StreamWriter sw = null;
            //if (sw == null)
            //    sw = new StreamWriter(@"C:\Starcounter\MyLog\tMaxLogin-Log.txt", true);

            try
            {
                StreamWriter sw = new StreamWriter(@"C:\Starcounter\MyLog\tMaxLogin-Log.txt", true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }

        public static void WriteTrackingLog(string Msg)
        {
            try
            {
                StreamWriter sw = new StreamWriter($@"C:\Starcounter\MyLog\TrackingLog.txt", true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}
