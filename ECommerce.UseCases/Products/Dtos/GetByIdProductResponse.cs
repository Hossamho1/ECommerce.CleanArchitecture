using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Products.Dtos;

public record GetByIdProductResponse(

    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string PictureUrl,
    string ProductBrand,
    string ProductType

    );
