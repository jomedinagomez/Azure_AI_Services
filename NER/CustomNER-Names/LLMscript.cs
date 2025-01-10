using System;  
using System.Collections.Generic;  
using System.Globalization;  
using System.IO;  
using System.Net.Http;  
using System.Net.Http.Json;  
using System.Threading.Tasks;  
using CsvHelper;  
using CsvHelper.Configuration;  
  
public class Program  
{  
    private static readonly HttpClient httpClient = new HttpClient();  
  
    public static async Task Main(string[] args)  
    {  
        string filePath = "path/to/your/file.csv";  
        var fullNames = ReadFullNamesFromCsv(filePath);  
  
        foreach (var fullName in fullNames)  
        {  
            var nameParts = await GetNamePartsFromAOAI(fullName);  
            Console.WriteLine($"Full Name: {fullName}");  
            Console.WriteLine($"First Name: {nameParts.FirstName}");  
            Console.WriteLine($"Middle Name: {nameParts.MiddleName}");  
            Console.WriteLine($"Last Name: {nameParts.LastName}");  
            Console.WriteLine();  
        }  
    }  
  
    public static List<string> ReadFullNamesFromCsv(string filePath)  
    {  
        using var reader = new StreamReader(filePath);  
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));  
        var records = new List<string>();  
  
        while (csv.Read())  
        {  
            var fullName = csv.GetField("full_name");  
            records.Add(fullName);  
        }  
  
        return records;  
    }  
  
    public static async Task<NameParts> GetNamePartsFromAOAI(string fullName)  
    {  
        var endpoint = "https://azureopenaieusjamg.openai.azure.com/openai/deployments/gpt4o/chat/completions?api-version=2024-02-15-preview";  
        var apiKey = "6129cde9669f42e9a255c1e5ba203628";  
  
        var requestBody = new  
        {  
            prompt = $"Extract first, middle, and last names from the full name: {fullName}",  
            max_tokens = 50  
        };  
  
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);  
  
        var response = await httpClient.PostAsJsonAsync(endpoint, requestBody);  
        response.EnsureSuccessStatusCode();  
  
        var completionResponse = await response.Content.ReadFromJsonAsync<CompletionResponse>();  
        return ParseNameParts(completionResponse.Choices[0].Text);  
    }  
  
    public static NameParts ParseNameParts(string text)  
    {  
        // Implement parsing logic based on the response structure  
        // This example assumes a simple format "First: X, Middle: Y, Last: Z"  
        var parts = text.Split(',');  
        return new NameParts  
        {  
            FirstName = parts[0].Split(':')[1].Trim(),  
            MiddleName = parts[1].Split(':')[1].Trim(),  
            LastName = parts[2].Split(':')[1].Trim()  
        };  
    }  
}  
  
public class NameParts  
{  
    public string FirstName { get; set; }  
    public string MiddleName { get; set; }  
    public string LastName { get; set; }  
}  
  
public class CompletionResponse  
{  
    public List<Choice> Choices { get; set; }  
}  
  
public class Choice  
{  
    public string Text { get; set; }  
}  