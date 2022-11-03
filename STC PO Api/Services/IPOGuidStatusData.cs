using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IPOGuidStatusData
    {
        POGuidStatus AddPOGuid(POPending pO, ICollection<POAttachment> pOAttachments, string[] sendTOs);
        ICollection<POGuidStatus> GetPOStatus(ICollection<POPending> POs);
        void DeletePOStatus(ICollection<POGuidStatus> pOGuidStatusess);
        void DeleteUpdatedPO(ICollection<POGuidStatus> pOGuidStatuses);
    }
}
