using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class UpdatePOStatus
    {
        [Required]
        public int POId { get; set; }
        [Required]
        public string StatusType { get; set; }
        [Required]
        public string Requestor { get; set; }
        public string ApproverName { get; set; }
        public string ApproverEmail { get; set; }
        public string Message { get; set; }

        public string? SIOrBI { get; set; }
        public int? TermsOfPayment { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}
