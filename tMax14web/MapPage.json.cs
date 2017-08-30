using Starcounter;

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

            double LAT = 37.026835, LON = 27.563807;

            /*
            for (int i = 0; i < 10; i++)
            {

                //var a = new MarkersElementJson();
                var a = new MarkersItem();

                a.Lat = string.Format("{0}", LAT + (double)i / 100.0);
                a.Lon = string.Format("{0}", LON + (double)i / 100.0);

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
                Lat = "37.026835",
                Lon = "27.563807",
                Title = "Sener",
                Info = "Demiral Ev"
            });

            MapList.Add(new MapListItem()
            {
                Idx = 1,
                Lat = "37.033568",
                Lon = "27.423496",
                Title = "Marina",
                Info = "<strong>Bodrum<br>Limani</strong>"
            });

            MapList.Add(new MapListItem()
            {
                Idx = 2,
                Lat = "37.096046",
                Lon = "27.270502",
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
                        Lat = this.Lat,
                        Lon = this.Lon,
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
