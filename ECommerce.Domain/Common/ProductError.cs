
namespace ECommerce.Domain.Common;

public static class ProductErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Product.NotFound", "Product was not found");

    public static readonly Error NameRequired =
        Error.Validation("Product.NameRequired", "Product name is required");

    public static readonly Error DescriptionRequired =
        Error.Validation("Product.DescriptionRequired", "Product description is required");

    public static readonly Error PictureUrlRequired =
        Error.Validation("Product.PictureUrlRequired", "Product picture URL is required");

    public static readonly Error InvalidPrice =
        Error.Validation("Product.InvalidPrice", "Product price cannot be negative");

    public static readonly Error ProductBrandRequired =
        Error.Validation("Product.ProductBrandRequired", "Product brand is required");

    public static readonly Error ProductTypeRequired =
        Error.Validation("Product.ProductTypeRequired", "Product type is required");

    public static readonly Error AlreadyExists =
        Error.Conflict("Product.AlreadyExists", "Product already exists");

    public static readonly Error CreateFailed =
        Error.Failure("Product.CreateFailed", "Product could not be created");

    public static readonly Error UpdateFailed =
        Error.Failure("Product.UpdateFailed", "Product could not be updated");

    public static readonly Error DeleteFailed =
        Error.Failure("Product.DeleteFailed", "Product could not be deleted");
}