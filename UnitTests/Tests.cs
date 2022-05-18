using System;
using System.Collections.Generic;
using System.Linq;
using DataCreation;
using FluentAssertions;
using Xunit;

namespace UnitTests;

public class Tests
{

    private readonly NorthwindActions _northwindActions;

    public Tests()
    {
        _northwindActions = new NorthwindActions();
    }

    [Fact]
    public void T01_NumberOfKeyIsCorrect()
    {
        _northwindActions.QuantityListPerCustomerDictionary();
        _northwindActions.quantitysDictionary.Count.Should().Be(8);
    }

    [Theory]
    [InlineData("COMMI", 1, 9, 4, 9, 4, 8, 8, 5, 5, 3)]
    public void T02_QuantityListPerCustomer_Quantities(string customer, params int[] quantities)
    {
        _northwindActions.QuantityListPerCustomerDictionary();
        _northwindActions.quantitysDictionary[customer].ToArray().Should().BeEquivalentTo(quantities);
    }

    [Fact]
    public void T03_NumberOfDatesIsCorrect()
    {
        _northwindActions.ProductsPerDayDictionary();
        _northwindActions.dateProductDictionary.Count.Should().Be(3);
    }
    
    [Fact]
    public void T04_ProductsSorted()
    {
        _northwindActions.ProductsPerDayDictionary();
        var list = new List<string>()
        {
            "Chang","Sasquatch Ale","Perth Pasties", "Ravioli Angelo","Chang","Chai", "Longlife Tofu", "Tofu", "Chai",
            "Outback Lager", "Chai", "Filo Mix", "Chocolade", "Valkoinen suklaa", "Sasquatch Ale", "Mishi Kobe Niku", "Chang",
            "Tarte au sucre"
        }.Distinct().OrderBy(x => x).ToList();
        _northwindActions.dateProductDictionary[DateOnly.Parse("07.12.2018")].Should().BeEquivalentTo(list);
    }
    
    [Theory]
    [InlineData("Davolio", 1, 12, 16, 7, 9, 6, 7, 5, 9, 19, 4, 8, 8, 5, 5, 3, 7)]
    public void T05_TotalsPerProductAndEmployee(string employee, params int[] total)
    {
        _northwindActions.TotalsPerProductAndEmployee();
        _northwindActions.totalsPerEmployeeDictionary
            .Where(x => x.Key == employee)
            .SelectMany(x => x.Value.Values)
            .ToList().Should().BeEquivalentTo(total.ToList());
    }
    
    [Theory]
    [InlineData("Leverling", "Chai", 14)]
    public void T06_TotalsPerProductAndEmployee(string employee, string product, int total)
    {
        _northwindActions.TotalsPerProductAndEmployee();
        _northwindActions.totalsPerEmployeeDictionary[employee][product].Should().Be(total);
    }

    [Fact]
    public void T07_InsertProducts()
    {
        _northwindActions.InitData();
        int nrCategories = 8;
        int nrProducts = 77;
        int nrSuppliers = 29;
        _northwindActions.InsertProducts();
        _northwindActions.db.Categories.Count().Should().Be(nrCategories+2);
        _northwindActions.db.Suppliers.Count().Should().Be(nrSuppliers);
        _northwindActions.db.Products.Count().Should().Be(nrProducts+8);
        _northwindActions.Dispose();
    }
    
    
}