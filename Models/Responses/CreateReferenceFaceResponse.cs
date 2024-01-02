namespace biometricService.Models.Responses
{
    public class CreateReferenceFaceResponse : ErrorMessageModel
    {
        public Guid id { get; set; }
        public Detection detection { get; set; }
        public Links links { get; set; }
    }


    public class Detection
    {
        public float confidence { get; set; }
        public Facerectangle faceRectangle { get; set; }
    }

    public class Facerectangle
    {
        public Topleft topLeft { get; set; }
        public Topright topRight { get; set; }
        public Bottomright bottomRight { get; set; }
        public Bottomleft bottomLeft { get; set; }
    }

    public class Topleft
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Topright
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Bottomright
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Bottomleft
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    //public class Links
    //{
    //    public string self { get; set; }
    //}
}