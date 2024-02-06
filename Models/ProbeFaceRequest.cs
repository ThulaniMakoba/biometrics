namespace biometricService.Models
{
    public class ProbeFaceRequest : CreateReferenceFaceRequest
    {
        public int? EdnaId { get; set; }
        public string? IdNumber { get; set; }
        public string? Email { get; set; }
    }
}
