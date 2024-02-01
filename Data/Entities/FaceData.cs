using System.ComponentModel.DataAnnotations;

namespace biometricService.Data.Entities
{
    public class FaceData : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Guid? FaceReferenceId { get; set; }
        public string FaceBase64 { get; set; }
    }
}
