using ECommerce.Application.Products.Dtos;
using ECommerce.Application.Brands.Dtos;
using ECommerce.Application.Types.Dtos;
using ECommerce.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product,GetByIdProductResponse>()
            .Map(dest => dest.ProductBrand, src => src.ProductBrand.Name)
            .Map(dest => dest.ProductType, src => src.ProductType.Name);

        config.NewConfig<ProductBrand, GetAllBrandResponse>();
        config.NewConfig<ProductBrand, GetByIdBrandResponse>();
        config.NewConfig<ProductType, GetAllTypeResponse>();
        config.NewConfig<ProductType, GetByIdTypeResponse>();
    }
}
