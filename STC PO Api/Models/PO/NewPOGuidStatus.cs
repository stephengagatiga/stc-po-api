using STC_PO_Api.Entities.POEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class NewPOGuidStatus
    {
        [Required]
        public Guid POGuid { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
