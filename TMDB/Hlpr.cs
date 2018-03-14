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
        public static void ECBU5018958() 
        {
            using (StreamReader sr = new StreamReader($@"C:\Starcounter\tMaxData\ECBU5018958.txt", System.Text.Encoding.UTF8))
            {
                string line;
                Db.Transact(() =>
                {
                    string id = "864180034846423";
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        DateTime lts = DateTime.Parse(ra[0]);
                        double lat = LatDMCtoDD(ra[1] + ra[2]);
                        double lon = LonDMCtoDD(ra[3] + ra[4]);

                        new TH()
                        {
                            ID = id,
                            Lat = lat.ToString(),
                            Lng = lon.ToString(),
                            LTS = lts,
                            CntNo = "ECBU5018958"
                        };
                    }
                });
            }
        }

        static double LatDMCtoDD(string DMC)
        {
            // ddmm.mmmmC  C:N+/S-
            double D = Convert.ToDouble(DMC.Substring(0, 2));
            double M = Convert.ToDouble(DMC.Substring(2, 7));
            double Sgn = DMC.EndsWith("N") ? 1 : -1;   // Nort+ South-
            double DD = Math.Round(Sgn * (D + M / 60.0), 6);
            return DD;
        }

        static double LonDMCtoDD(string DMC)
        {
            // dddmm.mmmmC  C:E+/W-
            double D = Convert.ToDouble(DMC.Substring(0, 3));
            double M = Convert.ToDouble(DMC.Substring(3, 7));
            double Sgn = DMC.EndsWith("E") ? 1 : -1;   // East+ West-
            double DD = Math.Round(Sgn * (D + M / 60.0), 6);
            return DD;
        }

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

        public static void WriteRestLog(string Msg)
        {
            try
            {
                StreamWriter sw = new StreamWriter($@"C:\Starcounter\MyLog\RestLog.txt", true);
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
        public static void AS5000Log(string Msg)
        {
            try
            {
                StreamWriter sw = new StreamWriter($@"C:\Starcounter\MyLog\AS5000.txt", true);
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
