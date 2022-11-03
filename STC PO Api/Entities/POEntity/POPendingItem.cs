using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POEntity
{
    public class POPendingItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int POPendingId { get; set; }
        [ForeignKey("POPendingId")]
        //Prevent Self refencing loop
        [IgnoreDataMember]
        public POPending POPending { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(minimum: -1_000_000_000, maximum: 1_000_000_000)]
        public decimal Price { get; set; }
        [Required]
        [Range(minimum: 1, maximum: int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        [Range(minimum: -1_000_000_000, maximum: 1_000_000_000)]
        public decimal Total { get; set; }
        public int Order { get; set; }
    }
}
