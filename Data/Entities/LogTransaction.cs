using System.ComponentModel.DataAnnotations;

namespace biometricService.Data.Entities
{
    public class LogTransaction
    {
        [Key]
        public int Id { get; set; }     
        public DateTime TimeStamp { get; set; }
        public string TransactionDetails { get; set; }

    }
}
