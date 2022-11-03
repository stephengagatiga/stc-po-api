using Microsoft.EntityFrameworkCore;
using STC_PO_Api.Entities.POApproverEntity;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Entities.POSupplierEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Context
{
    public class POContext : DbContext
    {
        public POContext(DbContextOptions<POContext> options) : base(options)
        {

        }

        public DbSet<POPendingItem> POPendingItems { get; set; }
        public DbSet<POPending> POPendings { get; set; }
        public DbSet<POAuditTrail> POAuditTrails { get; set; }
        public DbSet<POApprover> POApprovers { get; set; }
        public DbSet<POSupplier> POSuppliers { get; set; }
        public DbSet<POSupplierContactPerson> POSupplierContactPersons { get; set; }
        public DbSet<POAttachment> POAttachments { get; set; }
        public DbSet<POExternalAttachment> POExternalAttachments { get; set; }
        public DbSet<POLink> POLinks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasSequence<int>("PONumbers", schema: "shared")
               .StartsAt(37700)
               .IsCyclic(false)
               .IncrementsBy(1);

            modelBuilder.Entity<POPendingItem>()
              .HasOne(o => o.POPending)
              .WithMany(x => x.POPendingItems)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<POAuditTrail>()
                .HasOne(o => o.POPending)
                .WithMany(x => x.POAuditTrails)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<POPending>()
                .Property(o => o.OrderNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.PONumbers");


        }
    }
}
