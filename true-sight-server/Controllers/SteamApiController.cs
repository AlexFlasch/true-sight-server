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
		private const int HASH_SIZE = 16;
		private string connectionString =
			"Server=tcp:xyymsldado.database.windows.net,1433;Database=true-sight;User ID=true-sight@xyymsldado;Password=Sh0cking;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public SteamApiController()
		{
			SteamWebAPI.SetGlobalKey(ConfigurationManager.AppSettings["SteamApiKey"]);
		}

        [System.Web.Http.ActionName("GetSteamIdAction")]
        public ResponseBase GetSteamIdFromVanityUrl(string email, string vanityUrl, bool saveVanityUrl)
	    {
			var steamId = SteamWebAPI.General().ISteamUser().ResolveVanityURL(vanityUrl).GetResponse();

            if (saveVanityUrl)
            {
                SaveSteamIdForUser(email, steamId.ToString());
            }

	        return steamId;
	    }
        
        [System.Web.Http.ActionName("RegisterUserAction")]
		public void RegisterUser(string email, string password)
		{
			string hashedPassword = HashPassword(email, password);
			
			using (SqlConnection conn = new SqlConnection(connectionString))
			using (SqlCommand command = new SqlCommand("dbo.InsertUser", conn)
			{
				CommandType = CommandType.Procedure	
			})
			{
				conn.Open();
				command.Add(new SqlParameter("@email", email));
				command.Add(new SqlParameter("@password", hashedPassword));
				conn.ExecuteNonQuery(command);
				conn.close();
			}
		}
		
		

	    public void SaveSteamIdForUser(string email, string steamId)
	    {
		    using (SqlConnection connection = new SqlConnection(connectionString))
			using (SqlCommand commmand = new SqlCommand("dbo.InsertSteamId", conn)
			{
				CommandType = CommandType.Procedure
			})
		    {
			    conn.Open();
				command.Add(new SqlParameter("@email", email));
				command.Add(new SqlParameter("@steamId", steamId));
				conn.ExecuteNonQuery(command);
				conn.Close();
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

		private static string HashPassword(string email, string password)
		{
            HashAlgorithm hash = new SHA256CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(email += password);
            byte[] byteHash = hash.ComputeHash(byteValue);
			return Convert.ToBase64String(byteHash);
		}
	}
}
