using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class NewPOPending
    {
        [Required]
        public string ReferenceNumber { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public string SupplierName { get; set; }
        [Required]
        public string SupplierAddress { get; set; }
        [Required]
        public int ContactPersonId { get; set; }
        [Required]
        public string ContactPersonName { get; set; }
        [Required]
        public string CustomerName { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        [Required]
        public string Currency { get; set; }
        [Range(minimum: 0, maximum: 100)]
        public decimal Discount { get; set; }
        [Required]
        public int ApproverId { get; set; }
        [Required]
        public string ApproverName { get; set; }
        [Required]
        public string ApproverEmail { get; set; }
        [Required]
        public string ApproverJobTitle { get; set; }
        public string InternalNote { get; set; }
        public string Remarks { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public string CreatedByName { get; set; }
        [Required]
        public string RequestorEmail { get; set; }
        [Required]
        public string POPendingItems { get; set; }
        [Required]
        public string TextLineBreakCount { get; set; }
        public IFormFile[] Files { get; set; }
        public IFormFile[] ExternalFiles { get; set; }

        public string? SIOrBI { get; set; }
        public string? TermsOfPayment { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }

}
