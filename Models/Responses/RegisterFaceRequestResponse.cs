namespace biometricService.Models.Responses
{
    public class RegisterFaceRequestResponse: ErrorMessageModel
    {
        public Guid Id { get; set; }
        public string Base64Image { get; set; }
    }
}
