using STC_PO_Api.Entities.POApproverEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IPOApproverData
    {
        POApprover AddPOApprover(NewPOApprover newPOApprover);
        POApprover UpdatePOApprover(EditPOApprover editPOApprover, POApprover oldApprover);
        POApprover GetPOApprover(int id);
        void DeleteApprover(POApprover approver);
        ICollection<POApprover> GetApprovers();
    }
}
