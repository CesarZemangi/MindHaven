using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindhaven.Service
{
    public class OpenAiChatService
    {
        private readonly string apiKey= Environment.GetEnvironmentVariable("OpenAI_ApiKey");
        private readonly string apiUrl = "https://api.openai.com/v1/chat/completions";

        public async Task<string> GetResponseAsync(string userMessage)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo", // safer default model
                    messages = new[]
                    {
                new { role = "system", content = "You are MindHaven, a compassionate mental health chatbot. Respond kindly and empathetically." },
                new { role = "user", content = userMessage }
            }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Parse safely
                var j = JObject.Parse(responseString);

                // If there's an error object, return it directly
                if (j["error"] != null)
                {
                    return $"⚠️ Error: {j["error"]?["message"]}";
                }

                var reply = j["choices"]?[0]?["message"]?["content"]?.ToString();
                return reply ?? "⚠️ Sorry, I couldn’t understand the response.";
            }
        }
    }
}
