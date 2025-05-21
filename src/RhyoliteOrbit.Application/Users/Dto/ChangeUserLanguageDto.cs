using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}