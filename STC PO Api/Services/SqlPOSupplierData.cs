using Microsoft.EntityFrameworkCore;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POSupplierEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public class SqlPOSupplierData : IPOSupplierData
    {
        private POContext _context;

        public SqlPOSupplierData(POContext context)
        {
            _context = context;
        }

        public POSupplier AddPOSupplier(NewPOSupplier newPOSupplier)
        {
            if (newPOSupplier == null)
            {
                return null;
            }

            var supplier = new POSupplier
            {
                Name = newPOSupplier.Name,
                Address = newPOSupplier.Address
            };

            supplier.ContactPersons = new List<POSupplierContactPerson>();

            foreach (var item in newPOSupplier.ContactPersons)
            {
                supplier.ContactPersons.Add(new POSupplierContactPerson
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName
                });
            }

            _context.POSuppliers.Add(supplier);
            _context.Entry(supplier).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            _context.SaveChanges();

            return supplier;

        }

        public POSupplier GetPOSupplier(int supplierId)
        {
            var supplier = _context.POSuppliers.FirstOrDefault(s => s.Id == supplierId);
            return supplier;
        }

        public ICollection<POSupplier> GetSuppliers()
        {
            return _context.POSuppliers
                        .OrderBy(s => s.Name)
                        .Include(s => s.ContactPersons)
                        .ToList();
        }

        public POSupplier UpdatePOSupplier(POSupplier supplier, EditPOSupllier editPOSupplier)
        {
            if (editPOSupplier == null || supplier == null)
            {
                return null;
            }

            supplier.Name = editPOSupplier.Name;
            supplier.Address = editPOSupplier.Address;

            foreach (var item in editPOSupplier.ContactPersons)
            { 
                switch(item.Type)
                {
                    case "ADD":
                        POSupplierContactPerson cp = new POSupplierContactPerson
                        {
                            POSupplierId = supplier.Id,
                            FirstName = item.FirstName,
                            LastName = item.LastName
                        };
                        _context.POSupplierContactPersons.Add(cp);
                        _context.Entry(cp).State = EntityState.Added;
                        break;
                    case "EDIT":
                        var cpEdit = _context.POSupplierContactPersons.FirstOrDefault(c => c.Id == item.Id);
                        cpEdit.FirstName = item.FirstName;
                        cpEdit.LastName = item.LastName;
                        _context.POSupplierContactPersons.Update(cpEdit);
                        _context.Entry(cpEdit).State = EntityState.Modified;
                        break;
                    case "DELETE":
                        var cpRemove = _context.POSupplierContactPersons.FirstOrDefault(c => c.Id == item.Id);
                        _context.POSupplierContactPersons.Remove(cpRemove);
                        _context.Entry(cpRemove).State = EntityState.Deleted;
                        break;
                }
            }

            _context.POSuppliers.Update(supplier);
            _context.Entry(supplier).State = EntityState.Modified;

            _context.SaveChanges();

            var newSupplier = _context.POSuppliers
                                    .Include(s => s.ContactPersons)
                                    .FirstOrDefault(s => s.Id == supplier.Id);

            return newSupplier;
        }
    }
}
