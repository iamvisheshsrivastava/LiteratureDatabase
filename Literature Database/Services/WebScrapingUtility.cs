using System.Text.RegularExpressions;
using System;
using System.Linq;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace Literature_Database.Services
{
    public class WebScrapingUtility
    {
        public async Task<string> ScrapeWebPageAsync(string url)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "MyScraperBot/1.0");

            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // Handle Forbidden (403) error
                return "Access Forbidden";
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Handle Not Found (404) error
                return "Page Not Found";
            }
            catch (Exception e)
            {
                // Handle other exceptions
                return $"An error occurred: {e.Message}";
            }
        }
        public string CleanScrapedContent(string content)
        {
            // Remove HTML Tags using HtmlAgilityPack
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var textOnly = doc.DocumentNode.InnerText;

            // Remove special characters
            textOnly = Regex.Replace(textOnly, "[^a-zA-Z0-9 .,;!?]", String.Empty);

            // Trimming leading and trailing whitespace
            textOnly = textOnly.Trim();

            // Filter Irrelevant Sections
            textOnly = FilterIrrelevantSections(textOnly, new string[] { "advertisement", "footer", "related articles" });

            // Summarization
            var summary = SummarizeText(textOnly, 5);

            return summary;
        }

        private string FilterIrrelevantSections(string text, string[] filterKeywords)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var filteredLines = lines.Where(line => !filterKeywords.Any(keyword => line.Contains(keyword, StringComparison.OrdinalIgnoreCase)));

            return string.Join("\n", filteredLines);
        }

        public string SummarizeText(string text, int numberOfSentences)
        {
            var wordFrequency = CalculateWordFrequency(text);
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+");

            var sentenceScores = new Dictionary<string, int>();
            foreach (var sentence in sentences)
            {
                int sentenceScore = 0;
                var words = sentence.Split(' ');
                foreach (var word in words)
                {
                    if (wordFrequency.ContainsKey(word))
                    {
                        sentenceScore += wordFrequency[word];
                    }
                }

                // Check if the sentence is already in the dictionary
                if (!sentenceScores.ContainsKey(sentence))
                {
                    sentenceScores.Add(sentence, sentenceScore);
                }
                else
                {
                    // Update the score if the sentence is already present
                    sentenceScores[sentence] += sentenceScore;
                }
            }

            var summarizedText = sentenceScores.OrderByDescending(kvp => kvp.Value)
                                                .Take(numberOfSentences)
                                                .Select(kvp => kvp.Key);

            return string.Join(" ", summarizedText);
        }

        private Dictionary<string, int> CalculateWordFrequency(string text)
        {
            var frequency = new Dictionary<string, int>();
            var words = text.Split(' ');

            foreach (var word in words)
            {
                if (!frequency.ContainsKey(word))
                    frequency[word] = 0;

                frequency[word]++;
            }

            return frequency;
        }
    }
}