﻿using System.ComponentModel.DataAnnotations;

namespace biometricService.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string InnovatricsId { get; set; }
    }
}
