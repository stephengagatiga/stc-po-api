using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class NewAuditTrail
    {
        [Required]
        public int POPendingId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
