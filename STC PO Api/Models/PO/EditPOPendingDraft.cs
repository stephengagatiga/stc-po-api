using Microsoft.AspNetCore.Http;
using STC_PO_Api.Entities.POEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class EditPOPendingDraft
    {
        [Required]
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public int? ContactPersonId { get; set; }
        public string ContactPersonName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public string Currency { get; set; }
        public decimal Discount { get; set; }
        public int? ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string ApproverEmail { get; set; }
        public string ApproverJobTitle { get; set; }
        public string InternalNote { get; set; }
        public string Remarks { get; set; }
        [Required]
        public string ModifiedByName { get; set; }
        public string RequestorEmail { get; set; }
        public string POPendingItemsJsonString { get; set; }
        public POStatus Status { get; set; }
        public string TextLineBreakCount { get; set; }
        public int[] AttachmentIdsToRemove { get; set; }
        public IFormFile[] Files { get; set; }
        public int[] ExternalAttachmentIdsToRemove { get; set; }
        public IFormFile[] ExternalFiles { get; set; }

        public string? SIOrBI { get; set; }
        public string? TermsOfPayment { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}
