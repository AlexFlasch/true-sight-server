using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableSteam;
using PortableSteam.Infrastructure;

namespace true_sight_server.Controllers
{
	public class SteamApiController : ApiController
	{
		private const int saltValSize = 16;
		private string connectionString =
			"Server=tcp:xyymsldado.database.windows.net,1433;Database=true-sight;User ID=true-sight@xyymsldado;Password=Sh0cking;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public SteamApiController()
		{
			SteamWebAPI.SetGlobalKey(ConfigurationManager.AppSettings["SteamApiKey"]);
		}

        public ResponseBase GetSteamIdFromVanityUrl(string username, string vanityUrl)
	    {
            /*Uri uri = new Uri(@"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + apiKey + "&vanityurl=" + vanityUrl);

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
            
            return obj;*/

			var steamId = SteamWebAPI.General().ISteamUser().ResolveVanityURL(vanityUrl).GetResponse();

	        using (SqlConnection connection = new SqlConnection(connectionString))
	        {
		        
	        }

	        return steamId;
	    }

	    public void SaveSteamIdForUser(string username, string steamId)
	    {
		    using (SqlConnection connection = new SqlConnection(connectionString))
		    {
			    
		    }
	    }

		private static string GenerateSaltValue()
		{
			UnicodeEncoding utf16 = new UnicodeEncoding();

			if (utf16 != null)
			{
				Random random = new Random(unchecked ((int)DateTime.Now.Ticks));

				if (random != null)
				{
					byte[] saltValue = new byte[saltValSize];

					random.NextBytes(saltValue);

					string saltValueString = utf16.GetString(saltValue);

					return saltValueString;
				}
			}

			return null;
		}

		private static string HashPassword(string clearData, string saltValue, HashAlgorithm hash)
		{
			UnicodeEncoding encoding = new UnicodeEncoding();

			if (clearData != null && hash != null && encoding != null)
			{
				if (saltValue == null)
				{
					saltValue = GenerateSaltValue();
				}

				byte[] binarySaltValue = new byte[saltValSize];

				for (int i = 0; i <= saltValSize; i++)
				{
					binarySaltValue[i] = byte.Parse(saltValue.Substring(i*2, 2), System.Globalization.NumberStyles.HexNumber,
						CultureInfo.InvariantCulture.NumberFormat);
				}

				byte[] valueToHash = new byte[saltValSize + encoding.GetByteCount(clearData)];
				byte[] binaryPassword = encoding.GetBytes(clearData);

				binarySaltValue.CopyTo(valueToHash, 0);
				binaryPassword.CopyTo(valueToHash, saltValSize);

				byte[] hashValue = hash.ComputeHash(valueToHash);

				string hashedPassword = saltValue;

				foreach (byte hexdigit in hashValue)
				{
					hashedPassword += hexdigit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat);
				}

				return hashedPassword;
			}
		}
	}
}
