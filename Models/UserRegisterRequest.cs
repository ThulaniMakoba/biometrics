namespace biometricService.Models
{
    public class UserRegisterRequest
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string IdNumber { get; set; }
        public string ComputerMotherSerialNumber { get; set; }
      
    }
}
