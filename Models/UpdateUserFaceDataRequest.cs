namespace biometricService.Models
{
    public class UpdateUserFaceDataRequest
    {
        public int UserId { get; set; }
        public Guid FaceReferenceId { get; set; }
        public string FaceImageBase64 { get; set; }
    }
}
