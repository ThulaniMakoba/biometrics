namespace biometricService.Models
{
    public class CreateCustomerResponse
    {
        public string Id { get; set; }
        public Links Links { get; set; }
    }
    public class Links
    {
        public string Self { get; set; }
    }
}
