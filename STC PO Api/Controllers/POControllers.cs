using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using STC_PO_Api.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STC_PO_Api.Controllers
{
    [Route("po")]
    public class POControllers : Controller
    {
        private IPOPendingData _poData;
        private IPOGuidStatusData _pOGuidStatusData;
        private IPOAuditTrailData _pOAuditTrailData;
        private IPOApproverData _pOApproverData;
        private IPOSupplierData _pOSupplierData;
        private IPOAttachmentData _pOAttachmenData;
        private IPOExternalAttachmentData _pOExternalAttachmenData;
        private IUtils _utils;
        private IWebHostEnvironment _hostingEnvironment;
        private IConverter _converter;
        private IConfiguration _configuration;
        private ILinkActionData _linkActionData;

        public POControllers(IPOPendingData iPOPendingItemData, IPOGuidStatusData pOGuidStatusData, IPOAuditTrailData pOAuditTrailData, IPOApproverData pOApproverData, IPOSupplierData pOSupplierData ,IWebHostEnvironment hostingEnvironment, IConverter converter, IUtils utils, IPOAttachmentData pOAttachmentData, IConfiguration configuration, IPOExternalAttachmentData pOExternalAttachmentData, ILinkActionData linkActionData)
        {
            _hostingEnvironment = hostingEnvironment;
            _converter = converter;
            _poData = iPOPendingItemData;
            _pOGuidStatusData = pOGuidStatusData;
            _pOAuditTrailData = pOAuditTrailData;
            _pOApproverData = pOApproverData;
            _pOSupplierData = pOSupplierData;
            _pOAttachmenData = pOAttachmentData;
            _utils = utils;
            _configuration = configuration;
            _pOExternalAttachmenData = pOExternalAttachmentData;
            _linkActionData = linkActionData;
        }

        [HttpGet]
        public IActionResult GetPOs()
        {
            var pos = _poData.GetPOs();
            return Ok(pos);
        }

        [HttpPost]
        public IActionResult AddPO([FromForm] NewPOPending newPOPending)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var item = _poData.AddPO(newPOPending);
            if (item == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create PO");
            }

            //This is only for on-prem
            //var attachements = _pOAttachmenData.GetPOAttachmentsData(item.Id);
            //var TOs = _utils.GetApproverEmails().ToArray();
            //_pOGuidStatusData.AddPOGuid(item, attachements, TOs);


            //This is only for cloud
            _linkActionData.AddPOLink(item.Id, item.Guid, POLinkAction.ForApproval);

            //_utils.SendPOApproval(item);
            //_pOAuditTrailData.SendApprovalAuditLog(item.Id, item.CreatedByName, item.ApproverName, item.ApproverEmail);

            return Ok(item);
        }

        [HttpPut]
        public IActionResult UpdatePO([FromForm] EditPOPending pOPending)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var oldPO = _poData.GetPO(pOPending.Id);

            if (oldPO == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }


            string previousApprover = oldPO.ApproverEmail;

            var result = _poData.UpdatePO(pOPending, oldPO);


            var attachements = _pOAttachmenData.GetPOAttachmentsData(result.Id);
            //For On-prem only
            //_pOGuidStatusData.AddPOGuid(result, attachements, _utils.GetApproverEmails().ToArray());

            //For Cloud
            _linkActionData.AddPOLink(result.Id, result.Guid, POLinkAction.ForApproval);


            ///If you want to send update to approver use the code below
            if (oldPO.HasBeenApproved && previousApprover != null)
            {
                //_utils.SendUpdateToApprover(result, previousApprover, attachements);
            }

            result.POAttachments = attachements;

            //_utils.SendPOApproval(result);
            //_pOAuditTrailData.SendApprovalAuditLog(result.Id, result.CreatedByName, result.ApproverName, result.ApproverEmail);

            return Ok(result);
        }

        [HttpPut("draft")]
        public IActionResult UpdatePODraft([FromForm] EditPOPendingDraft pOPending)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

         
            var oldPO = _poData.GetPO(pOPending.Id);
            if (oldPO == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }

            string previousApprover = oldPO.ApproverEmail;

            var result = _poData.UpdateDraftPO(pOPending, oldPO);
            var attachements = _pOAttachmenData.GetPOAttachmentsData(result.Id);


            if (oldPO.HasBeenApproved)
            {
                _utils.SendUpdateToApprover(result, previousApprover, attachements);
            }

            return Ok(result);
        }

        [HttpPost("draft")]
        public IActionResult AddPODraft([FromForm] NewPOPendingDraft newPOPendingDraft)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var item = _poData.AddPODraft(newPOPendingDraft);

            return Ok(item);
        }

        [HttpGet("pdf/{id}")]
        public IActionResult GeneratePDF(int id)
        {
            var po = _poData.GetPO(id);
            if (po == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }

            string docPath = _hostingEnvironment.ContentRootPath;
            string fileName = String.Format(@"PO-{0}.pdf", po.OrderNumber);

            docPath = docPath + "\\po\\";

            bool exists = System.IO.Directory.Exists(docPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(docPath);

            docPath = docPath + fileName;

            if (System.IO.File.Exists(docPath))
            {
                // If file found, delete it    
                System.IO.File.Delete(docPath);
            }

            var pdfPO = new PDFPO
            {
                ApprovedOn = po.ApprovedOn,
                ApproverJobTitle = po.ApproverJobTitle,
                ApproverName = po.ApproverName,
                ContactPersonName = po.ContactPersonName,
                Currency = po.Currency,
                CustomerName = po.CustomerName,
                Discount = po.Discount,
                EstimatedArrival = po.EstimatedArrival,
                Id = po.OrderNumber.ToString(_utils.getCultureInfo()),
                POPendingItemsJsonString = JsonConvert.SerializeObject(po.POPendingItems),
                ReferenceNumber = po.ReferenceNumber,
                Remarks = po.Remarks,
                SupplierAddress = po.SupplierAddress,
                SupplierName = po.SupplierName,
                TextLineBreakCount = po.TextLineBreakCount,
                Total = po.Total,

                OrderNumber = po.OrderNumber,
                InvoiceDate = po.InvoiceDate,
                SIOrBI = po.SIOrBI,
                TermsOfPayment = po.TermsOfPayment

            };

            if (po.ApproverId != null)
            {
                pdfPO.ApproverId = (int)po.ApproverId;
            }


            if (po.Status == POStatus.Draft)
            {
                pdfPO.POPendingItemsJsonString = po.POPendingItemsJsonString;
            }

            if (po.CancelledOn != null)
            {
                pdfPO.ApprovedOn = null;
            }

            _utils.ConvertToPdf(pdfPO, docPath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(docPath);
            return File(fileBytes, "application/pdf");
        }

        [HttpPost("pdf/preview")]
        public IActionResult PreviewInPdf([FromBody] PDFPO po)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }


            if (po == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }

            var result = _utils.ConvertToPdf(po);
            return File(result, "application/pdf");
        }

        [HttpGet("updatepending")]
        public IActionResult CheckPending()
        {
            StringBuilder result = new StringBuilder(); ;
            var pendingPOs = _poData.GetPendingPOs();
            result.AppendFormat(new CultureInfo("en-US"), "Need to update {0} records. ", pendingPOs.Count);

            if (pendingPOs.Count != 0)
            {
                var pOsStatus = _pOGuidStatusData.GetPOStatus(pendingPOs);
                result.AppendFormat(new CultureInfo("en-US"), "Found {0} records in cloud. ", pOsStatus.Count);

                var updatedPOs = _poData.UpdatePOsStatus(pOsStatus, pendingPOs);
                _pOGuidStatusData.DeletePOStatus(updatedPOs);
                result.AppendFormat(new CultureInfo("en-US"), "Updated {0} records. ", updatedPOs.Count);
            }

            return Ok(result.ToString());
        }

        [HttpPost("audittrail")]
        public IActionResult AddAuditTrail([FromBody] NewAuditTrail newAuditTrail)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var auditTrail = _pOAuditTrailData.AddAuditTrail(newAuditTrail);
            if (auditTrail == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to add audit trail");
            }

            return Ok(auditTrail);
        }

        [HttpGet("{id}/audittrail")]
        public IActionResult GetPOAuditTrail(int id)
        {
            var result = _pOAuditTrailData.GetPOAuditTrails(id);
            return Ok(result);
        }

        [HttpPost("approver")]
        public IActionResult AddApprover([FromForm] NewPOApprover newPOApprover)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var approver = _pOApproverData.AddPOApprover(newPOApprover);
            if (approver == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "We're unable to process your request");
            }

            return Ok(approver);
        }

        [HttpGet("approvers")]
        public IActionResult GetApprovers()
        {
            var result = _pOApproverData.GetApprovers();
            return Ok(result);
        }

        [HttpPut("approver/{id}")]
        public IActionResult UpdateApprover([FromForm] EditPOApprover editPOApprover, int id)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var approver = _pOApproverData.GetPOApprover(id);
            if (approver == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Approver not found");
            }

            approver = _pOApproverData.UpdatePOApprover(editPOApprover, approver);

            return Ok(approver);
        }

        [HttpDelete("approver/{id}")]
        public IActionResult DeleteApprover(int id)
        {
            var approver = _pOApproverData.GetPOApprover(id);
            if (approver == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Approver not found");
            }

            _pOApproverData.DeleteApprover(approver);

            return Ok();
        }

        [HttpGet("approver/{id}/signature")]
        public IActionResult ApproverSignature(int id)
        {
            var approver = _pOApproverData.GetPOApprover(id);
            return File(approver.SignatureImg, "image/jpeg");
        }

        [HttpPost("supplier")]
        public IActionResult AddSupplier([FromBody] NewPOSupplier newPOSupplier)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var supplier = _pOSupplierData.AddPOSupplier(newPOSupplier);

            if (supplier == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create supplier");
            }

            return Ok(supplier);
        } 
        
        [HttpGet("suppliers")]
        public IActionResult GetSuppliers()
        {
            return Ok(_pOSupplierData.GetSuppliers());
        }

        [HttpPut("supplier/{id}")]
        public IActionResult UpdateSupplier([FromBody] EditPOSupllier editPOSupllier, int id)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var supplier = _pOSupplierData.GetPOSupplier(id);

            if (supplier == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Supplier not found");
            }

            var result = _pOSupplierData.UpdatePOSupplier(supplier, editPOSupllier);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to update supplier");
            }

            return Ok(result);
        }

        [HttpPost("status")]
        public IActionResult UpdatePOStatus([FromBody] UpdatePOStatus updatePOStatus)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var po = _poData.GetPO(updatePOStatus.POId);

            if (po == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }

            var result = _poData.UpdatePOStatus(updatePOStatus, po);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to process your request");
            }

            if (updatePOStatus.StatusType.ToUpper(_utils.getCultureInfo()) == "RECEIVED")
            {

            } else if (updatePOStatus.StatusType.ToUpper(_utils.getCultureInfo()) == "CANCEL")
            {
                //Request for cancellation

                //For On-prem only
                //_pOGuidStatusData.AddPOGuid(result.pO, attachements, _utils.GetApproverEmails().ToArray());
                //var attachements = _pOAttachmenData.GetPOAttachmentsData(po.Id);
                //_utils.SendPOCancellation(result.pO, updatePOStatus);

                //For Cloud
                var link = _linkActionData.AddPOLink(result.pO.Id, result.pO.Guid, POLinkAction.ForCancellation);
                
                return Ok(
                    new {
                        Log = result.pOAuditTrail,
                        Link = link
                    }
                );
            }

            return Ok(result.pOAuditTrail);
        }

        [HttpPost("siorbi")]
        public IActionResult UpdateSIorBI([FromBody] UpdatePOStatus updatePOStatus)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var po = _poData.GetPO(updatePOStatus.POId);

            if (po == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }

            var result = _poData.UpdatePOSIorBI(updatePOStatus, po);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to process your request");
            }

            return Ok(result.pOAuditTrail);
        }

        [HttpGet("attachments/{poId}")]
        public IActionResult GetPOAttachments(int poId)
        {
            var result = _pOAttachmenData.GetPOAttachments(poId);
            return Ok(result);
        }

        [HttpGet("externalattachments/{poId}")]
        public IActionResult GetPOExternalAttachments(int poId)
        {
            var result = _pOExternalAttachmenData.GetPOExternalAttachments(poId);
            return Ok(result);
        }

        [HttpGet("externalattachmentsdata/{poId}")]
        public IActionResult GetPOExternalAttachmentsData(int poId)
        {
            var result = _pOExternalAttachmenData.GetPOExternalAttachmentsData(poId);
            return Ok(result);
        }

        [HttpGet("{poId}/sendupdates")]
        public IActionResult SendEmailUpdates(int poId)
        {
            var poWithLogs = _poData.GetPOWithLogs(poId);

            if (poWithLogs == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }

            _utils.SendPOEmailUpdates(poWithLogs, String.Format(_utils.getCultureInfo(), @"PO No. {0} Updates", poWithLogs.Id));

            return Ok();
        }

        [HttpGet("allowedgroups")]
        public IActionResult GetAllowedGroups()
        {
            var readEmails = _utils.GetGroupsEmailsWithReadPermission();
            var modifyEmails = _utils.GetGroupsEmailsWithModifyPermission();
            var receiverEmails = _utils.GetGroupsEmailsWithReceiverPermission();
            var approvedEmails = _utils.GetApproverEmails();
            var updatesEmails = _utils.GetGroupsEmailsThatPOUpdatesShouldNotify();

            return Ok(new {
                readPermissions = readEmails,
                modifyPermissions = modifyEmails,
                receiverPermissions = receiverEmails,
                sendApprovePOTo = approvedEmails,
                sendPOUpdatesTo = updatesEmails
            });
        }
    
        [HttpPost("statuses")]
        public IActionResult GetPOStatuses([FromBody] POIds poIDs)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
            }

            var result = _poData.GetPOStatuses(poIDs);

            return Ok(result);
        }
    }

}
