using Newtonsoft.Json;
using SimulationsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _3DX
{
    class OauthHelper
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseUrl { get; set; }
        public string Scope { get; set; }

        private string AccessToken { get; set; }
		private DateTime AccessTokenTimeStamp { get; set; }
		/// <summary>
		///  Helper class to get OAuth2 access token.
		/// </summary>
		public OauthHelper()
        {
	        AccessToken = "";
	        AccessTokenTimeStamp = DateTime.MinValue;

        }

        /// <summary>
        /// Helper class to get OAuth2 access token.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="baseUrl"></param>
        public OauthHelper(string clientId, string clientSecret, string baseAddress)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            BaseUrl = baseAddress;
        }

        /// <summary>
        /// Send a GET request. Access token is taken care of.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T> SendHttpGetAsync<T>(string url)
        {
            AccessToken = await GetAccessToken();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Add the authorization header with the access token.
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);

                // HTTP GET request to the server and parse the response.
                HttpResponseMessage response = await client.GetAsync(url);

                string jsonString = await response.Content.ReadAsStringAsync();
                T responseData = JsonConvert.DeserializeObject<T>(jsonString);

                return responseData;
            }
        }

        /// <summary>
        /// Send a POST request. Access token is taken care of.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<T> SendHttpPostAsync<T>(string url, Object data)
        {
            string jsonString = "";
            T responseData = default(T);
	        AccessToken = await GetAccessToken();

            

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Add the authorization header with the access token.
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);

                // Add the data for the POST request.
                var jsonContent = JsonConvert.SerializeObject(data);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var content = new ByteArrayContent(buffer);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                // HTTP POST request to the Server and parse the response.
                HttpResponseMessage response = await client.PostAsync(url, content);
                if (response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
                {
                    Helper.ToConsole("3DX:Gateway returned (GatewayTimeout).");
                    throw new NetworkInformationException(Convert.ToInt32(response.StatusCode));
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    jsonString = await response.Content.ReadAsStringAsync();
                    responseData = JsonConvert.DeserializeObject<T>(jsonString);
                    
                }
                else
                {
	                Helper.ToConsole("3DX:Gateway returned (unknown error).");
	                throw new NetworkInformationException(Convert.ToInt32(response.StatusCode)); 
                }
            }
            return responseData;
        }

        private async Task<string> GetAccessToken()
        {
            if (string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(ClientSecret) || string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new Exception("At least one of the required properties; ClientId, ClientSecret, BaseUrl, is null.");
            }

	        if ((AccessTokenTimeStamp == DateTime.MinValue) || // Ever been used?
	            ((DateTime.Now - AccessTokenTimeStamp).Hours > 0) || // Token valid for an hour, so hours must be 0?
	            (AccessToken.Length == 0)) // Token is empty?
	        {
		        AccessTokenTimeStamp = DateTime.Now;

		        using (var client = new HttpClient())
		        {
			        client.BaseAddress = new Uri(BaseUrl);

			        // Set response to JSON
			        client.DefaultRequestHeaders.Accept.Clear();
			        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			        // Add the data for the POST request.
			        List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
			        postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
			        postData.Add(new KeyValuePair<string, string>("client_id", ClientId));
			        postData.Add(new KeyValuePair<string, string>("client_secret", ClientSecret));
			        postData.Add(new KeyValuePair<string, string>("scope", Scope));

			        FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

			        // Post to the Server and parse the response.
			        HttpResponseMessage response = await client.PostAsync("token", content);
			        string jsonString = await response.Content.ReadAsStringAsync();
			        dynamic responseData = JsonConvert.DeserializeObject(jsonString);

					// Return the Access Token.
					AccessToken = ((dynamic) responseData).access_token;
		        }
	        }

	        return AccessToken;

        }
    }
}