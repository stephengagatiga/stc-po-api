using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POApproverEntity
{
    public class POApprover
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The job title is required")]
        public string JobTitle { get; set; }
        [Required]
        public string AmountLogicalOperatorPHP { get; set; }
        [Range(minimum: -1_000_000_000, maximum: 1_000_000_000)]
        public decimal AmountPHP { get; set; }
        [Required]
        public string AmountLogicalOperatorUSD { get; set; }
        [Range(minimum: -1_000_000_000, maximum: 1_000_000_000)]
        public decimal AmountUSD { get; set; }
        [Required]
        public string AmountLogicalOperatorEUR { get; set; }
        [Range(minimum: -1_000_000_000, maximum: 1_000_000_000)]
        public decimal AmountEUR { get; set; }
        [Required]
        public byte[] SignatureImg { get; set; }

    }
}
