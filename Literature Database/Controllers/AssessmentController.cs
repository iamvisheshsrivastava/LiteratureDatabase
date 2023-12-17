using Microsoft.AspNetCore.Mvc;
using Literature_Database.Models;
using Literature_Database.Services;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Google.Cloud.AIPlatform.V1;

namespace Literature_Database.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly GoogleSearchService _googleSearchService;
        private readonly OpenAIService _openAIService;

        public AssessmentController(GoogleSearchService googleSearchService, OpenAIService openAIService)
        {
            _googleSearchService = googleSearchService;
            _openAIService = openAIService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Results(AssessmentData results)
        {
            return View(results);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAssessment(AssessmentData data)
        {
            var results = new AssessmentData();
            string searchQuery = $"Company's Industry: {data.Industry}, Number of Employees: {data.Size}, Sustainability Awareness among Employees: {data.SustainabilityAwareness}, Waste Management Practices: {data.WasteManagement}, Eco-Friendly Transportation Promotion: {data.Mobility}, Employee Training and Development: {data.TrainingDevelopment}";

            //// Call the GoogleSearchService to perform the search
            //var urls = await _googleSearchService.SearchWebAsync(searchQuery);

            //// Inject WebScrapingUtility via the constructor or method injection
            //var webScraper = new WebScrapingUtility();

            //List<string> scrapedData = new List<string>();
            //foreach (var url in urls)
            //{
            //    var content = await webScraper.ScrapeWebPageAsync(url);
            //    var cleanedContent = webScraper.CleanScrapedContent(content);
            //    scrapedData.Add(cleanedContent);
            //}

            //string combinedData = string.Join(Environment.NewLine, scrapedData);
            //string trimmedData = TrimToTokenLimit(combinedData, 3500);

            string aiResponse = "";
            if (data.AIModel == "GoogleAI")
            {
                var googleAIModel = new GoogleAIModel();
                aiResponse = await googleAIModel.QueryGoogleAI(searchQuery);
            }

            else if (data.AIModel == "OpenAI")
            {
                aiResponse = await _openAIService.AskModelAsync(
                    "Given the following sustainability self-assessment for a company: "
                    + searchQuery
                    + ". Please provide a structured analysis that includes: "
                    + "1) Key strengths, "
                    + "2) Areas for improvement, "
                    + "3) Recommended actions for each area of sustainability, "
                    + "4) A sustainability score out of 10, "
                    + "5) Pressure of action rating (1-10 or categorized as low, medium, high) considering factors like company size, industry, etc."
                );

                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(aiResponse);
                var content = jsonResponse.choices[0].text.ToString();

                // Set the properties of the results object
                results.KeyStrengths = content;
                results.AreasForImprovement = "";
                results.RecommendedActions = "";
                results.SustainabilityScore = 4;
                results.PressureOfAction = "Medium";
            }

            else if (data.AIModel == "gpt-4-1106-preview" || data.AIModel == "gpt-3.5-turbo" || data.AIModel == "text-curie-001")
            {
                var response = await _openAIService.AnalyzeDataAsync(
                    "Given the following sustainability self-assessment for a company: "
                    + searchQuery 
                    + ". Please provide a structured analysis that includes: "
                    + "1) Key strengths, "
                    + "2) Areas for improvement, "
                    + "3) Recommended actions for each area of sustainability, "
                    + "4) A sustainability score out of 10, "
                    + "5) Pressure of action rating (1-10 or categorized as low, medium, high) considering factors like company size, industry, etc."
                    ,data.AIModel);

                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response);
                var content = jsonResponse.choices[0].message.content.ToString();

                results.KeyStrengths = ExtractSection(content, "1) Key strengths:");
                results.AreasForImprovement = ExtractSection(content, "2) Areas for improvement:");
                results.RecommendedActions = ExtractSection(content, "3) Recommended actions for each area of sustainability:");
                results.SustainabilityScore = ExtractSustainabilityScore(content);
                results.PressureOfAction = ExtractPressureOfAction(content);
            }
            return RedirectToAction("Results", results);
        }

        private string ExtractSection(string content, string sectionTitle)
        {
            var pattern = $"{Regex.Escape(sectionTitle)}(.*?)(?=\\n\\d\\) |\\n\\n|$)";
            var match = Regex.Match(content, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var sectionContent = match.Groups[1].Value.Trim();
                return sectionContent.TrimEnd('\n').Trim();
            }

            return "Section not found";
        }

        private int ExtractSustainabilityScore(string content)
        {
            // Pattern to match '7.5 out of 10' or '7 out of 10'
            var scorePattern = @"(\d+(\.\d+)?) out of 10";
            var match = Regex.Match(content, scorePattern);

            if (match.Success)
            {
                double score = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                return (int)Math.Round(score);
            }

            return 0;
        }
        private string ExtractPressureOfAction(string content)
        {
            var pattern = @"5\) Pressure of action rating:.*?(?=\n\d\)|\n\n|$)";
            var match = Regex.Match(content, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var startIndex = match.Index + match.Value.IndexOf(':') + 1;
                var extractedText = content.Substring(startIndex).Trim();
                return extractedText;
            }

            return "Pressure of action rating not found";
        }

        private static string TrimToTokenLimit(string data, int tokenLimit)
        {
            var tokens = Regex.Split(data, @"[\s\r\n]+").Where(t => !string.IsNullOrEmpty(t)).ToList();
            return string.Join(" ", tokens.Take(tokenLimit));
        }
    }
}
