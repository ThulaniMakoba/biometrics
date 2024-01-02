namespace biometricService.Models
{
    public class CreateReferenceFaceRequest
    {
        public Image image { get; set; } 
        public Detection detection { get; set; }
    }
}
