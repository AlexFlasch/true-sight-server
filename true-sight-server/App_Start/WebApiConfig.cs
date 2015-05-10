using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace true_sight_server
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			//enable CORS
			config.EnableCors();

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "SteamApi",
				routeTemplate: "api/{controller}"
			);

            config.Routes.MapHttpRoute(
                name: "GetSteamIdFromVanityUrl",
                routeTemplate: "api/{controller}/GetSteamId/{email}/{vanityUrl}/{saveVanityUrl}",
                defaults: new { action = "GetSteamIdAction" }
            );

            config.Routes.MapHttpRoute(
                name: "RegisterUser",
                routeTemplate: "api/{controller}/Register/{email}/{password}",
                defaults: new { action = "RegisterUserAction" }
            );

			config.Routes.MapHttpRoute(
				name: "LoginUser",
				routeTemplate: "api/{controller}/Login/{email}/{password}",
				defaults: new { action = "LoginUserAction" }
			);

			config.Routes.MapHttpRoute(
				name: "GetMatchHistory",
				routeTemplate: "api/{controller}/GetMatchHistory/{steamId}",
				defaults: new { action = "GetMatchHistoryAction" }
			);

			config.Routes.MapHttpRoute(
				name: "GetMatchDetails",
				routeTemplate: "api/{controller}/GetMatchDetails/{matchId}",
				defaults: new { action = "GetMatchDetailsAction"}
			);

			config.Routes.MapHttpRoute(
				name: "GetHeroList",
				routeTemplate: "api/{controller}/GetHeroList",
				defaults: new { action = "GetHeroListAction" }
			);

			config.Routes.MapHttpRoute(
				name: "GetItemList",
				routeTemplate: "api/{controller}/GetItemList",
				defaults: new { action = "GetItemListAction" }
			);

			config.Routes.MapHttpRoute(
				name: "GetAbilitiesList",
				routeTemplate: "api/{controller}/GetAbilityList",
				defaults: new { action = "GetAbilityListAction" }
			);

			config.Routes.MapHttpRoute(
				name: "GetLobbyList",
				routeTemplate: "api/{controller}/GetLobbyList",
				defaults: new { action = "GetLobbyListAction" }
			);

			config.Routes.MapHttpRoute(
				name: "GetModeList",
				routeTemplate: "api/{controller}/GetModeList",
				defaults: new { action = "GetModeListAction"}
			);

			config.Routes.MapHttpRoute(
				name: "GetRegionList",
				routeTemplate: "api/{controller}/GetRegionList",
				defaults: new { action = "GetRegionListAction" }
			);

		    config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(config.Formatters.JsonFormatter);
		}
	}
}
