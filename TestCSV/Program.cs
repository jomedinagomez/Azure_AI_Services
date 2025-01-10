using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
 
namespace NameParserApp
{
    class Program
    {
        // Replace with your Azure OpenAI endpoint and API key
        private static readonly string aoaiEndpoint = "https://azureopenaieusjamg.openai.azure.com/";
        private static readonly string aoaiApiKey = "6129cde9669f42e9a255c1e5ba203628";
        private static readonly string deploymentName = "gpt4o"; // e.g., "gpt-35-turbo"
 
        static async Task Main(string[] args)
        {
            string csvFilePath = "C:/Users/jomedin/Documents/Azure_AI_Services/NER/data/processed/full_names.csv"; // Path to your CSV file
 
            // Read the CSV file and get the full names
            List<string> fullNames = ReadFullNamesFromCsv(csvFilePath);
 
            // Process each full name
            foreach (var fullName in fullNames)
            {
                var nameParts = await ParseNameWithAOAI(fullName);
 
                if (nameParts != null)
                {
                    Console.WriteLine($"Full Name: {fullName}");
                    Console.WriteLine($"First Name: {nameParts.FirstName}");
                    Console.WriteLine($"Middle Name: {nameParts.MiddleName}");
                    Console.WriteLine($"Last Name: {nameParts.LastName}");
                    Console.WriteLine(new string('-', 40));
                }
            }
        }
 
        // Method to read full names from a CSV file
        static List<string> ReadFullNamesFromCsv(string filePath)
        {
            var fullNames = new List<string>();
 
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
 
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    var fullName = csv.GetField("full_name");
                    if (!string.IsNullOrEmpty(fullName))
                    {
                        fullNames.Add(fullName);
                    }
                }
            }
 
            return fullNames;
        }
 
        // Method to call Azure OpenAI API and parse the name
        static async Task<NameParts> ParseNameWithAOAI(string fullName)
        {
            using (var httpClient = new HttpClient())
            {
                // Set the request URL
                string requestUrl = $"{aoaiEndpoint}openai/deployments/{deploymentName}/chat/completions?api-version=2023-03-15-preview";
 
                // Set the API key in the header
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", aoaiApiKey);
 
                // Prepare the request payload
                var requestBody = new
                {
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = $"Extract the first, middle, and last names from the following full name: \"{fullName}\". Provide the output in JSON format with keys 'FirstName', 'MiddleName', and 'LastName'. If a part is missing, set its value to null."
                        }
                    },
                    temperature = 0.0,
                    max_tokens = 50
                };
 
                try
                {
                    // Send the POST request
                    var response = await httpClient.PostAsJsonAsync(requestUrl, requestBody);
 
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JObject.Parse(responseContent);
                        var content = jsonResponse["choices"][0]["message"]["content"].ToString();
 
                        // Parse the JSON content returned by AOAI
                        var nameParts = JObject.Parse(content).ToObject<NameParts>();
                        return nameParts;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorContent);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
 
            return null;
        }
    }
 
    // Class to hold the name parts
    public class NameParts
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}