using biometricService.Models.Responses;

namespace biometricService.Models
{
    public class UserModel : ErrorMessageModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsSuccess { get; set; }
    }
}
