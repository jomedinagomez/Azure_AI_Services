using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private const string API_KEY = "YOUR_API_KEY"; // Set your key here
    private const string QUESTION = "Please responde ina json with the first, last name and middle name initial for me. Please use the following full name: Medina Gomez, Jose A"; // Set your question here

    private const string ENDPOINT  = "https://<OpenAIEndpoint>/openai/deployments/gpt4o/chat/completions?api-version=2024-02-15-preview";
    static async Task Main()
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);
            var payload = new
            {
                messages = new object[]
                {
                  new {
                      role = "system",
                      content = new object[] {
                          new {
                              type = "text",
                              text = "Can you extract the first and last name from the following text? ##Important Please provide the output in a json with the keys 'first_name' and 'last_name' and middle_name_initial"
                          }
                      }
                  },
                  new {
                      role = "user",
                      content = new object[] {
                          new {
                              type = "text",
                              text = QUESTION
                          }
                      }
                  }
                },
                temperature = 0.2,
                top_p = 0.95,
                max_tokens = 800,
                stream = false
            };

            var response = await httpClient.PostAsync(ENDPOINT , new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                Console.WriteLine(responseData);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}, {response.ReasonPhrase}");
            }
        }
    }
}