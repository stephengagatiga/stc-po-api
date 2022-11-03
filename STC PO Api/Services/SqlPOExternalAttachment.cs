using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;

namespace STC_PO_Api.Services
{
    public class SqlPOExternalAttachment : IPOExternalAttachmentData
    {
        private POContext _context;

        public SqlPOExternalAttachment(POContext context)
        {
            _context = context;
        }

        public ICollection<GetPOAttachmentsResult> GetPOExternalAttachments(int poId)
        {
            var attachments =  _context.POExternalAttachments.Where(p => p.POId == poId).Select(p => new GetPOAttachmentsResult()
            {
                Id = p.Id,
                Name = p.Name,
                POId = p.POId,
                Size = p.Size,
                ContentType = p.ContentType
            }).ToList();

            return attachments;
        }

        public ICollection<POExternalAttachment> GetPOExternalAttachmentsData(int pOId)
        {
            return _context.POExternalAttachments.Where(p => p.POId == pOId).ToList();
        }
    }
}
