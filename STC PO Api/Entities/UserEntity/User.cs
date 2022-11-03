using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.UserEntity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ObjectId { get; set; }

        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
