using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POEntity
{
    public class POGuidStatus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid POGuid { get; set; }
        [Required]
        public POStatus POStatus { get; set; }
        [Required]
        public DateTime AddedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string RequestorName { get; set; }
        public string RequestorEmail { get; set; }
        public string POData { get; set; }
        public string SendTOs { get; set; }
        public ICollection<POGuidStatusAttachment> POGuidStatusAttachments { get; set; }
        [Required]
        public Guid TrackerId { get; set; }

    }
}
