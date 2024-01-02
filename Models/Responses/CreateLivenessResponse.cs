namespace biometricService.Models.Responses
{
    public class CreateLivenessResponse
    {
        public Links links { get; set; }


        public class Links
        {
            public string self { get; set; }
        }

    }
}
