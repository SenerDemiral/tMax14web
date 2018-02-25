using System;
using System.Linq;
using System.Net;
using System.Text;
using Starcounter;
using TMDB;

namespace Tracking
{
    class Program
    {
        enum DW3 { TrckID, Cmnd, Trh, GPS, Lat, Lon, Speed, Zmn, Direction, Alt, Star };

        static void Main()
        {
            Handle.Tcp(6002, (TcpSocket tcpSocket, Byte[] incomingData) =>
            {
                UInt64 socketId = tcpSocket.ToUInt64();
                // Checking if we have socket disconnect here.
                if (null == incomingData)
                {
                    // One can use "socketId" to dispose resources associated with this socket.
                    return;
                }
                //byte[] bytes = Encoding.ASCII.GetBytes(someString);
                string text = Encoding.ASCII.GetString(incomingData);
                TMDB.Hlpr.WriteTrackingLog($"Received 6002/TCP: {text}");

                /// One can check if there are any resources associated with "socketId" and otherwise create them.
                /// Db.SQL("...");

                /// Sending the echo back.
                //tcpSocket.Send(incomingData);

                /// Or even more data, if its needed: tcpSocket.Send(someMoreData);
            });


            Handle.Udp(6000, (IPAddress clientIp, UInt16 clientPort, Byte[] datagram) =>
            {
                string text = Encoding.ASCII.GetString(datagram);

                TMDB.Hlpr.WriteTrackingLog("Received 6000/UDP: " + clientIp.ToString() + " " + clientPort.ToString() + " " + text);

                // One can update any resources associated with "clientIp" and "clientPort".
                //UdpSocket.Send(clientIp, clientPort, 6000, Encoding.ASCII.GetBytes("ok"));

                if (text.StartsWith("##,imei:"))
                {
                    // Login Packet received ##,imei:864180034846423,A;
                    TMDB.Hlpr.WriteTrackingLog("Login packet received, sending LOAD");
                    UdpSocket.Send(clientIp, clientPort, 6000, "LOAD");
                }
                else if (text.StartsWith("imei:"))
                {
                    // Position received
                    // imei:864180034846423,work notify,180108164145,,F,084144.000,A,3701.6108,N,02733.8299,E,0.00,0;
                    TMDB.Hlpr.WriteTrackingLog("Position received");
                    string[] msgss = text.Split(new char[] { ':', ',', ';' });
                    if (msgss[5] == "F")    // Valid GPS data
                    {
                        string id = msgss[1];
                        double lat = LatDMCtoDD(msgss[8]+msgss[9]);
                        double lon = LonDMCtoDD(msgss[10]+msgss[11]);

                        Db.Transact(() =>
                        {
                            //var th = Db.FromId<TMDB.TH>(ulong.Parse(trckID));
                            var th = Db.SQL<TMDB.TH>("select h from TMDB.TH h where h.ID = ?", id).FirstOrDefault();
                            if (th == null)
                            {
                                new TMDB.TH
                                {
                                    ID = id,
                                    Lat = lat.ToString(),
                                    Lng = lon.ToString(),
                                    LTS = DateTime.Now,
                                    CntNo = "ECBU5018958"
                                };
                            }
                            else
                            {
                                th.Lat = lat.ToString();
                                th.Lng = lon.ToString();
                                th.LTS = DateTime.Now;
                                th.CntNo = "ECBU5018958";

                            }
                        });

                    }

                    // UdpSocket.Send(clientIp, clientPort, 6002, "LOAD");
                }
                else
                {
                    // Heartbeat Pocket received
                    TMDB.Hlpr.WriteTrackingLog("Heartbeat received, sending ON");
                    UdpSocket.Send(clientIp, clientPort, 6000, "ON");
                }

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