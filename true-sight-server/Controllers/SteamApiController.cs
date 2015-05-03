using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace true_sight_server.Controllers
{
	public class SteamApiController : ApiController
	{
	    readonly string apiKey = ConfigurationManager.AppSettings["SteamApiKey"];
        
        //GET api/<controller>
        public JObject GetSteamIdFromVanityUrl(string username, string vanityUrl)
	    {
            Uri uri = new Uri(@"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + apiKey + "&vanityurl=" + vanityUrl);

            WebRequest request = WebRequest.Create(uri);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
            var responseContent = reader.ReadToEnd();
            reader.Close();
            
            JObject obj = JsonConvert.DeserializeObject<JObject>(responseContent);
            if (obj["success"].ToString() == "1")
            {
                SaveSteamIdForUser(username, obj["steamid"].ToString());
                return obj;
            }
            
            return obj;
	    }

	    public void SaveSteamIdForUser(string username, string steamId)
	    {
	        
	    }
	}
}
