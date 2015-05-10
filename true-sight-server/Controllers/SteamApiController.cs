using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableSteam;
using PortableSteam.Infrastructure;

namespace true_sight_server.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class SteamApiController : ApiController
	{
		private const int HashSize = 16;
		private const string ConnectionString = "Server=tcp:xyymsldado.database.windows.net,1433;Database=true-sight-db;User ID=true-sight@xyymsldado;Password=Sh0cking;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
		private const long SteamIdConversion = 76561197960265728;

		public SteamApiController()
		{
			SteamWebAPI.SetGlobalKey(ConfigurationManager.AppSettings["SteamApiKey"]);
		}

		#region SteamWebAPI
		[ActionName("GetSteamIdAction")]
		[AcceptVerbs("GET", "POST")]
        public ResponseBase GetSteamIdFromVanityUrl(string email, string vanityUrl, bool saveVanityUrl)
	    {
			var steamId = SteamWebAPI.General().ISteamUser().ResolveVanityURL(vanityUrl).GetResponse();

            if (saveVanityUrl)
            {
	            long steamIdLong = steamId.Data.Identity.SteamID;
	            string steamIdString = steamIdLong.ToString();
                SaveSteamIdForUser(email, steamIdString);
            }

	        return steamId;
	    }

		[ActionName("GetMatchHistoryAction")]
		[AcceptVerbs("GET", "POST")]
		public ResponseBase GetMatchHistoryFromSteamId(string steamId)
		{
			var accountId = Convert.ToInt64(steamId) - SteamIdConversion;
			var accountIdentity = SteamIdentity.FromAccountID(accountId);

			var matchHistory = SteamWebAPI.Game().Dota2().IDOTA2Match().GetMatchHistory().Account(accountIdentity).GetResponse();

			return matchHistory;
		}

		[ActionName("GetMatchDetailsAction")]
		[AcceptVerbs("GET", "POST")]
		public ResponseBase GetMatchDetailsFromMatchId(string matchId)
		{
			var matchIdLong = Convert.ToInt64(matchId);

			var matchDetails = SteamWebAPI.Game().Dota2().IDOTA2Match().GetMatchDetails(matchIdLong).GetResponse();

			return matchDetails;
		}
		#endregion

		#region UserMethods
		[ActionName("LoginUserAction")]
		[AcceptVerbs("GET", "POST")]
		public JObject LoginUser(string email, string password)
		{
			var hashedPassword = HashPassword(email, password);

			using (var conn = new SqlConnection(ConnectionString))
			using (var command = new SqlCommand("dbo.Login", conn)
			{
				CommandType = CommandType.StoredProcedure
			})
			{
				conn.Open();
				command.Parameters.Add(new SqlParameter("@email", email));
				command.Parameters.Add(new SqlParameter("@password", hashedPassword));
				bool success = false;
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						success = (bool) reader["SuccessfulLogin"];
					}
				}
				conn.Close();

				return new JObject {{"success", success}};
			}
		}
        
        [ActionName("RegisterUserAction")]
		[AcceptVerbs("GET","POST")]
		public JObject RegisterUser(string email, string password)
		{
			var hashedPassword = HashPassword(email, password);

	        if (UserExists(email)) return new JObject {{"success", false}, {"message", "This email is already being used."}};
	        using (var conn = new SqlConnection(ConnectionString))
	        using (var command = new SqlCommand("dbo.InsertUser", conn)
	        {
		        CommandType = CommandType.StoredProcedure
	        })
	        {
		        conn.Open();
		        command.Parameters.Add(new SqlParameter("@email", email));
		        command.Parameters.Add(new SqlParameter("@password", hashedPassword));
		        command.ExecuteNonQuery();
		        conn.Close();
	        }
		        
	        return new JObject {{"success", true}};
		}

		private bool UserExists(string email)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			using (SqlCommand command = new SqlCommand("dbo.UserExists", conn)
			{
				CommandType = CommandType.StoredProcedure
			})
			{
				conn.Open();
				command.Parameters.Add(new SqlParameter("@email", email));
				bool userExists = false;
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						userExists = (bool) reader["UserExists"];
					}
				}
				conn.Close();
				return userExists;
			}
		}

	    public void SaveSteamIdForUser(string email, string steamId)
	    {
		    using (var conn = new SqlConnection(ConnectionString))
			using (var command = new SqlCommand("dbo.UpdateSteamId", conn){
				CommandType = CommandType.StoredProcedure
			})
		    {
			    conn.Open();
				command.Parameters.Add(new SqlParameter("@email", email));
				command.Parameters.Add(new SqlParameter("@steamId", steamId));
				command.ExecuteNonQuery();
				conn.Close();
		    }
	    }
		#endregion

		#region UtilMethods
		//not going to worry about salting passwords right now... Fuck security.
		private static string GenerateSaltValue()
		{
			UnicodeEncoding utf16 = new UnicodeEncoding();

			if (utf16 != null)
			{
				Random random = new Random(unchecked ((int)DateTime.Now.Ticks));

				if (random != null)
				{
					byte[] saltValue = new byte[HashSize];

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
		#endregion
	}
}
