using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class GetPOAttachmentsResult
    {
        public int Id { get; set; }
        public int POId { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public IActionResult File { get; set; }
    }
}
