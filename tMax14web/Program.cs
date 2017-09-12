using System.IO;
using System;
using Starcounter;


namespace tMax14web
{
	class Program
	{
		static void Main()
		{
			var HTML = @"<!DOCTYPE html>
				<html>
				<head>
					<meta charset=""utf-8"">
				    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=yes"">
					<title>{0}</title>
					
                    <script src=""/sys/webcomponentsjs/webcomponents.min.js""></script>
					<script src=""/sys/document-register-element/build/document-register-element.js""></script>

                    <script>
                        window.Polymer = {{
                            dom: ""shadow""
                        }};
                    </script>

					<link rel=""import"" href=""/sys/polymer/polymer.html"">
					<link rel=""import"" href=""/sys/starcounter.html"">
					<link rel=""import"" href=""/sys/starcounter-include/starcounter-include.html"">
					
					<!--
                    <link rel=""import"" href=""/sys/bootstrap.html"">
					<link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html"">
					<script src=""/sys/webcomponentsjs/webcomponents.min.js""></script>
					<link rel=""import"" href=""/sys/iron-icons/maps-icons.html"">
					<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css"" integrity=""sha384-rwoIResjU2yc3z8GV/NPeZWAv56rSmLldC3R/AZzGRnGxQQKnKkoFVhFQhNUwEyJ"" crossorigin=""anonymous"">
					
					<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css"" crossorigin=""anonymous"">
					<link rel=""stylesheet"" href=""/sys/Stylesheet1.css"">
					<link rel=""import"" href=""/sys/github-browser-app.html"">
					-->

                    <script src=""/sys/thenBy.js""></script>
					<link rel=""stylesheet"" href=""/sys/normalize.css"">
					<link rel=""stylesheet"" href=""/sys/Stylesheet2.css"">
				
				</head>

                <body>
					<template is=""dom-bind"" id=""puppet-root"">
            		<starcounter-include view-model=""{{{{model}}}}""></starcounter-include>
                    </template>
					<palindrom-client ref=""puppet-root"" remote-url=""{1}""></palindrom-client>
				</body>

				</html>";
            //
            //		< puppet-client ref=""puppet-root"" remote-url=""{1}"" use-web-socket=""true"" ></ puppet-client >
            /*
            		<template is=""imported-template"" content$=""{{{{model.Html}}}}"" model=""{{{{model}}}}""></template>
            		<starcounter-include view-model=""{{{{model}}}}"" partial-id$=""{{{{model.Html}}}}""></starcounter-include>
					<!--
					<starcounter-debug-aid></starcounter-debug-aid>
					<script src=""https://code.jquery.com/jquery-3.1.1.slim.min.js"" integrity=""sha384-A7FZj7v+d/sdmMqp/nOQwliLvUsJfDHW+k9Omg/a/EheAdgtzNs3hpfag6Ed950n"" crossorigin=""anonymous""></script>
					<script src=""https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js"" integrity=""sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb"" crossorigin=""anonymous""></script>
					<script src=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js"" integrity=""sha384-vBWWzlZJ8ea9aCX4pEW3rVHjgjt7zpkNpZk+02D9phzyeVkE+jo0ieGizqPLForn"" crossorigin=""anonymous""></script>
					-->
            
            */

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider(HTML));

            Handle.GET("/tMax14web/init", () => {
                Db.Transact(() =>
                {
                    var f57036 = Db.SQL<TMDB.FRT>("select f from FRT f where f.FrtID = ?", 57036).First;
                    f57036.Pwd = "57036";
                    /*
                    foreach (var h in Db.SQL<TMDB.OPH>("select h from OPH h"))
                    {
                        h.VM3 = 1;
                        h.ChW = 2;
                    }*/
                });
                return "OK";
            });
            /*
            Handle.GET("/tMax14web/Mapxxxx", () =>
            {
                //var page = new MapPage();
                //page.Data = null;
                //return page; // new MapPage();
                
            var master = (MasterPage)Self.GET("/tMax14web");
                master.CurrentPage = new MapPage();
                master.CurrentPage.Data = null;

                return master;

            });
        */
            Handle.GET("/tMax14web/d2", () =>
            {
                var master = (MasterPage)Self.GET("/tMax14web");
                master.CurrentPage = new D2();
                master.CurrentPage.Data = null;

                return master;
            });

            //MainHandlers mh = new MainHandlers();
			//mh.CreateIndex();
            //mh.Register();

            Handle.GET("/tMax14web", () => { return Self.GET("/tMax14web/MainPage"); });

            Handle.GET("/tMax14web/partial/MainPage", () => new MainPage());
            Handle.GET("/tMax14web/MainPage", () => WrapPage<MainPage>("/tMax14web/partial/MainPage"));

            //Handle.GET("/tMax14web/partial/MasterPage", () => new MasterPage());
            //Handle.GET("/tMax14web/MasterPage", () => WrapPage<MasterPage>("/tMax14web/partial/MasterPage"));


            //Handle.GET("/tMax14web", () => { return Self.GET("/tMax14web/MasterPage"); });
            Handle.GET("/tMax14web/partial/OphClientTable", () => new OphClientPageTable());
            Handle.GET("/tMax14web/OphClientTable", () => WrapPage<OphClientPageTable>("/tMax14web/partial/OphClientTable"));

            Handle.GET("/tMax14web/partial/OphClient", () => new OphClientPage());
            Handle.GET("/tMax14web/OphClient", () => WrapPage<OphClientPage>("/tMax14web/partial/OphClient"));

            Handle.GET("/tMax14web/partial/gMap", () => new MapPage());
            Handle.GET("/tMax14web/gMap", () => WrapPage<MapPage>("/tMax14web/partial/gMap"));

            // Asagidaki gibi calismiyor (/tMax14web/Map), /tMax14web/MapX yapinca oluyor
            //Handle.GET("/tMax14web/partial/Map", () => new MapPage());
            //Handle.GET("/tMax14web/Map", () => WrapPage<MapPage>("/tMax14web/partial/Map"));
        }

        public static MasterPage GetMasterPageFromSession()
        {
            if (Session.Current == null)
            {
                Session.Current = new Session(Session.Flags.PatchVersioning);
            }

            MasterPage master = Session.Current.Data as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                Session.Current.Data = master;
            }

            return master;
        }

        private static Json WrapPage<T>(string partialPath) where T : Json
        {
            var master = GetMasterPageFromSession();

            /*
            if (master.CurrentPage != null && master.CurrentPage.GetType().Equals(typeof(T)))
            {
                return master;
            }
            */
            master.CurrentPage = Self.GET(partialPath);

            //if (master.CurrentPage.Data == null)
            {
                master.CurrentPage.Data = null; //trick to invoke OnData in partial
            }

            return master;
        }

    }
}