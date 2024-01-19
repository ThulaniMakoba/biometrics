namespace biometricService.Models.Responses
{
    public class RegisterUserResponse
    {
        public int UserId { get; set; }
        public int EdnaId { get; set; }
        public string? Message { get; set; }
    }
}
