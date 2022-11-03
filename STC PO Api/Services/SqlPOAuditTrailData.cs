using Microsoft.EntityFrameworkCore;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public class SqlPOAuditTrailData : IPOAuditTrailData
    {

        private POContext _context;

        public SqlPOAuditTrailData(POContext context)
        {
            _context = context;
        }

        public POAuditTrail AddAuditTrail(NewAuditTrail newAuditTrail)
        {
            POAuditTrail pOAuditTrail = new POAuditTrail()
            {
                POPendingId = newAuditTrail.POPendingId,
                UserName = newAuditTrail.UserName,
                Message = newAuditTrail.Message,
                DateAdded = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
            };

            _context.POAuditTrails.Add(pOAuditTrail);
            _context.Entry(pOAuditTrail).State = EntityState.Added;

            _context.SaveChanges();

            return pOAuditTrail;
        }

        public ICollection<POAuditTrail> GetPOAuditTrails(int pOId)
        {
            return _context.POAuditTrails
                                .Where(a => a.POPendingId == pOId)
                                .OrderByDescending(a => a.DateAdded)
                                .ToList();
        }

        public void SendApprovalAuditLog(int pOId, string userName, string name, string email)
        {
            string message = String.Format(new CultureInfo("en-US"), @"Sent approval to {0} ({1})", name, email);
            POAuditTrail pOAuditTrail = new POAuditTrail()
            {
                POPendingId = pOId,
                UserName = userName,
                Message = message,
                DateAdded = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
            };

            _context.POAuditTrails.Add(pOAuditTrail);
            _context.Entry(pOAuditTrail).State = EntityState.Added;

            _context.SaveChanges();
        }

        public void UpdatePOAuditLog(POPending oldPO, POPending newPO, string modifiedByName)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat(new CultureInfo("en-US"), "Updated the following: ");

            if (oldPO.ApproverId != newPO.ApproverId)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\nApprover from {0} to {1}", oldPO.ApproverName, newPO.ApproverName);
            }

            if (oldPO.SupplierId != newPO.SupplierId)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\nSupplier from {0} to {1}", oldPO.SupplierName, newPO.SupplierName);
            }

            if (oldPO.ContactPersonId != newPO.ContactPersonId)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\nContact Person from {0} to {1}", oldPO.ContactPersonName, newPO.ContactPersonName);
            }

            if (oldPO.ReferenceNumber != newPO.ReferenceNumber)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\nReference Number from {0} to {1}", oldPO.ReferenceNumber, newPO.ReferenceNumber);
            }

            if (oldPO.EstimatedArrival != newPO.EstimatedArrival)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\nExpected Delivery from {0} to {1}", oldPO.EstimatedArrivalString, newPO.EstimatedArrivalString);
            }

            if (oldPO.CustomerName.Trim() != newPO.CustomerName.Trim())
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\nCustomer Name from {0} to {1}", oldPO.CustomerName, newPO.CustomerName);
            }

            if (oldPO.Currency != newPO.Currency)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\nCurrency from {0} to {1}", oldPO.Currency, newPO.Currency);
            }

            if (oldPO.Discount != newPO.Discount)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\Discount from {0} to {1}", oldPO.Discount, newPO.Discount);
            }

            if (oldPO.Status != newPO.Status)
            {
                message.AppendFormat(new CultureInfo("en-US"), @"\Status from {0} to {1}", oldPO.Status.ToString(), newPO.Status.ToString());
            }

            POAuditTrail pOAuditTrail = new POAuditTrail()
            {
                POPendingId = oldPO.Id,
                UserName = modifiedByName,
                Message = message.ToString(),
                DateAdded = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"))
            };

            _context.POAuditTrails.Add(pOAuditTrail);
            _context.Entry(pOAuditTrail).State = EntityState.Added;

            _context.SaveChanges();
        }
    }
}
