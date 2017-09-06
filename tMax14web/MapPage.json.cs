using Starcounter;
using System;

namespace tMax14web
{
    [MapPage_json]
    public partial class MapPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            MapList.Clear();
            Markers.Clear();

            /*
            Distance between 2 LatLng
            http://andrew.hedges.name/experiments/haversine/
            dlon = lon2 - lon1 
            dlat = lat2 - lat1 
            a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2 
            c = 2 * atan2( sqrt(a), sqrt(1-a) ) 
            d = R * c (where R is the radius of the Earth = 6373Km  6357-6378)


            function findDistance(frm) {
		        var t1, n1, t2, n2, lat1, lon1, lat2, lon2, dlat, dlon, a, c, dm, dk, mi, km;
		
		        // get values for lat1, lon1, lat2, and lon2
		        t1 = frm.lat1.value;
		        n1 = frm.lon1.value;
		        t2 = frm.lat2.value;
		        n2 = frm.lon2.value;
		
		        // convert coordinates to radians
		        lat1 = deg2rad(t1);
		        lon1 = deg2rad(n1);
		        lat2 = deg2rad(t2);
		        lon2 = deg2rad(n2);
		
		        // find the differences between the coordinates
		        dlat = lat2 - lat1;
		        dlon = lon2 - lon1;
		
		        // here's the heavy lifting
		        a  = Math.pow(Math.sin(dlat/2),2) + Math.cos(lat1) * Math.cos(lat2) * Math.pow(Math.sin(dlon/2),2);
		        c  = 2 * Math.atan2(Math.sqrt(a),Math.sqrt(1-a)); // great circle distance in radians
		        dm = c * Rm; // great circle distance in miles
		        dk = c * Rk; // great circle distance in km
		
		        // round the results down to the nearest 1/1000
		        mi = round(dm);
		        km = round(dk);
		
		        // display the result
		        frm.mi.value = mi;
		        frm.km.value = km;
	        }
	
	
	        // convert degrees to radians
	        function deg2rad(deg) {
		        rad = deg * Math.PI/180; // radians = degrees * pi/180
		        return rad;
	        }
          	// round to the nearest 1/1000
	        function round(x) {
		        return Math.round( x * 1000) / 1000;
	        }
            */
            double R = 6373;
            double lat1 = 37.027035 * Math.PI / 180, lon1 = 27.563505 * Math.PI / 180; 
            double lat2 = 37.027168 * Math.PI / 180, lon2 = 27.563957 * Math.PI / 180;
            double dlon = lon2 - lon1;
            double dlat = lat2 - lat1;
            double a = Math.Pow(Math.Sin(dlat / 2.0), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2.0), 2);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            double d = c * R;   // in Km  Sonuc 0.043km OK


            double LAT = 37.026835, LON = 27.563807;

            /*
            for (int i = 0; i < 10; i++)
            {

                //var a = new MarkersElementJson();
                var a = new MarkersItem();

                a.lat = string.Format("{0}", LAT + (double)i / 100.0);
                a.lng = string.Format("{0}", LON + (double)i / 100.0);

                Markers.Add(a);
            }
            */
            /*
            Markers.Add(new MarkersItem()
            {
                Lat = "37.026835",
                Lon = "27.563807",
                Title = "Ev",
                Info = "Bodrum Ev"
            });
            */

            MapList.Add(new MapListItem()
            {
                Idx = 0,
                lat = "37.026835",
                lng = "27.563807",
                Title = "Sener",
                Info = "Demiral Ev"
            });

            MapList.Add(new MapListItem()
            {
                Idx = 1,
                lat = "37.033568",
                lng = "27.423496",
                Title = "Marina",
                Info = "<strong>Bodrum<br>Limani</strong>"
            });

            MapList.Add(new MapListItem()
            {
                Idx = 2,
                lat = "37.096046",
                lng = "27.270502",
                Title = "Nil",
                Info = "<strong>Pakyurek<br>Ev</strong>"
            });
            /*
                        Markers.Add(new MarkersItem()
                        {
                            Lat = "37.026835",
                            Lon = "27.563807",
                            Title = "Ev",
                            Info = "Bodrum Ev"
                        });
                        Markers.Add(new MarkersItem()
                        {
                            Lat = "37.033568",
                            Lon = "27.423496",
                            Title = "Marina",
                            Info = "Bodrum Limani"
                        });*/
        }

        [MapPage_json.Markers]
        public partial class MarkersItem : Json
        {
            protected override void OnData()
            {
                base.OnData();
            }

            void Handle(Input.ClickTrigger Action)
            {
                //Open = Open ? false : true;
                IsVisible = IsVisible ? false : true;
                /*
                var parent = (MapPage)this.Parent.Parent;
                parent.Markers.Add(new MarkersItem()
                {
                    Lat = "37.04",
                    Lon = "27.423496",
                    Title = "Marina",
                    Info = "Bodrum Limani"
                });*/

            }

        }

        [MapPage_json.MapList]
        public partial class MapListItem : Json
        {
            void Handle(Input.Show Action)
            {
                var parent = (MapPage)this.Parent.Parent;
                if (Show == 0)
                {
                    parent.Markers.Add(new MarkersItem()
                    {
                        Idx = this.Idx,
                        lat = this.lat,
                        lng = this.lng,
                        Title = this.Title,
                        Info = this.Info
                    });
                }
                else
                {
                    ShowInfo = ShowInfo ? false : true;
                    //parent.Markers[0].ShowInfo = ShowInfo;
                    var ms = parent.Markers;
                    foreach(var m in ms)
                    {
                        if (m.Idx == Idx)
                        {
                            //m.ShowInfo = ShowInfo;
                            m.ShowInfo = m.ShowInfo ? false : true;
                            break;
                        }

                    }
                }
            }
        }
        
        //[MapPage_json.LatLons]
            //[MapPage_json.Markers]
            /*
            partial class MarkersPage
            {
                protected override void OnData()
                {
                    base.OnData();
                }

                void Handle(Input.ClickTrigger action)
                {

                }
            }*/
        }

    }
