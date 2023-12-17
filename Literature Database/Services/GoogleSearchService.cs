using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Literature_Database.Services
{
    public class GoogleSearchService
    {
        private readonly string apiKey = "AIzaSyD8dIx-";
        private readonly string searchEngineId;
        private readonly string endpoint = "https://www.googleapis.com/customsearch/v1";

        public GoogleSearchService(string apiKey, string searchEngineId)
        {
            this.apiKey = apiKey;
            this.searchEngineId = searchEngineId;
        }

        public async Task<string[]> SearchWebAsync(string query)
        {
            var httpClient = new HttpClient();
            var requestUri = $"{endpoint}?key={apiKey}&cx={searchEngineId}&q={Uri.EscapeDataString(query)}";

            var response = await httpClient.GetAsync(requestUri);
            var contentString = await response.Content.ReadAsStringAsync();

            dynamic jsonResponse = JsonConvert.DeserializeObject(contentString);

            // Check if items is not null
            if (jsonResponse.items == null)
            {
                return new string[0];
            }

            var items = jsonResponse.items;
            var urls = new string[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                urls[i] = items[i].link;
            }
            return urls;
        }
    }
}
