﻿using System;
using System.Linq;
using Starcounter;
using System.Net;
using System.Text;
using TMDB;

namespace Tracking
{
    class Program
    {
        enum DW3 { TrckID, Cmnd, Trh, GPS, Lat, Lon, Speed, Zmn, Direction, Alt, Star };

        static void Main()
        {
            Handle.Udp(6000, (IPAddress clientIp, UInt16 clientPort, Byte[] datagram) =>
            {
                string text = Encoding.ASCII.GetString(datagram);

                TMDB.Hlpr.WriteTrackingLog("Received on: " + clientIp.ToString() + " " + clientPort.ToString() + " " + text);

                // One can update any resources associated with "clientIp" and "clientPort".
                //UdpSocket.Send(clientIp, clientPort, 6000, Encoding.ASCII.GetBytes("ok"));
                UdpSocket.Send(clientIp, clientPort, 6000, "ok");

                if (text.StartsWith("(") && text.EndsWith(")"))
                    processMessages(text);
            });

            Handle.GET("/bodved/IndexCreate", () =>
            {
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxTH_ID").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxTH_ID ON TH (UNIQUE ID )");

                return "OK IndexCreate";
            });

            Handle.GET("/bodved/IndexDrop", () =>
            {
                Db.SQL("DROP INDEX IdxTH_ID ON TH");

                return "OK IndexDrop";
            });

            Handle.GET("/Tracking/initTH", () =>
            {
                Db.Transact(() =>
               {
                   Db.SQL("delete from TH");
               });
                return "OK initTH";
            });

            Handle.GET("/Tracking/denemeMsj", () => 
            {
                string msg = "(864768011110084,DW3C,291017,A,4104.1572N,02852.2621E,0.65,100155,0.00,193.00,11,0)";
                processMessages(msg);
                msg = "(864768011110084,DW3C,291017,A,4104.1656N,02852.4349E,0.06,052805,0.00,1.00,13,0)";
                processMessages(msg);
                msg = "(864768011110084,DW3C,291017,A,4104.1565N,02852.3095E,0.41,075052,0.00,161.40,11,0)";
                processMessages(msg);
                msg = "(864768011110084,DW3C,291017,A,4104.1456N,02852.3263E,0.01,095159,0.00,145.30,11,0)";
                processMessages(msg);
                return "OK";
            });
        }

        static void processMessages(string txt)
        {
            string[] msgss = txt.Split(new char[] { ')' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string msg in msgss)
            {
                processSingleMessage(msg.Substring(1)); // Ilk karakter '(' gec
            }
        }

        static void processSingleMessage(string msg)
        {
            string[] items = msg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string trckID = items[0];
            string cmnd = items[1];

            if (cmnd == "DW30" || cmnd == "DW3B" || cmnd == "DW3C")
            {
                string Trh = items[(int)DW3.Trh];   // ddMMyy
                string Zmn = items[(int)DW3.Zmn];   // HHmmss
                DateTime EXD = new DateTime(Convert.ToInt32(Trh.Substring(4)) + 2000, Convert.ToInt32(Trh.Substring(2, 2)), Convert.ToInt32(Trh.Substring(0, 2)), Convert.ToInt32(Zmn.Substring(0, 2)), Convert.ToInt32(Zmn.Substring(2, 2)), Convert.ToInt32(Zmn.Substring(4)));
                string GPS = items[(int)DW3.GPS];   // A:GPS valid data, V:GPS invalid data
                string LatDMC = items[(int)DW3.Lat];   // ddmm.mmmmC  C:N+/S-
                double LatDD = LatDMCtoDD(LatDMC);
                string LonDMC = items[(int)DW3.Lon];   // dddmm.mmmmC  C:E+/W-
                double LonDD = LonDMCtoDD(LonDMC);

                //Hlpr.WriteTrackingLog(string.Format("Cmnd:{0} TrckID:{1} EXD:{2} Lat,Lon:{3},{4}", cmnd, trckID, EXD, LatDD, LonDD));
                Hlpr.WriteTrackingLog($"Cmnd:{cmnd} EXD:{EXD} Lat,Lon:{LatDD},{LonDD}");

                Db.Transact(() =>
                {
                    //var th = Db.FromId<TMDB.TH>(ulong.Parse(trckID));
                    var th = Db.SQL<TMDB.TH>("select h from TMDB.TH h where h.ID = ?", trckID).FirstOrDefault();
                    if (th == null)
                    {
                        new TMDB.TH
                        {
                            ID = trckID,
                            Lat = LatDD.ToString(),
                            Lng = LonDD.ToString(),
                            LTS = EXD,
                            CntNo = "ECBU5001127"
                        };
                    }
                    else
                    {
                        th.Lat = LatDD.ToString();
                        th.Lng = LonDD.ToString();
                        th.LTS = EXD;
                        th.CntNo = "ECBU5001127";
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
    }
}