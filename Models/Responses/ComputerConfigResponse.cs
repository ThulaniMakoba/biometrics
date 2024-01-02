namespace biometricService.Models.Responses
{
    public class ComputerConfigResponse
    {
        public string ComputerSidNumber { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
    }
}