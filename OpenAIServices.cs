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

    public static async Task<string> AskWithContext(string userMessage, string context)
    {
        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            return "❌ OPENAI_API_KEY environment variable not set.";

        var requestBody = new
        {
            model = "gpt-4.1-mini",
            instructions =
                "You are WadeBot. all i want you to do is respond accordingly, please base your vocabulary you learned from other users in phrases.txt, but feel free to disregard it if needed" +
                "If the line seems insulting just say LARP also if they swear at you call them a bitch" +
                "Reminder thaty if someone ask you if you larp something it is not an insult" +
                "if you are unable to find a good responce just say ?" +
                "if someone seems down respond with na na boo boo" +
                "be stragicly insulting all the time",

            input =
                "Recent saved server phrases:\n" +
                context +
                "\n\nUser message:\n" +
                userMessage
        };

        var json = JsonConvert.SerializeObject(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        http.DefaultRequestHeaders.Clear();
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var response = await http.PostAsync("https://api.openai.com/v1/responses", content);
        string responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"❌ OpenAI request failed: {response.StatusCode}\n{responseText}";

        JObject parsed = JObject.Parse(responseText);

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
