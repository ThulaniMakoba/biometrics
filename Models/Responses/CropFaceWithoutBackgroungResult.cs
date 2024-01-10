namespace biometricService.Models.Responses
{
    public class CropFaceWithoutBackgroungResult : ErrorMessageModel
    {
        public Guid Id { get; set; }
        public string Base64Image { get; set; }
    }
}
