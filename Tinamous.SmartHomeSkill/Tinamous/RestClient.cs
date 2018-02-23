using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tinamous.SmartHome.Tinamous
{
    public class RestClient : ITinamousRestClient
    {
        /// <summary>
        /// Allow upto 30 second by default (EasyNetQ will usually fail at that point).
        /// </summary>
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
        private readonly Uri _siteUri = new Uri("https://api.tinamous.com");
        //private readonly Uri _siteUri = new Uri("http://dev.localhost:20866");

        public async Task Post<T>(string token, string apiUri, T t)
        {
            var uri = new Uri(_siteUri, apiUri);

            var handler = new HttpClientHandler { PreAuthenticate = true };
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Authorization = CreateBearerHeader(token);

                // Doesn't work. #WTF!
                //HttpResponseMessage response = await client.PostAsJsonAsync(uri, t);

                string json = JsonConvert.SerializeObject(t);
                var response = await client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    //Debug.WriteLine("Error posting data.");
                    //Debug.WriteLine("StatusCode:" + response.StatusCode);
                    throw new Exception("REST Post failed. Status code: " + response.StatusCode);
                }
            }
        }

        public async Task<T> GetAsJsonAsync<T>(string token, string apiUri)
        {
            var uri = new Uri(_siteUri, apiUri);

            using (var client = new HttpClient())
            {
                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Authorization = CreateBearerHeader(token);
                client.Timeout = _timeout;

                var response = await client.GetAsync(uri);

                if (!response.IsSuccessStatusCode)
                {
                    //Trace.WriteLine("HTTP GET Error getting data.");
                    //Trace.WriteLine("StatusCode:" + response.StatusCode);
                    throw new Exception("HTTP GET as json failed. Status code: " + response.StatusCode);
                }

                string s = response.Content.ReadAsStringAsync().Result;

                return await response.Content.ReadAsAsync<T>();
            }
        }

        private static AuthenticationHeaderValue CreateBearerHeader(string token)
        {
            return new AuthenticationHeaderValue("bearer", token);
        }
    }

}