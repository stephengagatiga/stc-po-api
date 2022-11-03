using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;

namespace STC_PO_Api.Services
{
    public class SqlPOGuidStatusData : IPOGuidStatusData
    {
        POGuidStatusContext _context;
        public SqlPOGuidStatusData(POGuidStatusContext context)
        {
            _context = context;
        }

        public POGuidStatus AddPOGuid(POPending pO, ICollection<POAttachment> pOAttachments, string[] sendTOs)
        {
            if (pO == null)
            {
                return null;
            }

            //Delete other related POs
            //var posToDelete = _context.POGuidStatus.Where(p => p.TrackerId == pO.TrackerId);
            //foreach (var item in posToDelete)
            //{
            //    var posAttachmentToDelte = _context.POGuidStatusAttachments.Where(a => a.POGuidStatusId == item.Id);
            //    foreach (var att in posAttachmentToDelte)
            //    {
            //        _context.POGuidStatusAttachments.Remove(att);
            //        _context.Entry(att).State = EntityState.Deleted;
            //    }

            //    _context.POGuidStatus.Remove(item);
            //    _context.Entry(item);

            //}


            POPending p = new POPending();
            p.ApprovedOn = pO.ApprovedOn;
            p.ApproverEmail = pO.ApproverEmail;
            p.ApproverId = pO.ApproverId;
            p.ApproverName = pO.ApproverName;
            p.CancelledOn = pO.CancelledOn;
            p.ContactPersonId = pO.ContactPersonId;
            p.ContactPersonName = pO.ContactPersonName;
            p.CreatedById = pO.CreatedById;
            p.CreatedByName = pO.CreatedByName;
            p.CreatedOn = pO.CreatedOn;
            p.Currency = pO.Currency;
            p.CustomerName = pO.CustomerName;
            p.Discount = pO.Discount;
            p.EstimatedArrival = pO.EstimatedArrival;
            p.EstimatedArrivalString = pO.EstimatedArrivalString;
            p.Guid = pO.Guid;
            p.HasBeenApproved = pO.HasBeenApproved;
            p.Id = pO.Id;
            p.InternalNote = pO.InternalNote;
            p.POPendingItemsJsonString = JsonConvert.SerializeObject(pO.POPendingItems);
            p.ReceivedOn = pO.ReceivedOn;
            p.ReferenceNumber = pO.ReferenceNumber;
            p.Remarks = pO.Remarks;
            p.RequestorEmail = pO.RequestorEmail;
            p.RequestorName = pO.RequestorName;
            p.Status = pO.Status;
            p.SupplierAddress = pO.SupplierAddress;
            p.SupplierId = pO.SupplierId;
            p.SupplierName = pO.SupplierName;
            p.TextLineBreakCount = pO.TextLineBreakCount;
            p.Total = pO.Total;
            p.OrderNumber = pO.OrderNumber;

            string pOData = JsonConvert.SerializeObject(p);

            POGuidStatus pOGuidStatus = new POGuidStatus() { 
                    POGuid = pO.Guid,
                    POStatus = POStatus.Pending,
                    AddedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")),
                    RequestorName = pO.RequestorName,
                    RequestorEmail = pO.RequestorEmail,
                    POData = pOData,
                    SendTOs = JsonConvert.SerializeObject(sendTOs),
                    POGuidStatusAttachments = new List<POGuidStatusAttachment>(),
                    TrackerId = pO.TrackerId
            };

            if (pOAttachments != null)
            {
                foreach (var item in pOAttachments)
                {
                    pOGuidStatus.POGuidStatusAttachments.Add(new POGuidStatusAttachment()
                    {
                        ContentType = item.ContentType,
                        CreatedOn = item.CreatedOn,
                        File = item.File,
                        Name = item.Name,
                        Size = item.Size,
                    });
                }
            }
            
            _context.POGuidStatus.Add(pOGuidStatus);
            _context.Entry(pOGuidStatus).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            _context.SaveChanges();

            return pOGuidStatus;
        }

        public void DeletePOStatus(ICollection<POGuidStatus> pOGuidStatusess)
        {
            foreach (var item in pOGuidStatusess)
            {
                var po = _context.POGuidStatus
                                .Where(p => p.TrackerId == item.TrackerId)
                                .Include(p => p.POGuidStatusAttachments)
                                .ToList();

                foreach (var poToDelete in po)
                {
                    if (poToDelete != null)
                    {
                        _context.POGuidStatus.Remove(poToDelete);
                        _context.Entry(poToDelete).State 
                            = EntityState.Deleted;
                    }
                }
            }

            _context.SaveChanges();
        }

        public void DeleteUpdatedPO(ICollection<POGuidStatus> pOGuidStatuses)
        {
            foreach (var item in pOGuidStatuses)
            {
                _context.POGuidStatus.Remove(item);
                _context.Entry(item).State = EntityState.Deleted;
            }

            _context.SaveChanges();
        }

        public ICollection<POGuidStatus> GetPOStatus(ICollection<POPending> POs)
        {
            //You need to store the Guid in the in the list
            //so the Linq can translate your querty to sql
            List<Guid> tmpGuid = new List<Guid>();
            foreach (var item in POs)
            {
                tmpGuid.Add(item.Guid);
            }

            return _context.POGuidStatus.Where(p => tmpGuid.Contains(p.POGuid) & (p.POStatus != POStatus.Pending && p.POStatus != POStatus.ForCancellation)).ToList();
        }
    }
}
