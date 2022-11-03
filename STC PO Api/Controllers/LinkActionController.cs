using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STC_PO_Api.Models.PO;
using STC_PO_Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Controllers
{
    [Route("l")]
    public class LinkActionController : Controller
    {
        private IPOPendingData _poPendingData;
        private ILinkActionData _linkActionData;
        private IPOAuditTrailData _pOAuditTrailData;
        private IUtils _utils;
        private IPOAttachmentData _pOAttachmenData;

        public LinkActionController(IPOPendingData poPendingData, ILinkActionData linkActionData, IPOAuditTrailData pOAuditTrailData, IUtils utils, IPOAttachmentData pOAttachmentData)
        {
            _poPendingData = poPendingData;
            _linkActionData = linkActionData;
            _pOAuditTrailData = pOAuditTrailData;
            _utils = utils;
            _pOAttachmenData = pOAttachmentData;
        }

        [HttpGet("approve/{id}")]

        public async Task<IActionResult> ApprovePO(Guid id)
        {
            var poLink = _linkActionData.GetPOLinkByGuid(id);

            if (poLink == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }


            if (poLink.Status == Entities.POEntity.POLinkStatus.Expired)
            {
                //If the Guid is not match that means the PO has been updated
                if (poLink.POGuid != poLink.POPending.Guid)
                {
                    string msg = String.Format(_utils.getCultureInfo(), "This link is expired. The PO has been updated and you cannot use this link anymore. ");
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                } else
                {
                    //if this match, then the user has click again the link
                    string msg = String.Format(_utils.getCultureInfo(), "This PO has been {0} on {1}", poLink.ResponseAction, poLink.ResponseDate.ToShortDateString());
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                }
            }

            _linkActionData.ApprovePO(poLink.POPending);

            _linkActionData.UpdatePOLink(poLink, Entities.POEntity.POLinkAction.Approved);

            NewAuditTrail newAuditTrail = new NewAuditTrail()
            {
                Message = "Approved",
                POPendingId = poLink.POPendingId,
                UserName = poLink.POPending.ApproverName
            };


            //For cloud only
            var TOs = _utils.GetApproverEmails().ToArray();
            var attachements = _pOAttachmenData.GetPOAttachmentsData(poLink.POPendingId);
            await _utils.SendPOToDistirubtionListAsync(TOs, attachements, poLink.POPending);

            _pOAuditTrailData.AddAuditTrail(newAuditTrail);

            return Ok("PO Approved, you may now close this page.");
        }

        [HttpGet("reject/{id}")]
        public IActionResult RejectPO(Guid id)
        {

            var poLink = _linkActionData.GetPOLinkByGuid(id);

            if (poLink == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }


            if (poLink.Status == Entities.POEntity.POLinkStatus.Expired)
            {
                //If the Guid is not match that means the PO has been updated
                if (poLink.POGuid != poLink.POPending.Guid)
                {
                    string msg = String.Format(_utils.getCultureInfo(), "This link is expired. The PO has been updated and you cannot use this link anymore. ");
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                }
                else
                {
                    //if this match, then the user has click again the link
                    string msg = String.Format(_utils.getCultureInfo(), "This PO has been {0} on {1}", poLink.ResponseAction, poLink.ResponseDate.ToShortDateString());
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                }
            }

            _linkActionData.RejectPO(poLink.POPending);

            _linkActionData.UpdatePOLink(poLink, Entities.POEntity.POLinkAction.Rejected);


            NewAuditTrail newAuditTrail = new NewAuditTrail()
            {
                Message = "Rejected",
                POPendingId = poLink.POPendingId,
                UserName = poLink.POPending.ApproverName
            };

            _pOAuditTrailData.AddAuditTrail(newAuditTrail);


            return Ok("PO Rejected, you may now close this page.");

        }

        [HttpGet("cancel/approve/{id}")]
        public async Task<IActionResult> ApprovePOCacellation(Guid id)
        {

            var poLink = _linkActionData.GetPOLinkByGuid(id);

            if (poLink == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }


            if (poLink.Status == Entities.POEntity.POLinkStatus.Expired)
            {
                //If the Guid is not match that means the PO has been updated
                if (poLink.POGuid != poLink.POPending.Guid)
                {
                    string msg = String.Format(_utils.getCultureInfo(), "This link is expired. The PO has been updated and you cannot use this link anymore. ");
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                }
                else
                {
                    //if this match, then the user has click again the link
                    string msg = String.Format(_utils.getCultureInfo(), "This PO has been {0} on {1}", poLink.ResponseAction, poLink.ResponseDate.ToShortDateString());
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                }
            }

            _linkActionData.ApproveCancellationPO(poLink.POPending);

            _linkActionData.UpdatePOLink(poLink, Entities.POEntity.POLinkAction.Cancelled);


            NewAuditTrail newAuditTrail = new NewAuditTrail()
            {
                Message = "Cancelled",
                POPendingId = poLink.POPendingId,
                UserName = poLink.POPending.ApproverName
            };

            _pOAuditTrailData.AddAuditTrail(newAuditTrail);


            //For cloud only
            var TOs = _utils.GetApproverEmails().ToArray();
            var attachements = _pOAttachmenData.GetPOAttachmentsData(poLink.POPendingId);
            await _utils.SendPOToDistirubtionListAsync(TOs, attachements, poLink.POPending);

            return Ok("PO Cancelled, you may now close this page.");
        }

        [HttpGet("cancel/reject/{id}")]
        public async Task<IActionResult> RejectPOCacellation(Guid id)
        {

            var poLink = _linkActionData.GetPOLinkByGuid(id);

            if (poLink == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "PO not found");
            }


            if (poLink.Status == Entities.POEntity.POLinkStatus.Expired)
            {
                //If the Guid is not match that means the PO has been updated
                if (poLink.POGuid != poLink.POPending.Guid)
                {
                    string msg = String.Format(_utils.getCultureInfo(), "This link is expired. The PO has been updated and you cannot use this link anymore. ");
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                }
                else
                {
                    //if this match, then the user has click again the link
                    string msg = String.Format(_utils.getCultureInfo(), "This PO has been {0} on {1}", poLink.ResponseAction, poLink.ResponseDate.ToShortDateString());
                    return StatusCode(StatusCodes.Status404NotFound, msg);
                }
            }

            _linkActionData.RejectCancellationPO(poLink.POPending);

            _linkActionData.UpdatePOLink(poLink, Entities.POEntity.POLinkAction.CancellationRejected);


            NewAuditTrail newAuditTrail = new NewAuditTrail()
            {
                Message = "Cancellation Rejected",
                POPendingId = poLink.POPendingId,
                UserName = poLink.POPending.ApproverName
            };

            _pOAuditTrailData.AddAuditTrail(newAuditTrail);

            //For cloud only
            var TOs = _utils.GetGroupsEmailsThatPOUpdatesShouldNotify().ToArray();
            var attachements = _pOAttachmenData.GetPOAttachmentsData(poLink.POPendingId);
            await _utils.SendPOToDistirubtionListAsync(TOs, attachements, poLink.POPending);

            return Ok("PO request for cancellation rejected, you may now close this page.");
        }
    }
}
