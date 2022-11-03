using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class EditPOApprover
    {
        [Required]
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
        public decimal AmountPHP { get; set; }
        [Required]
        public string AmountLogicalOperatorUSD { get; set; }
        public decimal AmountUSD { get; set; }
        [Required]
        public string AmountLogicalOperatorEUR { get; set; }
        public decimal AmountEUR { get; set; }
        public IFormFile ImageFile { get; set; }

        [Required]
        public bool ReplaceImg { get; set; }
    }
}
