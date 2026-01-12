using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//Got the connection and request structure from OpenAI's documentation
public static class OpenAIService
{
    private static readonly HttpClient http = new HttpClient();

    public static async Task<string> AskGPT(string prompt)
    {
        string? apiKey = System.Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            return "❌ OPENAI_API_KEY environment variable not set.";

        var requestBody = new
        {
            model = "gpt-4.1-mini",
            input = prompt
        };

        var json = JsonConvert.SerializeObject(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        http.DefaultRequestHeaders.Clear();
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        HttpResponseMessage response;
        try
        {
            response = await http.PostAsync(
                "https://api.openai.com/v1/responses",
                content
            );
        }
        catch (HttpRequestException ex)
        {
            return $"❌ HTTP error: {ex.Message}";
        }

        string responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"❌ OpenAI request failed: {response.StatusCode}\n{responseText}";

        JObject parsed;
        try
        {
            parsed = JObject.Parse(responseText);
        }
        catch
        {
            return "❌ Failed to parse OpenAI response.";
        }

        if (parsed["output"] is not JArray outputArray)
            return "⚠️ OpenAI response missing output.";

        foreach (JToken item in outputArray)
        {
            if (item["content"] is not JArray contentArray)
                continue;

            foreach (JToken contentItem in contentArray)
            {
                if (contentItem["type"]?.ToString() == "output_text")
                {
                    string? text = contentItem["text"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                        return text;
                }
            }
        }

        return "⚠️ No response text received.";
    }
}
