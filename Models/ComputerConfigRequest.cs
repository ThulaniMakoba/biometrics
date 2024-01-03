namespace biometricService.Models
{
    public class ComputerConfigRequest
    {
        public string IdNumber {  get; set; } = string.Empty;
        public string? ComputerName { get; set;}
        public string ComputerSerialNumber { get; set; } = string.Empty;
    }
}
