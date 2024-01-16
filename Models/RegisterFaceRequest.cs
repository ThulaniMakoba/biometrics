namespace biometricService.Models
{
    public class RegisterFaceRequest
    {
        public Guid CustomerId { get; set; }
        public Image Image { get; set; }
        public string Assertion { get; set; } = "NONE";
        public Detection Detection { get; set; }
        public int UserId { get; set; }
        public string ComputerSerialNumber { get; set; }
    }
}
