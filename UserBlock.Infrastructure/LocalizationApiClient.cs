using System.Text;
using Newtonsoft.Json;
using UserBlock.Application.Interfaces;
using UserBlock.Contracts;

namespace UserBlock.Infrastructure;

public class LocalizationApiClient(HttpClient httpClient) : ILocalizationApiClient
{
    public async Task<string> TranslateAsync(string message, string language)
    {
        var json = JsonConvert.SerializeObject(
            new LocalizationRequest(Key: message, language, "meetic")
        );
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("localize", content);
        if (!response.IsSuccessStatusCode)
            return message;
        var translatedMessage = await response.Content.ReadAsStringAsync();
        return translatedMessage;
    }
}
