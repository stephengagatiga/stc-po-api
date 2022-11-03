using STC_PO_Api.Entities.POEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface ILinkActionData
    {
        POPending ApprovePO(POPending po);
        POPending RejectPO(POPending po);
        POPending ApproveCancellationPO(POPending po);
        POPending RejectCancellationPO(POPending po);
        POLink GetPOLinkByGuid(Guid id);
        POLink UpdatePOLink(POLink link, POLinkAction responseAction);
        POLink AddPOLink(int POId, Guid id, POLinkAction pendingAction);
    }
}
