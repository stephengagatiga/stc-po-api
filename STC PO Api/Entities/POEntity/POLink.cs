using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POEntity
{
    public class POLink
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int POPendingId { get; set; }
        [ForeignKey("POPendingId")]
        //Prevent Self refencing loop
        [IgnoreDataMember]
        public POPending POPending { get; set; }
        public Guid POGuid { get; set; }
        public POLinkStatus Status { get; set; }
        public POLinkAction PendingAction { get; set; }
        public POLinkAction ResponseAction { get; set; }
        public DateTime ResponseDate { get; set; }
        public DateTime Created { get; set; }
    }
}
