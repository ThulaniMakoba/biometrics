namespace biometricService.Models
{
    public class EnrolmentRequest
    {
        public EnrolmentModel Data { get; set; }
        public byte[] Content { get; set; }
    }

    public class EnrolmentModel
    {
        public DataModel Data { get; set; }
        public ImageModel Image { get; set; }
    }

    public class DataModel
    {
        public DetectionModel Detection { get; set; }
        public ImageResolutionModel ImageResolution { get; set; }
    }
    public class DetectionModel
    {
        public double Confidence { get; set; }
        public XYAxisModel TopLeft { get; set; }
        public XYAxisModel BottomRight { get; set; }
        public XYAxisModel FaceCenter { get; set; }
        public double FaceSize { get; set; }
        public double Brightness { get; set; }
        public double Sharpness { get; set; }

    }

    public class XYAxisModel
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class ImageResolutionModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class ImageModel
    {
        public int Size { get; set; }
        public string? Type { get; set; }

    }

}
