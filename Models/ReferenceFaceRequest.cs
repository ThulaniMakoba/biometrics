namespace biometricService.Models
{
    public class ReferenceFaceRequest
    {
        public Image image { get; set; }
        public Detection detection { get; set; }
    }
}
