using System.ComponentModel.DataAnnotations;

namespace UserBlock.Contracts;

public record LocalizationRequest(
    [Required(ErrorMessage = "Key is required")] string Key,
    [Required(ErrorMessage = "CultureCode is required")]
    [RegularExpression(
        "^[a-zA-Z]{2}-[a-zA-Z]{2}$",
        ErrorMessage = "CultureCode must be in the format 'xx-XX'"
    )]
        string CultureCode,
    string? SiteCode
);
