using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Literature_Database.Services
{
    public class GoogleAIModel
    {
        private readonly HttpClient httpClient;
        private readonly string endpoint = "https://us-central1-aiplatform.googleapis.com/v1/projects/cosmic-surface-408212/locations/us-central1/publishers/google/models/gemini-pro:streamGenerateContent";
        private readonly string accessToken = "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";
        public GoogleAIModel()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<string> QueryGoogleAI(string userInput)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "USER",
                        parts = new[] { new { text = userInput } }
                    },
                    // Add additional contents as required
                },
                // Include tools, safetySettings, and generationConfig as needed
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(endpoint, jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
