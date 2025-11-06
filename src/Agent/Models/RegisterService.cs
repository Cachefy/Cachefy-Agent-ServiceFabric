using System.ComponentModel.DataAnnotations;

namespace Agent.Models
{
    public class RegisterService
    {
        [Required]
        public string Name { get; set; } = null!;

        public string Status { get; set; }

        public string Version { get; set; }
    }
}