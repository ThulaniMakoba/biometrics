namespace biometricService.Models
{
    public class CreateReferenceFaceRequest
    {
        public int UserId { get; set; }
        public Image image { get; set; } 
        public Detection detection { get; set; }
    }
}
