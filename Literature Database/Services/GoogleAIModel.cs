using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using System.IO;

namespace Literature_Database.Services
{
    public class GoogleAIModel
    {
        private readonly HttpClient httpClient;
        private readonly string endpoint = "https://us-central1-aiplatform.googleapis.com/v1/projects/cosmic-surface-408212/locations/us-central1/publishers/google/models/gemini-pro:streamGenerateContent";

        public GoogleAIModel()
        {
            httpClient = new HttpClient();
        }

        public async Task<string> QueryGoogleAI(string userInput)
        {
            try
            {
                string jsonPath = @"C:\Users\Vishesh Srivastava\Desktop\Delete\Literature Database\cosmic-surface-408212-2a155adc8484.json";
                GoogleCredential credential;

                using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream);
                }

                credential = credential.CreateScoped(new string[] {
                    "https://www.googleapis.com/auth/cloud-platform",
                    "https://www.googleapis.com/auth/aiplatform"
                });

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await credential.UnderlyingCredential.GetAccessTokenForRequestAsync());

                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    role = "USER",
                    parts = new[] { new { text = userInput } }
                },
            },
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(endpoint, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error calling API: {response.StatusCode}. Response: {errorResponse}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw; 
            }
        }
    }
}
