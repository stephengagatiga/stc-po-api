﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using STC_PO_Api.Context;

#nullable disable

namespace STC_PO_Api.Migrations
{
    [DbContext(typeof(POContext))]
    partial class POContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.HasSequence<int>("PONumbers", "shared")
                .StartsAt(37700L);

            modelBuilder.Entity("STC_PO_Api.Entities.POApproverEntity.POApprover", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("AmountEUR")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("AmountLogicalOperatorEUR")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AmountLogicalOperatorPHP")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AmountLogicalOperatorUSD")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("AmountPHP")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("AmountUSD")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("SignatureImg")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.ToTable("POApprovers");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ContentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("File")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("POId")
                        .HasColumnType("int");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("POId");

                    b.ToTable("POAttachments");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POAuditTrail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("POPendingId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("POPendingId");

                    b.ToTable("POAuditTrails");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POExternalAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ContentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("File")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("POId")
                        .HasColumnType("int");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("POId");

                    b.ToTable("POExternalAttachments");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("POGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("POPendingId")
                        .HasColumnType("int");

                    b.Property<int>("PendingAction")
                        .HasColumnType("int");

                    b.Property<int>("ResponseAction")
                        .HasColumnType("int");

                    b.Property<DateTime>("ResponseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("POPendingId");

                    b.ToTable("POLinks");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POPending", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("ApprovedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("ApproverEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ApproverId")
                        .HasColumnType("int");

                    b.Property<string>("ApproverJobTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApproverName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CancelledByName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CancelledOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ContactPersonId")
                        .HasColumnType("int");

                    b.Property<string>("ContactPersonName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CreatedById")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Currency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("EstimatedArrival")
                        .HasColumnType("datetime2");

                    b.Property<string>("EstimatedArrivalString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("HasBeenApproved")
                        .HasColumnType("bit");

                    b.Property<string>("InternalNote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("InvoiceDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastApprovalRequestByName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("NEXT VALUE FOR shared.PONumbers");

                    b.Property<string>("POPendingItemsJsonString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReceivedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReferenceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestorEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SIOrBI")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("SupplierAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SupplierId")
                        .HasColumnType("int");

                    b.Property<string>("SupplierName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TermsOfPayment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextLineBreakCount")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("TrackerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("POPendings");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POPendingItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("POPendingId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("POPendingId");

                    b.ToTable("POPendingItems");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POSupplierEntity.POSupplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("POSuppliers");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POSupplierEntity.POSupplierContactPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("POSupplierId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("POSupplierId");

                    b.ToTable("POSupplierContactPersons");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POAttachment", b =>
                {
                    b.HasOne("STC_PO_Api.Entities.POEntity.POPending", "POPending")
                        .WithMany("POAttachments")
                        .HasForeignKey("POId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("POPending");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POAuditTrail", b =>
                {
                    b.HasOne("STC_PO_Api.Entities.POEntity.POPending", "POPending")
                        .WithMany("POAuditTrails")
                        .HasForeignKey("POPendingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("POPending");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POExternalAttachment", b =>
                {
                    b.HasOne("STC_PO_Api.Entities.POEntity.POPending", "POPending")
                        .WithMany("POExternalAttachments")
                        .HasForeignKey("POId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("POPending");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POLink", b =>
                {
                    b.HasOne("STC_PO_Api.Entities.POEntity.POPending", "POPending")
                        .WithMany()
                        .HasForeignKey("POPendingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("POPending");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POPendingItem", b =>
                {
                    b.HasOne("STC_PO_Api.Entities.POEntity.POPending", "POPending")
                        .WithMany("POPendingItems")
                        .HasForeignKey("POPendingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("POPending");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POSupplierEntity.POSupplierContactPerson", b =>
                {
                    b.HasOne("STC_PO_Api.Entities.POSupplierEntity.POSupplier", null)
                        .WithMany("ContactPersons")
                        .HasForeignKey("POSupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POEntity.POPending", b =>
                {
                    b.Navigation("POAttachments");

                    b.Navigation("POAuditTrails");

                    b.Navigation("POExternalAttachments");

                    b.Navigation("POPendingItems");
                });

            modelBuilder.Entity("STC_PO_Api.Entities.POSupplierEntity.POSupplier", b =>
                {
                    b.Navigation("ContactPersons");
                });
#pragma warning restore 612, 618
        }
    }
}
