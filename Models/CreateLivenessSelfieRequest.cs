namespace biometricService.Models
{
    public class CreateLivenessSelfieRequest
    {
        public string assertion { get; set; }
        public Image image { get; set; }
    }
}
