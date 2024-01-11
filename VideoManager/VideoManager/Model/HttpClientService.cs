using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VideoManager.Model
{
    class HttpClientService
    {
        private static readonly HttpClientService instance = new HttpClientService();
        private readonly HttpClient httpClient;

        private HttpClientService()
        {
            httpClient = new HttpClient();
        }

        public static HttpClientService Instance
        {
            get { return instance; }
        }

        public async Task<string> Get(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
