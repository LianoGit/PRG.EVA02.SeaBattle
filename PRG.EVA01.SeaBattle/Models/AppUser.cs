using System.ComponentModel.DataAnnotations;

namespace PRG.EVA01.SeaBattle.Models
{
    public class AppUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string NormalizedEmail { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string Role { get; set; } = "Player";
    }
}
