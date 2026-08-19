using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ECommerce.Domain.Entities;

public class Basket
{
    public Guid BuyerId { get; set; }


    public List<BasketItem> Items { get; private set; } = [];
    [JsonConstructor]
    private Basket(Guid buyerId, List<BasketItem> items)
    {
        BuyerId = buyerId;
        Items = items;
    }
    private Basket(Guid buyerId)
    {
        BuyerId = buyerId;
        Items = [];
    }

    public static Result<Basket> CreateEmpty(Guid buyerId)
    {
        if (buyerId == Guid.Empty)
            return Result<Basket>.Failure(Errors.BasketErrors.InvalidBuyerId);
        var basket = new Basket(buyerId);
        return Result<Basket>.Success(basket);
    }

    public int TotalItems => Items.Sum(i => i.Quantity);

    public decimal SubTotal => Items.Sum(item => item.LineTotal);

    public Result AddItem(Guid productId, string productName, string pictureUrl,
    decimal unitPrice, int quantity)
    {
        var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem is not null)
            return existingItem.IncreaseQuantity(quantity);

        var createResult = BasketItem.Create(productId, productName, pictureUrl, unitPrice, quantity);

        if (createResult.IsFailure)
            return Result.Failure(createResult.Error);

        Items.Add(createResult.Value);

        return Result.Success();
    }

    public Result DeleteItem(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure(Errors.BasketErrors.BasketItemNotFound);

        Items.Remove(item);
        return Result.Success();
    }


    
    public Result UpdateItemQuantity(Guid productId, int newQuantity)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure(Errors.BasketErrors.BasketItemNotFound);
       return item.SetQuantity(newQuantity);
    }

    public void Clear()
    {
        Items.Clear();
    }
    public Result MergeFrom(Basket other)
    {
        if (other.BuyerId == BuyerId)
            return Result.Failure(Errors.BasketErrors.CannotMergeSameBuyer);

        foreach (var item in other.Items)
        {
            var mergeResult = AddItem(item.ProductId, item.ProductName, item.PictureUrl,
                item.UnitPrice, item.Quantity);

            if (mergeResult.IsFailure)
                return mergeResult;
        }

        return Result.Success();
    }

}
