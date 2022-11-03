using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Models.PO
{
    public class NewPOSupplier
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public ICollection<NewPOContactPerson> ContactPersons { get; set; }
    }
}
