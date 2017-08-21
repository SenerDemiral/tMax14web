using Starcounter;

namespace tMax14web
{
    partial class MapPage : Json
    {
        [MapPage_json]
        protected override void OnData()
        {
            base.OnData();

            LatLons.Clear();

            LatLons.Add(new LatLonsElementJson
            {
                Lat = "37.026835",
                Lon = "27.563807",
                Title = "Ev",
                Info = "Bodrum Ev"
            });
            
            LatLons.Add(new LatLonsElementJson
            { 
                Lat = "37.033568",
                Lon = "27.423496",
                Title = "Marina",
                Info = "Bodrum Limani"
            });
        }
    }
}
