using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public class SqlPOPendingData : IPOPendingData
    {
        private POContext _context;

        public SqlPOPendingData(POContext context)
        {
            _context = context;
        }

        public POPending AddPO(NewPOPending pOPending)
        {
            var item = new POPending()
            {
                ReferenceNumber = pOPending.ReferenceNumber,

                SupplierId = pOPending.SupplierId,
                SupplierName = pOPending.SupplierName,
                SupplierAddress = pOPending.SupplierAddress,

                ContactPersonId = pOPending.ContactPersonId,
                ContactPersonName = pOPending.ContactPersonName,

                CustomerName = pOPending.CustomerName,

                EstimatedArrival = pOPending.EstimatedArrival,
                EstimatedArrivalString = pOPending.EstimatedArrival != null ? ((DateTime)(pOPending.EstimatedArrival)).ToString("dddd, dd MMMM yyyy") : "",

                Currency = pOPending.Currency,

                ApproverName = pOPending.ApproverName,
                ApproverId = pOPending.ApproverId,
                ApproverEmail = pOPending.ApproverEmail,
                ApproverJobTitle = pOPending.ApproverJobTitle,

                InternalNote = pOPending.InternalNote,
                Remarks = pOPending.Remarks,

                Discount = pOPending.Discount,
                CreatedByName = pOPending.CreatedByName,
                CreatedById = pOPending.CreatedById,
                RequestorName = pOPending.CreatedByName,
                RequestorEmail = pOPending.RequestorEmail,
                CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")),
                Status = POStatus.Pending,
                Guid = Guid.NewGuid(),
                TextLineBreakCount = pOPending.TextLineBreakCount,
                HasBeenApproved = false,
                POPendingItemsJsonString = pOPending.POPendingItems,
                TrackerId = Guid.NewGuid(),

                SIOrBI = pOPending.SIOrBI,
                InvoiceDate = pOPending.InvoiceDate,
                TermsOfPayment = pOPending.TermsOfPayment
            };

            var parseItems = JsonConvert.DeserializeObject<NewPOPendingItem[]>(pOPending.POPendingItems);

            item.POPendingItems = new List<POPendingItem>();
            decimal total = 0;

            foreach (var i in parseItems)
            {
                var pOItems = new POPendingItem()
                {
                    Name = i.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Total = i.Price * i.Quantity,
                    Order = i.Order
                };

                total += pOItems.Total;
                item.POPendingItems.Add(pOItems);
            }
            decimal discountAmount = Math.Round((item.Discount / 100) * total, 2);
            
            item.Total = total - discountAmount;

            item.POAuditTrails = new List<POAuditTrail>();
            POAuditTrail pOAuditTrail = new POAuditTrail()
            {
                POPendingId = item.Id,
                UserName = item.CreatedByName,
                DateAdded = item.CreatedOn,
                Message = "Created"
            };
            item.POAuditTrails.Add(pOAuditTrail);

            //Internal Attachments
            item.POAttachments = new List<POAttachment>();
            if (pOPending.Files != null)
            {
                foreach (var f in pOPending.Files)
                {
                
                    var attachment = new POAttachment()
                    {
                        POId = item.Id,
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                    
                        attachment.File = fileBytes;
                    }

                    item.POAttachments.Add(attachment);
                }
            }

            //External Attachments
            item.POExternalAttachments = new List<POExternalAttachment>();
            if (pOPending.ExternalFiles != null)
            {
                foreach (var f in pOPending.ExternalFiles)
                {

                    var attachment = new POExternalAttachment()
                    {
                        POId = item.Id,
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();

                        attachment.File = fileBytes;
                    }

                    item.POExternalAttachments.Add(attachment);
                }
            }


            _context.POPendings.Add(item);
            _context.Entry(item).State = EntityState.Added;
            _context.SaveChanges();

            return item;
        }

        public POPending AddPODraft(NewPOPendingDraft pOPendingDraft)
        {

            if (pOPendingDraft == null)
            {
                return null;
            }
            
            var item = new POPending()
            {
                ReferenceNumber = pOPendingDraft.ReferenceNumber,

                SupplierId = pOPendingDraft.SupplierId,
                SupplierName = pOPendingDraft.SupplierName,
                SupplierAddress = pOPendingDraft.SupplierAddress,

                ContactPersonId = pOPendingDraft.ContactPersonId,
                ContactPersonName = pOPendingDraft.ContactPersonName,

                CustomerName = pOPendingDraft.CustomerName,

                EstimatedArrival = pOPendingDraft.EstimatedArrival,
                EstimatedArrivalString = pOPendingDraft.EstimatedArrival != null ? ((DateTime)(pOPendingDraft.EstimatedArrival)).ToString("dddd, dd MMMM yyyy") : "",

                Currency = pOPendingDraft.Currency,

                ApproverName = pOPendingDraft.ApproverName,
                ApproverId = pOPendingDraft.ApproverId,
                ApproverEmail = pOPendingDraft.ApproverEmail,
                ApproverJobTitle = pOPendingDraft.ApproverJobTitle,

                InternalNote = pOPendingDraft.InternalNote,
                Remarks = pOPendingDraft.Remarks,

                Discount = pOPendingDraft.Discount,
                CreatedByName = pOPendingDraft.CreatedByName,
                CreatedById = pOPendingDraft.CreatedById,
                RequestorName = pOPendingDraft.CreatedByName,
                RequestorEmail = pOPendingDraft.RequestorEmail,
                CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")),
                Status = POStatus.Draft,
                POPendingItemsJsonString = pOPendingDraft.POPendingItemsJsonString,
                TextLineBreakCount = pOPendingDraft.TextLineBreakCount,
                HasBeenApproved = false,
                TrackerId = Guid.NewGuid(),

                SIOrBI = pOPendingDraft.SIOrBI,
                InvoiceDate = pOPendingDraft.InvoiceDate,
                TermsOfPayment = pOPendingDraft.TermsOfPayment
            };

            item.POAuditTrails = new List<POAuditTrail>();

            POAuditTrail pOAuditTrail = new POAuditTrail()
            {
                UserName = item.CreatedByName,
                DateAdded = item.CreatedOn,
                Message = "Saved as draft"
            };
            item.POAuditTrails.Add(pOAuditTrail);


            //Internal Attachment
            item.POAttachments = new List<POAttachment>();
            if (pOPendingDraft.Files != null)
            {
                foreach (var f in pOPendingDraft.Files)
                {
                    var attachment = new POAttachment()
                    {
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        attachment.File = fileBytes;
                    }

                    item.POAttachments.Add(attachment);
                }
            }

            //External Attachment
            item.POExternalAttachments = new List<POExternalAttachment>();
            if (pOPendingDraft.ExternalFiles != null)
            {
                foreach (var f in pOPendingDraft.ExternalFiles)
                {
                    var attachment = new POExternalAttachment()
                    {
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        attachment.File = fileBytes;
                    }

                    item.POExternalAttachments.Add(attachment);
                }
            }

            decimal total = 0;
            var parseItems = JsonConvert.DeserializeObject<NewPOPendingItem[]>(item.POPendingItemsJsonString);
            foreach (var i in parseItems)
            {
                total += i.Quantity * i.Price;
            }

            decimal discountAmount = Math.Round((item.Discount / 100) * total, 2);
            item.Total = total - discountAmount;


            _context.POPendings.Add(item);
            _context.Entry(item).State = EntityState.Added;

            _context.SaveChanges();

            return item;
        }

        public POPending GetPO(int poId)
        {
            var po = _context.POPendings.Where(p => p.Id == poId)
                                            .Include(p => p.POPendingItems)
                                            .FirstOrDefault();

            return po;
        }

        public POPending GetPOByGuid(Guid guid)
        {
            var po = _context.POPendings.Where(p => p.Guid == guid)
                                    .Include(p => p.POPendingItems)
                                    .FirstOrDefault();
            return po;
        }

        public ICollection<POPending> GetPOs(int skip, int take)
        {
            var result = _context.POPendings
                                    .OrderByDescending(p => p.Id)
                                    .Include(i => i.POPendingItems)
                                    .Skip(skip).Take(take)
                                    .ToList();
            return result;
        }

        public ICollection<POPending> SearchPOs(string keyword, string searchType)
        {
            var result = new List<POPending>();

            switch(searchType)
            {
                case "PO":
                    var on = int.Parse(keyword);
                    result = _context.POPendings
                        .Where(p => p.OrderNumber == on)
                        .Include(i => i.POPendingItems)
                        .OrderByDescending(p => p.Id)
                        .Take(100)
                        .ToList();
                break;
                case "Supplier":
                    result = _context.POPendings
                        .Where(p => p.SupplierName.Contains(keyword))
                        .Include(i => i.POPendingItems)
                        .OrderByDescending(p => p.Id)
                        .Take(100)
                        .ToList();
                break;
                case "Customer":
                    result = _context.POPendings
                        .Where(p => p.CustomerName.Contains(keyword))
                        .Include(i => i.POPendingItems)
                        .OrderByDescending(p => p.Id)
                        .Take(100)
                        .ToList();
                break;
                case "Reference":
                    result = _context.POPendings
                        .Where(p => p.ReferenceNumber.Contains(keyword))
                        .Include(i => i.POPendingItems)
                        .OrderByDescending(p => p.Id)
                        .Take(100)
                        .ToList();
                break;
                default:
                    var on2 = int.Parse(keyword);
                    result = _context.POPendings
                        .Where(p => p.OrderNumber == on2)
                        .Include(i => i.POPendingItems)
                        .OrderByDescending(p => p.Id)
                        .Take(100)
                        .ToList();
                break;
            }

            return result;
        }

        public POPending UpdatePO(EditPOPending newPOPending, POPending oldPOPending)
        {

            StringBuilder message = new StringBuilder();
            message.AppendFormat(new CultureInfo("en-US"), "Updated the following: ");

            if (oldPOPending.ApproverId != newPOPending.ApproverId)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nApprover from {0} to {1}", oldPOPending.ApproverName, newPOPending.ApproverName);
            }

            if (oldPOPending.SupplierId != newPOPending.SupplierId)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nSupplier from {0} to {1}", oldPOPending.SupplierName, newPOPending.SupplierName);
            }

            if (oldPOPending.ContactPersonId != newPOPending.ContactPersonId)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nContact Person from {0} to {1}", oldPOPending.ContactPersonName, newPOPending.ContactPersonName);
            }

            if (oldPOPending.ReferenceNumber != newPOPending.ReferenceNumber)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nReference Number from {0} to {1}", oldPOPending.ReferenceNumber, newPOPending.ReferenceNumber);
            }

            if (oldPOPending.EstimatedArrival != newPOPending.EstimatedArrival)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nExpected Delivery from {0} to {1}", oldPOPending.EstimatedArrival == null ? "" : ((DateTime)oldPOPending.EstimatedArrival).ToString("MM/dd/yyyy"), ((DateTime)newPOPending.EstimatedArrival).ToString("MM/dd/yyyy"));
            }

            if (oldPOPending.CustomerName != newPOPending.CustomerName)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nCustomer Name from {0} to {1}", oldPOPending.CustomerName, newPOPending.CustomerName);
            }

            if (oldPOPending.Currency != newPOPending.Currency)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nCurrency from {0} to {1}", oldPOPending.Currency, newPOPending.Currency);
            }

            if (oldPOPending.Discount != newPOPending.Discount)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nDiscount from {0} to {1}", oldPOPending.Discount, newPOPending.Discount);
            }

            if (oldPOPending.Remarks != newPOPending.Remarks)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nRemarks from \n{0} \nto \n{1}", oldPOPending.Remarks, newPOPending.Remarks);
            }

            if (oldPOPending.InternalNote != newPOPending.InternalNote)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nInternal Note from \n{0} \nto \n{1}", oldPOPending.InternalNote, newPOPending.InternalNote);
            }

            if (oldPOPending.Status != newPOPending.Status)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nStatus from {0} to {1}", oldPOPending.Status.ToString(), newPOPending.Status.ToString());
            }

            if (oldPOPending.InvoiceDate != newPOPending.InvoiceDate)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nInvoice Date from {0} to {1}",
                    oldPOPending.InvoiceDate == null ? "" : ((DateTime)oldPOPending.InvoiceDate).ToString("MM/dd/yyyy"),
                    newPOPending.InvoiceDate == null ? "" : ((DateTime)newPOPending.InvoiceDate).ToString("MM/dd/yyyy"));
            }

            if (oldPOPending.SIOrBI != newPOPending.SIOrBI)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nS.I# or B.I# from {0} to {1}", 
                    oldPOPending.SIOrBI, 
                    newPOPending.SIOrBI);
            }

            if (oldPOPending.TermsOfPayment != newPOPending.TermsOfPayment)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nTerms Of Payment {0} to {1}",
                    oldPOPending.TermsOfPayment,
                    newPOPending.TermsOfPayment);
            }

            oldPOPending.ApproverEmail = newPOPending.ApproverEmail;
            oldPOPending.ApproverId = newPOPending.ApproverId;
            oldPOPending.ApproverName = newPOPending.ApproverName;
            oldPOPending.ApproverJobTitle = newPOPending.ApproverJobTitle;
            oldPOPending.ContactPersonId = newPOPending.ContactPersonId;
            oldPOPending.ContactPersonName = newPOPending.ContactPersonName;
            oldPOPending.Currency = newPOPending.Currency;
            oldPOPending.CustomerName = newPOPending.CustomerName;
            oldPOPending.Discount = newPOPending.Discount;
            oldPOPending.EstimatedArrival = newPOPending.EstimatedArrival;
            oldPOPending.EstimatedArrivalString = newPOPending.EstimatedArrival == null ? null : ((DateTime)newPOPending.EstimatedArrival).ToString("MM/dd/yyyy");
            oldPOPending.Guid = Guid.NewGuid();
            oldPOPending.InternalNote = newPOPending.InternalNote;
            oldPOPending.POPendingItemsJsonString = newPOPending.POPendingItemsJsonString;
            oldPOPending.ReferenceNumber = newPOPending.ReferenceNumber;
            oldPOPending.Remarks = newPOPending.Remarks;
            oldPOPending.RequestorName = newPOPending.ModifiedByName;
            oldPOPending.RequestorEmail = newPOPending.RequestorEmail;
            oldPOPending.Status = newPOPending.Status;
            oldPOPending.SupplierAddress = newPOPending.SupplierAddress;
            oldPOPending.SupplierId = newPOPending.SupplierId;
            oldPOPending.SupplierName = newPOPending.SupplierName;
            oldPOPending.TextLineBreakCount = newPOPending.TextLineBreakCount;
            oldPOPending.ApprovedOn = null;
            oldPOPending.InvoiceDate = newPOPending.InvoiceDate;
            oldPOPending.TermsOfPayment = newPOPending.TermsOfPayment;
            oldPOPending.SIOrBI = newPOPending.SIOrBI;

            var oldPOPendingItems = _context.POPendingItems.Where(p => p.POPendingId == oldPOPending.Id).ToList();
            foreach (var item in oldPOPendingItems)
            {
                _context.POPendingItems.Remove(item);
                _context.Entry(item).State = EntityState.Deleted;
            }

            oldPOPending.POPendingItems = new List<POPendingItem>();
            decimal total = 0;
            var parseItems = JsonConvert.DeserializeObject<NewPOPendingItem[]>(newPOPending.POPendingItems);

          
            foreach (var item in parseItems)
            {
                decimal amount = item.Price * item.Quantity;
                oldPOPending.POPendingItems.Add(new POPendingItem()
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Total = amount,
                    Order = item.Order
                });
                total += amount;
            }

            if (newPOPending.AttachmentIdsToRemove != null)
            {
                foreach (var item in newPOPending.AttachmentIdsToRemove)
                {
                    var a = _context.POAttachments.FirstOrDefault(b => b.Id == item);
                    if (a != null)
                    {
                        _context.POAttachments.Remove(a);
                        _context.Entry(a).State = EntityState.Deleted;
                    }
                }
            }

            if (newPOPending.ExternalAttachmentIdsToRemove != null)
            {
                foreach (var item in newPOPending.ExternalAttachmentIdsToRemove)
                {
                    var a = _context.POExternalAttachments.FirstOrDefault(b => b.Id == item);
                    if (a != null)
                    {
                        _context.POExternalAttachments.Remove(a);
                        _context.Entry(a).State = EntityState.Deleted;
                    }
                }
            }


            if (newPOPending.Files != null)
            {
                foreach (var f in newPOPending.Files)
                {
                    var attachment = new POAttachment()
                    {
                        POId = oldPOPending.Id,
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        attachment.File = fileBytes;
                    }

                    _context.POAttachments.Add(attachment);
                    _context.Entry(attachment).State = EntityState.Added;
                }
            }

            if (newPOPending.ExternalFiles != null)
            {
                foreach (var f in newPOPending.ExternalFiles)
                {
                    var attachment = new POExternalAttachment()
                    {
                        POId = oldPOPending.Id,
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        attachment.File = fileBytes;
                    }

                    _context.POExternalAttachments.Add(attachment);
                    _context.Entry(attachment).State = EntityState.Added;
                }
            }

            decimal discountAmount = Math.Round((oldPOPending.Discount / 100) * total, 2);
            oldPOPending.Total = total - discountAmount;

            _context.POPendings.Update(oldPOPending);
            _context.Entry(oldPOPending).State = EntityState.Modified;


            POAuditTrail pOAuditTrail = new POAuditTrail()
            {
                POPendingId = oldPOPending.Id,
                UserName = newPOPending.ModifiedByName,
                Message = message.ToString(),
                DateAdded = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
            };

            _context.POAuditTrails.Add(pOAuditTrail);
            _context.Entry(pOAuditTrail).State = EntityState.Added;

            _context.SaveChanges();

            return oldPOPending;
        }

        public POPending UpdateDraftPO(EditPOPendingDraft newPOPending, POPending oldPOPending)
        {
            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

            StringBuilder message = new StringBuilder();
            message.AppendFormat(new CultureInfo("en-US"), "Updated the following: ");

            if (oldPOPending.ApproverId != newPOPending.ApproverId)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nApprover from {0} to {1}", oldPOPending.ApproverName, newPOPending.ApproverName);
            }

            if (oldPOPending.SupplierId != newPOPending.SupplierId)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nSupplier from {0} to {1}", oldPOPending.SupplierName, newPOPending.SupplierName);
            }

            if (oldPOPending.ContactPersonId != newPOPending.ContactPersonId)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nContact Person from {0} to {1}", oldPOPending.ContactPersonName, newPOPending.ContactPersonName);
            }

            if (oldPOPending.ReferenceNumber != newPOPending.ReferenceNumber)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nReference Number from {0} to {1}", oldPOPending.ReferenceNumber, newPOPending.ReferenceNumber);
            }

            if (oldPOPending.EstimatedArrival != newPOPending.EstimatedArrival)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nExpected Delivery from {0} to {1}", oldPOPending.EstimatedArrival == null ? "" : ((DateTime)oldPOPending.EstimatedArrival).ToString("MM/dd/yyyy"), ((DateTime)newPOPending.EstimatedArrival).ToString("MM/dd/yyyy"));
            }

            if (oldPOPending.CustomerName != newPOPending.CustomerName)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nCustomer Name from {0} to {1}", oldPOPending.CustomerName, newPOPending.CustomerName);
            }

            if (oldPOPending.Currency != newPOPending.Currency)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nCurrency from {0} to {1}", oldPOPending.Currency, newPOPending.Currency);
            }

            if (oldPOPending.Discount != newPOPending.Discount)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nDiscount from {0} to {1}", oldPOPending.Discount, newPOPending.Discount);
            }

            if (oldPOPending.Remarks != newPOPending.Remarks)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nRemarks from \n{0} \nto \n{1}", oldPOPending.Remarks, newPOPending.Remarks);
            }

            if (oldPOPending.InternalNote != newPOPending.InternalNote)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nInternal Note from \n{0} \nto \n{1}", oldPOPending.InternalNote, newPOPending.InternalNote);
            }

            if (oldPOPending.Status != newPOPending.Status)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nStatus from {0} to {1}", oldPOPending.Status.ToString(), newPOPending.Status.ToString());
            }

            if (oldPOPending.InvoiceDate != newPOPending.InvoiceDate)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nInvoice Date from {0} to {1}",
                    oldPOPending.InvoiceDate == null ? "" : ((DateTime)oldPOPending.InvoiceDate).ToString("MM/dd/yyyy"),
                    newPOPending.InvoiceDate == null ? "" : ((DateTime)newPOPending.InvoiceDate).ToString("MM/dd/yyyy"));
            }

            if (oldPOPending.SIOrBI != newPOPending.SIOrBI)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nS.I# or B.I# from {0} to {1}",
                    oldPOPending.SIOrBI,
                    newPOPending.SIOrBI);
            }

            if (oldPOPending.TermsOfPayment != newPOPending.TermsOfPayment)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\nTerms Of Payment {0} to {1}",
                    oldPOPending.TermsOfPayment,
                    newPOPending.TermsOfPayment);
            }


            oldPOPending.ApproverEmail = newPOPending.ApproverEmail;
            oldPOPending.ApproverId = newPOPending.ApproverId;
            oldPOPending.ApproverName = newPOPending.ApproverName;
            oldPOPending.ApproverJobTitle = newPOPending.ApproverJobTitle;
            oldPOPending.ContactPersonId = newPOPending.ContactPersonId;
            oldPOPending.ContactPersonName = newPOPending.ContactPersonName;
            oldPOPending.Currency = newPOPending.Currency;
            oldPOPending.CustomerName = newPOPending.CustomerName;
            oldPOPending.Discount = newPOPending.Discount;
            oldPOPending.EstimatedArrival = newPOPending.EstimatedArrival;
            oldPOPending.EstimatedArrivalString = newPOPending.EstimatedArrival == null ? "" : ((DateTime)newPOPending.EstimatedArrival).ToString("MM/dd/yyyy");
            oldPOPending.Guid = Guid.NewGuid();
            oldPOPending.InternalNote = newPOPending.InternalNote;
            oldPOPending.POPendingItemsJsonString = newPOPending.POPendingItemsJsonString;
            oldPOPending.ReferenceNumber = newPOPending.ReferenceNumber;
            oldPOPending.Remarks = newPOPending.Remarks;
            oldPOPending.RequestorName = newPOPending.ModifiedByName;
            oldPOPending.RequestorEmail = newPOPending.RequestorEmail;
            oldPOPending.Status = newPOPending.Status;
            oldPOPending.SupplierAddress = newPOPending.SupplierAddress;
            oldPOPending.SupplierId = newPOPending.SupplierId;
            oldPOPending.SupplierName = newPOPending.SupplierName;
            oldPOPending.TextLineBreakCount = newPOPending.TextLineBreakCount;
            oldPOPending.ApprovedOn = null;
            oldPOPending.InvoiceDate = newPOPending.InvoiceDate;
            oldPOPending.TermsOfPayment = newPOPending.TermsOfPayment;
            oldPOPending.SIOrBI = newPOPending.SIOrBI;

            POAuditTrail pOAuditTrail = new POAuditTrail()
            {
                POPendingId = oldPOPending.Id,
                UserName = newPOPending.ModifiedByName,
                Message = message.ToString(),
                DateAdded = now
            };

            decimal total = 0;
            var parseItems = JsonConvert.DeserializeObject<NewPOPendingItem[]>(oldPOPending.POPendingItemsJsonString);
            foreach (var item in parseItems)
            {
                total += item.Quantity * item.Price;
            }


            if (oldPOPending.Total != total)
            {
                message.AppendFormat(new CultureInfo("en-US"), "\n Order Total from {0} to {1}", oldPOPending.Total, total);
            }

            if (newPOPending.AttachmentIdsToRemove != null)
            {

                foreach (var item in newPOPending.AttachmentIdsToRemove)
                {
                    var a = _context.POAttachments.FirstOrDefault(b => b.Id == item);
                    if (a != null)
                    {
                        _context.POAttachments.Remove(a);
                        _context.Entry(a).State = EntityState.Deleted;
                    }
                }
            }


            if (newPOPending.ExternalAttachmentIdsToRemove != null)
            {

                foreach (var item in newPOPending.ExternalAttachmentIdsToRemove)
                {
                    var a = _context.POExternalAttachments.FirstOrDefault(b => b.Id == item);
                    if (a != null)
                    {
                        _context.POExternalAttachments.Remove(a);
                        _context.Entry(a).State = EntityState.Deleted;
                    }
                }
            }


            if (newPOPending.Files != null)
            {
                foreach (var f in newPOPending.Files)
                {
                    var attachment = new POAttachment()
                    {
                        POId = oldPOPending.Id,
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = now
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        attachment.File = fileBytes;
                    }

                    _context.POAttachments.Add(attachment);
                    _context.Entry(attachment).State = EntityState.Added;
                }
            }

            if (newPOPending.ExternalFiles != null)
            {
                foreach (var f in newPOPending.ExternalFiles)
                {
                    var attachment = new POExternalAttachment()
                    {
                        POId = oldPOPending.Id,
                        Name = f.FileName,
                        Size = f.Length,
                        ContentType = f.ContentType,
                        CreatedOn = now
                    };

                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        attachment.File = fileBytes;
                    }

                    _context.POExternalAttachments.Add(attachment);
                    _context.Entry(attachment).State = EntityState.Added;
                }
            }

            decimal discountAmount = Math.Round((oldPOPending.Discount / 100) * total, 2);
            oldPOPending.Total = total - discountAmount;         

            _context.POPendings.Update(oldPOPending);
            _context.Entry(oldPOPending).State = EntityState.Modified;

            _context.POAuditTrails.Add(pOAuditTrail);
            _context.Entry(pOAuditTrail).State = EntityState.Added;

            _context.SaveChanges();

            return oldPOPending;
        }

        public void UpdatePOStatus(POPending pOPending, POStatus pOStatus)
        {
            pOPending.Status = pOStatus;
            _context.POPendings.Update(pOPending);
            _context.Entry(pOPending).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public ICollection<POPending> GetPendingPOs()
        {
            return _context.POPendings.Where(p => (p.ApprovedOn == null && p.Status != POStatus.Draft) || p.Status == POStatus.ForCancellation).ToList();
        }

        public ICollection<POGuidStatus> UpdatePOsStatus(ICollection<POGuidStatus> pOGuidStatuses, ICollection<POPending> pOPendings)
        {
            List<POPending> pOsToUpdate = new List<POPending>();
            List<POGuidStatus> poGuids = new List<POGuidStatus>();
            foreach (var item in pOGuidStatuses)
            {
                var po = pOPendings.FirstOrDefault(p => p.Guid == item.POGuid);
                if (po != null)
                {
                    po.Status = item.POStatus;

                    var audit = new POAuditTrail();
                    audit.DateAdded = item.ModifiedOn;
                    audit.POPendingId = po.Id;
                    audit.UserName = po.ApproverName;

                    switch(item.POStatus)
                    {
                        case POStatus.Approved:
                            audit.Message = "Approved";
                            po.ApprovedOn = item.ModifiedOn;
                            po.HasBeenApproved = true;
                            break;
                        case POStatus.Rejected:
                            audit.Message = "Rejected";
                            break;
                        case POStatus.Cancelled:
                            audit.Message = "Cancelled";
                            po.CancelledOn = item.ModifiedOn;
                            break;
                        case POStatus.CancellationRejected:
                            audit.Message = "Cancellation rejected";
                            break;
                    }

                    pOsToUpdate.Add(po);
                    poGuids.Add(item);

                    _context.POAuditTrails.Add(audit);
                    _context.Entry(audit).State = EntityState.Added;

                    

                }
            }

            foreach (var item in pOsToUpdate)
            {
               

                _context.POPendings.Update(item);
                _context.Entry(item).State = EntityState.Modified;
            }

            _context.SaveChanges();

            return poGuids;

        }

        public UpdatePOStatusResult UpdatePOStatus(UpdatePOStatus updatePOStatus, POPending pO)
        {
            if (updatePOStatus == null || pO == null)
            {
                return null;
            }

            var now = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            var auditTrail = new POAuditTrail()
                {
                    POPendingId = pO.Id,
                    UserName = updatePOStatus.Requestor,
                    DateAdded = now
                };
            switch (updatePOStatus.StatusType.ToUpper())
            {
                case "RECEIVED":
                    auditTrail.Message = "Delivery received";
                    pO.ReceivedOn = now;
                    pO.Status = POStatus.Received;
                    pO.SIOrBI = updatePOStatus.SIOrBI;
                    pO.InvoiceDate = updatePOStatus.InvoiceDate;
                    break;
                case "CANCEL":
                    auditTrail.Message = String.Format("Cancellation request sent to {0}({1}) \nReason: {2}", updatePOStatus.ApproverName, updatePOStatus.ApproverEmail, updatePOStatus.Message);
                    pO.Status = POStatus.ForCancellation;
                    pO.Guid = Guid.NewGuid();
                    pO.CancelledByName = updatePOStatus.ApproverName;
                    pO.Reason = updatePOStatus.Message;
                    pO.LastApprovalRequestByName = updatePOStatus.Requestor;
                    break;
                default:
                    return null;
            }

            _context.POPendings.Update(pO);
            _context.Entry(pO).State = EntityState.Modified;

            _context.POAuditTrails.Add(auditTrail);
            _context.Entry(auditTrail).State = EntityState.Added;

            _context.SaveChanges();


            return new UpdatePOStatusResult()
            {
                pOAuditTrail = auditTrail,
                pO = pO
            };

        }
        public UpdatePOStatusResult UpdatePOSIorBI(UpdatePOStatus updatePOStatus, POPending pO)
        {
            if (updatePOStatus == null || pO == null)
            {
                return null;
            }

            var now = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            var auditTrail = new POAuditTrail()
            {
                POPendingId = pO.Id,
                UserName = updatePOStatus.Requestor,
                DateAdded = now
            };

            auditTrail.Message = "Updated the S.I.# or B.I.# / Invoice Date";
            pO.SIOrBI = updatePOStatus.SIOrBI;
            pO.InvoiceDate = updatePOStatus.InvoiceDate;

            _context.POPendings.Update(pO);
            _context.Entry(pO).State = EntityState.Modified;

            _context.POAuditTrails.Add(auditTrail);
            _context.Entry(auditTrail).State = EntityState.Added;

            _context.SaveChanges();


            return new UpdatePOStatusResult()
            {
                pOAuditTrail = auditTrail,
                pO = pO
            };

        }

        public POPending GetPOWithLogs(int poId)
        {
            var po = _context.POPendings.Where(p => p.Id == poId)
                                .Include(p => p.POPendingItems)
                                .Include(p => p.POAuditTrails)
                                .FirstOrDefault();

            return po;
        }

        public ICollection<POPending> GetPOStatuses(POIds pOIds)
        {
            return _context.POPendings
                     .Where(p => pOIds.POids.Any(p2 => p2 == p.Id))
                     .Select(p => new POPending()
                     {
                         Id = p.Id,
                         ApprovedOn = p.ApprovedOn,
                         CancelledOn = p.CancelledOn,
                         ReceivedOn = p.ReceivedOn
                     })
                     .ToList();
        }
    }
}
