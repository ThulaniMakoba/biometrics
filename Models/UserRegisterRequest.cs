namespace biometricService.Models
{
    public class UserRegisterRequest
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid InnovatricsFaceId { get; set; }
        public Guid WindowsProfileId { get; set; }
        public string ComputerSID { get; set; }
        public string Base64Image { get; set;}
    }
}
