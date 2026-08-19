using ECommerce.Domain.Common;

namespace ECommerce.Domain.Errors;

public static class BasketErrors
{
    public static readonly Error GuestBuyerIdRequired =
        Error.Validation(
            "Basket.GuestBuyerIdRequired",
            "Guest shoppers must send the X-Buyer-Id header with a client-generated id.");

    public static readonly Error AuthenticatedBuyerIdMissing =
        Error.Validation(
            "Basket.AuthenticatedBuyerIdMissing",
            "The user id claim is missing or invalid in the authentication token.");

    public static readonly Error InvalidBuyerId =
        Error.Validation(
            "Basket.InvalidBuyerId",
            "A valid buyer id is required.");

    public static readonly Error InvalidProductId =
        Error.Validation(
            "Basket.InvalidProductId",
            "Product id is required.");

    public static readonly Error ProductNotFound =
        Error.Validation(
            "Basket.ProductNotFound",
            "The specified product was not found.");

    public static readonly Error InvalidQuantity =
        Error.Validation(
            "Basket.InvalidQuantity",
            "Quantity must be between 1 and 99.");

    public static readonly Error QuantityTooLow =
        Error.Validation(
            "Basket.QuantityTooLow",
            "Basket item quantity must be at least 1.");

    public static readonly Error QuantityTooHigh =
        Error.Validation(
            "Basket.QuantityTooHigh",
            "Basket item quantity cannot exceed 99.");

    public static readonly Error InvalidProductName =
        Error.Validation(
            "Basket.InvalidProductName",
            "Product name is required.");

    public static readonly Error InvalidPictureUrl =
        Error.Validation(
            "Basket.InvalidPictureUrl",
            "Product picture URL is required.");

    public static readonly Error InvalidUnitPrice =
        Error.Validation(
            "Basket.InvalidUnitPrice",
            "Product unit price must be greater than or equal to zero.");

    public static readonly Error BasketItemNotFound =
        Error.Validation(
            "Basket.BasketItemNotFound",
            "The specified basket item was not found.");

    public static readonly Error CannotMergeSameBuyer =
        Error.Validation(
            "Basket.CannotMergeSameBuyer",
            "Cannot merge baskets belonging to the same buyer.");
}
