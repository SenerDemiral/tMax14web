using Starcounter;
using System;
using System.Linq;

namespace tMax14web
{
    public partial class MapPage : Json
    {
        void Handle(Input.DnmTrigger A)
        {
            double nLat = double.Parse(Markers[0].lat) + 0.01;
            double nLng = double.Parse(Markers[0].lng) + 0.01;
            //Markers[0].lat = $"{nLat}";
            //Markers[0].lng = $"{nLng}";

            //MapList[0].lat = $"{nLat}";
            //MapList[0].lng = $"{nLng}";


            Db.Transact(() =>
            {
                //var th = Db.FromId<TMDB.TH>(ulong.Parse(trckID));
                var th = Db.SQL<TMDB.TH>("select h from TMDB.TH h where h.ID = ?", "864768011110084").FirstOrDefault();
                if (th != null)
                {
                    th.Lat = $"{nLat}";
                    th.Lng = $"{nLng}";
                    th.LTS = DateTime.Now; 
                }
            });

            PushChanges();
        }

        public void PushChanges()
        {
            Session.ForAll((s, sId) => {
                var cp = (s.Store["App"] as MasterPage).CurrentPage;
                if (cp is MapPage)
                {
                    (s.Store["App"] as MasterPage).CurrentPage.Data = null;
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }

        protected override void OnData()
        {
            base.OnData();

            MapList.Clear();
            Markers.Clear();

            MapPage.MapListElementJson mapList;
            MapPage.MarkersElementJson marker;

            int i = 0;
            var th = Db.SQL<TMDB.TH>("select h from TH h");
            foreach (var t in th)
            {
                mapList = this.MapList.Add();
                mapList.Idx = i;
                mapList.lat = t.Lat;
                mapList.lng = t.Lng;
                mapList.LST_t = $"{t.LTS:dd.MM.yy HH:mm}"; // t.LTS.ToString("s");
                mapList.Title = t.CntNo;
                mapList.Info = $"<strong>{t.CntNo}</strong><br>{t.LTS:dd.MM.yy HH:mm}";

                marker = Markers.Add();
                marker.Idx = i;
                marker.lat = t.Lat;
                marker.lng = t.Lng;
                marker.Title = t.CntNo;
                marker.Info = $"<strong>{t.CntNo}</strong><br>{t.LTS:dd.MM.yy HH:mm}";

                i++;
            }
        }
   


        [MapPage_json.Markers]
        public partial class MarkersElementJson : Json
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
        public partial class MapListElementJson : Json
        {
            void Handle(Input.Show Action)
            {
                var parent = (MapPage)this.Parent.Parent;
                /*
                if (Show == 0)
                {
                    
                    parent.Markers.Add(new MarkersElementJson()
                    {
                        Idx = this.Idx,
                        lat = this.lat,
                        lng = this.lng,
                        Title = this.Title,
                        Info = this.Info
                    });
                }
                else*/
                {   
                    ShowInfo = ShowInfo ? false : true;
                    //parent.Markers[0].ShowInfo = ShowInfo;
                    var ms = parent.Markers;
                    foreach (var m in ms)
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
