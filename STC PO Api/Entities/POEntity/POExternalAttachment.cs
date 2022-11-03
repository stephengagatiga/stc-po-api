using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POEntity
{
    public class POExternalAttachment
    {
        [Key]
        public int Id { get; set; }
        public int POId { get; set; }
        [ForeignKey("POId")]
        public POPending POPending { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public byte[] File { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
