using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Models
{
    public class ProductImage:BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        public int ProductId{ get; set; }
        public Product Product { get; set; }
    }
}
