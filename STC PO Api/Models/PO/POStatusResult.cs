using STC_PO_Api.Entities.POEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class POStatusResult
    {
        public Guid Guid { get; set; }
        public POStatus POStatus { get; set; }
    }
}
