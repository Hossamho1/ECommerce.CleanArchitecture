namespace ECommerce.Domain.Common;

public static class BrandErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Brand.NotFound", "Brand was not found");
}
