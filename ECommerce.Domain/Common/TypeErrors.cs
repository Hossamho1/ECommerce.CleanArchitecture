namespace ECommerce.Domain.Common;

public static class TypeErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Type.NotFound", "Type was not found");
}
