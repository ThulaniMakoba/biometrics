namespace biometricService.Models
{
    public class VerificationRequest
    {
        public string ComputerSid { get; set; }
        public Guid WindowsProfileId { get; set; }
    }
}
