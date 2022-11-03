using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Entities.POSupplierEntity
{
    public class POSupplier
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public ICollection<POSupplierContactPerson> ContactPersons { get; set; }
    }
}
