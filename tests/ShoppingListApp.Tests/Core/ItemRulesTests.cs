using Xunit;
using ShoppingListApp.Core.Validation;

namespace ShoppingListApp.Tests.Core;

public class ItemRulesTests
{
    // Validate Name Tests
    [Fact]
    public void ValidateName_ShouldReturnError_WhenNameIsNullOrEmpty()
    {        
        Assert.Equal("Name must be between 1 and 100 characters.", ItemRules.ValidateName(null));
        Assert.Equal("Name must be between 1 and 100 characters.", ItemRules.ValidateName(""));
        Assert.Equal("Name must be between 1 and 100 characters.", ItemRules.ValidateName(" "));
    }

    [Fact]
    public void ValidateName_ShouldReturnError_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', ItemRules.NameMaxLength + 1);
        Assert.Equal($"Name must be between 1 and {ItemRules.NameMaxLength} characters.", ItemRules.ValidateName(longName));
    }
    
    [Fact]
    public void ValidateName_ShouldReturnEmpty_WhenNameIsAtMaxLength()
    {
        var name = new string('a', ItemRules.NameMaxLength);
        Assert.Equal("", ItemRules.ValidateName(name));
    }

    // Validate Price Tests
    [Fact]
    public void ValidatePrice_ShouldReturnError_WhenPriceIsNegativeOrExceedsMax()
    {
        Assert.Equal($"Price must be between 0 and {ItemRules.MaxPrice}.", ItemRules.ValidatePrice(-1));
        Assert.Equal($"Price must be between 0 and {ItemRules.MaxPrice}.", ItemRules.ValidatePrice(ItemRules.MaxPrice + 1));
    }

    [Fact]
    public void ValidatePrice_ShouldReturnEmpty_WhenPriceIsWithinValidRange()
    {
        Assert.Equal("", ItemRules.ValidatePrice(0));
        Assert.Equal("", ItemRules.ValidatePrice(ItemRules.MaxPrice));
    }

    // Validate Spending Limit Tests
    [Fact]
    public void ValidateSpendingLimit_ShouldReturnError_WhenSpendingLimitIsNegativeOrExceedsMax()
    {
        Assert.Equal($"Spending limit must be between 0 and {ItemRules.MaxSpendingLimit}.", ItemRules.ValidateSpendingLimit(-1));
        Assert.Equal($"Spending limit must be between 0 and {ItemRules.MaxSpendingLimit}.", ItemRules.ValidateSpendingLimit(ItemRules.MaxSpendingLimit + 1));
    }

    [Fact]
    public void ValidateSpendingLimit_ShouldReturnEmpty_WhenSpendingLimitIsWithinValidRange()
    {
        Assert.Equal("", ItemRules.ValidateSpendingLimit(0));
        Assert.Equal("", ItemRules.ValidateSpendingLimit(ItemRules.MaxSpendingLimit));
    }

}