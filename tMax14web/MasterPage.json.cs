using Starcounter;

namespace tMax14web
{
	partial class MasterPage : Json
	{
		void Handle(Input.DownloadTrigger action)
		{
			CurrentPage = Self.GET("/tMax14web/ophs2xlsx/57036/2016-01-01");
		}


	}
}
