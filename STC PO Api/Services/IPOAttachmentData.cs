using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IPOAttachmentData
    {
        ICollection<GetPOAttachmentsResult> GetPOAttachments(int poId);
        ICollection<POAttachment> GetPOAttachmentsData(int pOId);
        ICollection<POAttachment> GetPOAttachmentsFile(int pOId);

        POAttachment GetPOAttachment(int poId, string fileName);
    }
}
