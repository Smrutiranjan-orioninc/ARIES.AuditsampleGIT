using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
    }
}
