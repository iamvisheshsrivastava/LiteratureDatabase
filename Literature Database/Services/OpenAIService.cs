using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.AIPlatform.V1.Schema.TrainingJob.Definition.AutoMlImageClassificationInputs.Types;

namespace Literature_Database.Services
{
    public class OpenAIService
    {
        private readonly string apiKey = Environment.GetEnvironmentVariable("OpenAIApiKey");
        private readonly HttpClient httpClient = new HttpClient();

        public OpenAIService(string apiKey)
        {
            this.apiKey = apiKey;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> AnalyzeDataAsync(string data, string modelType)
        {
            var requestBody = new
            {
                model = modelType,
                //response_format = new { type = "json_object" },
                messages = new[]
                {
                    new { role = "system", content = "You are an AI knowledgeable in sustainability. Provide detailed, accurate, and factual information." },
                    new { role = "user", content = data }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content); // Using chat completions endpoint
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        public async Task<string> AskModelAsync(string data)
        {
            var requestBody = new
            {
                model = "ft:davinci-002:personal::8WCHpHkC",
                prompt = data,
                max_tokens = 150, // Adjust as needed
                temperature = 0.7, // Adjust for creativity/variation
                n = 1, // Number of completions to generate
                stop = (string)null
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.openai.com/v1/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
