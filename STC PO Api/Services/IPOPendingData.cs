using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IPOPendingData
    {
        POPending AddPO(NewPOPending pOPending);
        POPending AddPODraft(NewPOPendingDraft pOPendingDraft);
        POPending GetPO(int poId);
        POPending GetPOWithLogs(int poId);
        POPending GetPOByGuid(Guid guid);
        void UpdatePOStatus(POPending pOPending, POStatus pOStatus);
        POPending UpdatePO(EditPOPending newPOPending, POPending oldPOPending);
        POPending UpdateDraftPO(EditPOPendingDraft newPOPending, POPending oldPOPending);
        ICollection<POPending> GetPOs(int skip, int take);
        ICollection<POPending> SearchPOs(string keyword, string searchType);
        ICollection<POPending> GetPendingPOs();
        ICollection<POGuidStatus> UpdatePOsStatus(ICollection<POGuidStatus> pOGuidStatuses, ICollection<POPending> pOPendings);
        UpdatePOStatusResult UpdatePOStatus(UpdatePOStatus updatePOStatus, POPending pO);
        UpdatePOStatusResult UpdatePOSIorBI(UpdatePOStatus updatePOStatus, POPending pO);
        ICollection<POPending> GetPOStatuses(POIds pOIds);

    }
}
