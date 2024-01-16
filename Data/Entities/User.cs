using System.ComponentModel.DataAnnotations;

namespace biometricService.Data.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }        
        [Required]
        public string IdNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public Guid? InnovatricsFaceId { get; set; }
        public string? ComputerMotherboardSerialNumber { get; set; }
        public string? MobileIMEI { get; set; }
        public bool Deleted { get; set; }

        public ICollection<FaceData> FaceData { get; set; }
    }
}
