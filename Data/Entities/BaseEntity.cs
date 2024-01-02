namespace biometricService.Data.Entities
{
    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? TransactionBy { get; set; }
    }
}
