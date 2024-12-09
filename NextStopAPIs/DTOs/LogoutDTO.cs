using System.ComponentModel.DataAnnotations;

namespace NextStopAPIs.DTOs
{
    public class LogoutDTO
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
