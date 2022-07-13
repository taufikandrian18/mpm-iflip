using System.ComponentModel.DataAnnotations;

namespace MPM.FLP.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}