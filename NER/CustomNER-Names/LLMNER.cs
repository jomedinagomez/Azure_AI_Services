using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private const string API_KEY = "YOUR_API_KEY"; // Set your key here
    private const string FULL_NAME = "Medina Gomez, Jose A Microsoft Corportation"; // Set your question here

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
                              text = "Can you extract the first and last name from the following text? \n ##Important \n\b** Please provide the output in a json with the keys 'first_name' and, 'suffix', 'middle_initial', 'last_name', 'entity_type': 'LOC','PER','ORG','TIME' if you identify that the entity type is anything other than a person, please return the field names as an empty string.\n\b** If you have multiple name, please only return the information for the first name and ignore the others.\n\b\b** Example: CRISP, CATHERINE,  MICHAEL, RACHEL, SARAH. Output should be: First name = CATHERINE, Last name = CRISP \n Please use the following full name:"
                          }
                      }
                  },
                  new {
                      role = "user",
                      content = new object[] {
                          new {
                              type = "text",
                              text = FULL_NAME
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