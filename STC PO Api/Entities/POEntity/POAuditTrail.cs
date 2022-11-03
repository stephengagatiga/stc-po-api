using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POEntity
{
    public class POAuditTrail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int POPendingId { get; set; }
        [ForeignKey("POPendingId")]
        public POPending POPending { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime DateAdded { get; set; }

    }
}
