using Microsoft.EntityFrameworkCore;
using STC_PO_Api.Entities.POEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Context
{
    public class POGuidStatusContext : DbContext
    {
        public POGuidStatusContext(DbContextOptions<POGuidStatusContext> options) : base(options)
        {

        }

        public DbSet<POGuidStatus> POGuidStatus { get; set; }
        public DbSet<POGuidStatusAttachment> POGuidStatusAttachments { get; set; }

    }
}
