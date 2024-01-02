namespace biometricService.Models
{
    public class ProbeFaceRequest : CreateReferenceFaceRequest
    {
        public Guid ReferenceFaceId { get; set; }
    }
}
