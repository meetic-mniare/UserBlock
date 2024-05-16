namespace UserBlock.Application.Interfaces;

public interface ILocalizationApiClient
{
    Task<string> TranslateAsync(string message, string language);
}
