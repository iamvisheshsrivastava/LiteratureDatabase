using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Literature_Database.Services
{
    public class OpenAIModel
    {
        private readonly string apiKey = "sk-";
        private readonly HttpClient httpClient = new HttpClient();

        public OpenAIModel(string apiKey)
        {
            this.apiKey = apiKey;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> AnalyzeDataAsync(string data)
        {
            var requestBody = new
            {
                model = "your_fine_tuned_model_name",
                messages = new[] { new { role = "user", content = "Please provide a brief summary: " + data } },
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content); // Using chat completions endpoint
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        //async Task<string> UploadFile(HttpClient client, string folder, string dataset, string purpose)
        //{
        //    var file = Path.Combine(folder, dataset);
        //    using var fs = File.OpenRead(file);
        //    StreamContent fileContent = new(fs);
        //    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = dataset };

        //    using MultipartFormDataContent formData = new();
        //    formData.Add(new StringContent(purpose), "purpose");
        //    formData.Add(fileContent);

        //    var response = await client.PostAsync("openai/files?api-version=2023-10-01-preview", formData);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var data = await response.Content.ReadFromJsonAsync<FileUploadResponse>();
        //        return data.id;
        //    }

        //    return string.Empty;
        //}

        //async Task<string> SubmitTrainingJob(HttpClient client, string trainingFileId, string validationFileId)
        //{
        //    TrainingRequestModel trainingRequestModel = new()
        //    {
        //        model = "gpt-3.5-turbo-0613",
        //        training_file = trainingFileId,
        //        validation_file = validationFileId,
        //    };

        //    var requestBody = JsonSerializer.Serialize(trainingRequestModel);
        //    StringContent content = new(requestBody, Encoding.UTF8, "application/json");

        //    var response = await client.PostAsync("openai/fine_tuning/jobs?api-version=2023-10-01-preview", content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var data = await response.Content.ReadFromJsonAsync<TrainingResponseModel>();
        //        return data.id;
        //    }

        //    return string.Empty;
        //}
    }
}
