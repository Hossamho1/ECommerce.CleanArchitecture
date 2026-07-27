using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities;

public class ProductBrand : BaseEntity
{
    public string Name { get; private set; } = null!;
     public ICollection<Product> Product { get; private set; } = [];


}
