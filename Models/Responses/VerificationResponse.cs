namespace biometricService.Models.Responses
{
    public class VerificationResponse
    {
        public bool UserExist { get; set; }
        public Guid? ReferenceFaceId { get; set; }
    }
}
