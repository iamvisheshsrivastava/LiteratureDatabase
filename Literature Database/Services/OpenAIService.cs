using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Literature_Database.Services
{
    public class OpenAIService
    {
        private readonly string apiKey = "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";
        private readonly HttpClient httpClient = new HttpClient();

        public OpenAIService(string apiKey)
        {
            this.apiKey = apiKey;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> AnalyzeDataAsync(string data)
        {
            // Constructing the prompt to guide the model for a JSON format response
            string prompt = $"Please analyze the following query and provide the response in JSON format: {data}";

            var requestBody = new
            {
                model = "gpt-3.5-turbo", // Use your fine-tuned model if applicable
                messages = new[] { new { role = "system", content = "You are a helpful assistant." },
                           new { role = "user", content = prompt } },
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
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
