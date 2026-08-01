using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxPictureUrlLength = 50;

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string PictureUrl { get; private set; } = null!;
    public decimal Price { get; private set; }

    public Guid ProductBrandId { get; private set; }
    public ProductBrand ProductBrand { get; private set; } = null!;

    public Guid ProductTypeId { get; private set; }
    public ProductType ProductType { get; private set; } = null!;

    private Product()
    {
    }

    private Product(
        string name,
        string description,
        decimal price,
        string pictureUrl,
        Guid productBrandId,
        Guid productTypeId)
    {
        Name = name;
        Description = description;
        Price = price;
        PictureUrl = pictureUrl;
        ProductBrandId = productBrandId;
        ProductTypeId = productTypeId;
    }

    public static Result<Product> Create(
        string name,
        string description,
        decimal price,
        string pictureUrl,
        Guid productBrandId,
        Guid productTypeId)
    {
        var product = new Product();

        var nameResult = product.SetName(name);
        if (nameResult.IsFailure) return Result<Product>.Failure(nameResult.Error!);

        var descResult = product.SetDescription(description);
        if (descResult.IsFailure) return Result<Product>.Failure(descResult.Error!);

        var priceResult = product.ChangePrice(price);
        if (priceResult.IsFailure) return Result<Product>.Failure(priceResult.Error!);

        var picResult = product.ChangePicture(pictureUrl);
        if (picResult.IsFailure) return Result<Product>.Failure(picResult.Error!);

        var brandResult = product.ChangeBrand(productBrandId);
        if (brandResult.IsFailure) return Result<Product>.Failure(brandResult.Error!);

        var typeResult = product.ChangeType(productTypeId);
        if (typeResult.IsFailure) return Result<Product>.Failure(typeResult.Error!);

        return Result<Product>.Success(product);
    }

    public Result SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ProductErrors.NameRequired);

        name = name.Trim();

        if (name.Length > MaxNameLength)
            return Result.Failure(Error.Validation(
                "Product.NameTooLong",
                $"Product name cannot exceed {MaxNameLength} characters."));

        Name = name;
        return Result.Success();
    }

    public Result SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(ProductErrors.DescriptionRequired);

        description = description.Trim();

        if (description.Length > MaxDescriptionLength)
            return Result.Failure(Error.Validation(
               "Product.DescriptionTooLong",
               $"Description cannot exceed {MaxDescriptionLength} characters."));

        Description = description;
        return Result.Success();
    }

    public Result ChangePicture(string pictureUrl)
    {
        if (string.IsNullOrWhiteSpace(pictureUrl))
            return Result.Failure(ProductErrors.PictureUrlRequired);

        pictureUrl = pictureUrl.Trim();

        if (pictureUrl.Length > MaxPictureUrlLength)
            return Result.Failure(Error.Validation(
               "Product.PictureUrlTooLong",
               $"Picture URL cannot exceed {MaxPictureUrlLength} characters."));

        PictureUrl = pictureUrl;
        return Result.Success();
    }

    public Result ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            return Result.Failure(ProductErrors.InvalidPrice);

        Price = decimal.Round(newPrice, 2);
        return Result.Success();
    }

    public Result ChangeBrand(Guid brandId)
    {
        if (brandId == Guid.Empty)
            return Result.Failure(ProductErrors.ProductBrandRequired);

        ProductBrandId = brandId;
        return Result.Success();
    }

    public Result ChangeType(Guid typeId)
    {
        if (typeId == Guid.Empty)
            return Result.Failure(ProductErrors.ProductTypeRequired);

        ProductTypeId = typeId;
        return Result.Success();
    }
}