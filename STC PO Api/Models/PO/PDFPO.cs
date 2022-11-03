using STC_PO_Api.Entities.POEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class PDFPO
    {
        public string Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string ContactPersonName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public string Currency { get; set; }
        public string ApproverName { get; set; }
        public string ApproverJobTitle { get; set; }
        public int ApproverId { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public string Remarks { get; set; }
        public string POPendingItemsJsonString { get; set; }
        
        //This hold how many line breaks are there in PO items and remarks
        //The purpose of this is to disrtibue the PO items in pages if there are more
        //and to keep the signatory at bottom when not in the page 1\
        //the format of this is: 10,30,20 
        //where the last value is the remarks line breaks count
        [Required]
        public string TextLineBreakCount { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public int OrderNumber { get; set; }

        public string? SIOrBI { get; set; }
        public string? TermsOfPayment { get; set; }
        public DateTime? InvoiceDate { get; set; }

    }
}
