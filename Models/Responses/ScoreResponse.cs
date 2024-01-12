namespace biometricService.Models.Responses
{
    public class ScoreResponse : ErrorMessageModel
    {
        public string Score { get; set; }
        public bool IsSuccess { get; set; }
    }
}
