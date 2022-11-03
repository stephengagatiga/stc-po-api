using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IPOAuditTrailData
    {
        POAuditTrail AddAuditTrail(NewAuditTrail newAuditTrail);
        ICollection<POAuditTrail> GetPOAuditTrails(int pOId);
        void SendApprovalAuditLog(int pOId, string userName, string name, string email);
        void UpdatePOAuditLog(POPending oldPO, POPending newPO, string modifiedByName);

    }
}
