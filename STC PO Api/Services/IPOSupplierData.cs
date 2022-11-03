using STC_PO_Api.Entities.POSupplierEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IPOSupplierData
    {
        POSupplier AddPOSupplier(NewPOSupplier newPOSupplier);
        POSupplier UpdatePOSupplier(POSupplier supplier, EditPOSupllier editPOSupplier);
        POSupplier GetPOSupplier(int supplierId);
        ICollection<POSupplier> GetSuppliers();
    }
}
