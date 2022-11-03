using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;

namespace STC_PO_Api.Services
{
    public class SqlPOAttachment : IPOAttachmentData
    {
        private POContext _context;

        public SqlPOAttachment(POContext context)
        {
            _context = context;
        }

        public POAttachment GetPOAttachment(int poId, string fileName)
        {
            throw new NotImplementedException();
        }

        public ICollection<GetPOAttachmentsResult> GetPOAttachments(int poId)
        {
            return _context.POAttachments.Where(p => p.POId == poId).Select(p => new GetPOAttachmentsResult()
            {
                Id = p.Id,
                Name = p.Name,
                POId = p.POId,
                Size = p.Size,
                ContentType = p.ContentType
            }).ToList();
        }

        public ICollection<POAttachment> GetPOAttachmentsData(int pOId)
        {
            return _context.POAttachments.Where(p => p.POId == pOId).ToList();
        }

        public ICollection<POAttachment> GetPOAttachmentsFile(int pOId)
        {
            return _context.POAttachments.Where(p => p.POId == pOId).ToList();
        }
    }
}
