using System.ComponentModel.DataAnnotations;

namespace biometricService.Data.Entities
{
    public class ConfigSetting
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string IdNumber { get; set; } = string.Empty;
        [Required]
        public string ComputerUniqueNumber { get; set; } = string.Empty;
        public string? ComputerName { get; set; }
    }
}
