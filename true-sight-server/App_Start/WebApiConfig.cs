using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace true_sight_server
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "SteamApi",
				routeTemplate: "api/{controller}",
				defaults: new { id = RouteParameter.Optional }
			);

            config.Routes.MapHttpRoute(
                name: "GetSteamIdFromVanityUrl",
                routeTemplate: "api/{controller}/{email}/{vanityUrl}/{saveVanityUrl}",
                defaults: new { action = "GetSteamIdAction" }
            );

            config.Routes.MapHttpRoute(
                name: "RegisterUser",
                routeTemplate: "api/{controller}/{email}/{password}",
                defaults: new { action = "RegisterUserAction" }
            );

		    config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(config.Formatters.JsonFormatter);
		}
	}
}
