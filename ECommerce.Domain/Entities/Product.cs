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

    public Product(
        string name,
        string description,
        decimal price,
        string pictureUrl,
        Guid productBrandId,
        Guid productTypeId)
    {
        SetName(name);
        SetDescription(description);
        ChangePrice(price);
        ChangePicture(pictureUrl);
        ChangeBrand(productBrandId);
        ChangeType(productTypeId);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        name = name.Trim();

        if (name.Length > MaxNameLength)
            throw new ArgumentException(
                $"Product name cannot exceed {MaxNameLength} characters.",
                nameof(name));

        Name = name;
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        description = description.Trim();

        if (description.Length > MaxDescriptionLength)
            throw new ArgumentException(
                $"Description cannot exceed {MaxDescriptionLength} characters.",
                nameof(description));

        Description = description;
    }

    private void ChangePicture(string pictureUrl)
    {
        if (string.IsNullOrWhiteSpace(pictureUrl))
            throw new ArgumentException("Picture URL is required.", nameof(pictureUrl));

        pictureUrl = pictureUrl.Trim();

        if (pictureUrl.Length > MaxPictureUrlLength)
            throw new ArgumentException(
                $"Picture URL cannot exceed {MaxPictureUrlLength} characters.",
                nameof(pictureUrl));

        PictureUrl = pictureUrl;
    }

    private void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(newPrice),
                "Price must be greater than zero.");

        Price = decimal.Round(newPrice, 2);
    }

    private void ChangeBrand(Guid brandId)
    {
        if (brandId == Guid.Empty)
            throw new ArgumentException("Product brand is required.", nameof(brandId));

        ProductBrandId = brandId;
    }

    private void ChangeType(Guid typeId)
    {
        if (typeId == Guid.Empty)
            throw new ArgumentException("Product type is required.", nameof(typeId));

        ProductTypeId = typeId;
    }
}