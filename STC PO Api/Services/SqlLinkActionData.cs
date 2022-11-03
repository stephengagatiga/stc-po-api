using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POEntity;

namespace STC_PO_Api.Services
{
    public class SqlLinkActionData : ILinkActionData
    {

        private POContext _context;

        public SqlLinkActionData(POContext context)
        {
            _context = context;

        }

        public POLink AddPOLink(int POId, Guid id, POLinkAction pendingAction)
        {
            var link = new POLink()
            {
                Created = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")),
                PendingAction = pendingAction,
                POGuid = id,
                POPendingId = POId,
                Status = POLinkStatus.Valid
            };

            _context.POLinks.Add(link);
            _context.Entry(link).State = EntityState.Added;

            _context.SaveChanges();

            return link;
        }

        public POPending ApproveCancellationPO(POPending po)
        {
            po.CancelledOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            po.Status = POStatus.Cancelled;

            _context.POPendings.Update(po);
            _context.Entry(po).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            // save changes will happen in add audi trail

            return po;
        }

        public POPending ApprovePO(POPending po)
        {
            po.ApprovedOn = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            po.Status = POStatus.Approved;

            _context.POPendings.Update(po);
            _context.Entry(po).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            // save changes will happen in add audi trail

            return po;

        }

        public POLink GetPOLinkByGuid(Guid id)
        {
           return _context.POLinks.Where(p => p.POGuid == id)
                    .Include(p => p.POPending)
                    .ThenInclude(i => i.POPendingItems)
                    .FirstOrDefault();
        }

        public POPending RejectCancellationPO(POPending po)
        {
            po.CancelledOn = null;
            po.Status = POStatus.CancellationRejected;

            _context.POPendings.Update(po);
            _context.Entry(po).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            // save changes will happen in add audi trail

            return po;
        }

        public POPending RejectPO(POPending po)
        {
            po.ApprovedOn = null;
            po.Status = POStatus.Rejected;

            _context.POPendings.Update(po);
            _context.Entry(po).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            // save changes will happen in add audi trail

            return po;


        }

        public POLink UpdatePOLink(POLink link, POLinkAction responseAction)
        {
            link.Status = POLinkStatus.Expired;
            link.ResponseAction = responseAction;
            link.ResponseDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

            _context.POLinks.Update(link);
            _context.Entry(link).State = EntityState.Modified;

            //save changes hapen in add audit trail

            return link;
        }
    }
}
