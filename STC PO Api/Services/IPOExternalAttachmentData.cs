using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IPOExternalAttachmentData
    {
        ICollection<GetPOAttachmentsResult> GetPOExternalAttachments(int poId);
        ICollection<POExternalAttachment> GetPOExternalAttachmentsData(int pOId);
    }
}
