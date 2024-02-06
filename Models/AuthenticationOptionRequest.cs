namespace biometricService.Models
{
    public class AuthenticationOptionRequest
    {
        public int? EdnaId { get; set; }
        public string? IdNumber { get; set; }
        public string? Email { get; set; }
    }
}
