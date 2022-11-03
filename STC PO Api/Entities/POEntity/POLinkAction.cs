using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POEntity
{
    public enum POLinkAction
    {
        None,
        ForApproval,
        Approved,
        ForCancellation,
        Cancelled,
        Rejected,
        CancellationRejected
    }
}
