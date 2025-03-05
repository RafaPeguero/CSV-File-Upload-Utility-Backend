

using System;
using System.Collections.Generic;
using CSV_File_Upload_Utility_Backend.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csv_File_Upload_Utility_Test;

[TestClass]
public class SalesOrderTests
{
    [TestMethod]
    public void SalesOrder_WhenCreatedWithValidData_ShouldBeValid()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            {"OrderNumber", "12345" },
            {"OrderDate", "1/3/2025"},
            {"ShipToName", "Zach"},
            {"ShipToCompany", "Zach's Company"},
            {"ShipToAddress1", "123 Main St"},
            {"ShipToCity", "New York"},
            {"ShipToState", "NY"},
            {"ShipToPostalCode", "10001"},
            {"ShipToCountry", "USA"},
            {"Sku", "SKU-001"},
            {"Quantity", "5"},
            {"RequestedWarehouse", "Warehouse 1"},
            {"DeliveryInstructions", "Leave at the door"}
        };
            
        // Act
        var order = new SalesOrder(data);
        var validationErrors = order.Validate();
            
        // Assert
         Assert.AreEqual("12345", order.OrderNumber);
         Assert.AreEqual("Zach", order.ShipToName);
         Assert.AreEqual("Zach's Company", order.ShipToCompany);
         Assert.AreEqual("123 Main St", order.ShipToAddress1);
         Assert.AreEqual("New York", order.ShipToCity);
         Assert.AreEqual("NY", order.ShipToState);
         Assert.AreEqual("10001", order.ShipToPostalCode);
         Assert.AreEqual("USA", order.ShipToCountry);
         Assert.AreEqual("SKU-001", order.Sku);
         Assert.AreEqual(5, order.Quantity);
         Assert.AreEqual("Warehouse 1", order.RequestedWarehouse);
         Assert.AreEqual("Leave at the door", order.DeliveryInstructions);
         Assert.AreEqual(new DateTime(2025, 3, 1), order.OrderDate);
         Assert.IsTrue(validationErrors.Count == 0);
    }
    [TestMethod]
    public void SalesOrder_WhenCreatedWithInvalidData_ShouldBeInvalid()
    {
        // Arrange
        var data = new Dictionary<string, string>
        {
            {"OrderNumber", "" },
            {"OrderDate", "3/1/2025"},
            {"ShipToName", "Zach"},
            {"ShipToCompany", "Zach's Company"},
            {"ShipToAddress1", "123 Main St"},
            {"ShipToCity", "New York"},
            {"ShipToState", "NY"},
            {"ShipToPostalCode", "10001"},
            {"ShipToCountry", "USA"},
            {"Sku", ""},
            {"Quantity", ""},
            {"RequestedWarehouse", "Warehouse 1"},
            {"DeliveryInstructions", "Leave at the door"}
        };
            
        // Act
        var order = new SalesOrder(data);
        var validationErrors = order.Validate();
            
        // Assert
        Assert.IsTrue(validationErrors.Count > 0);
        Assert.IsTrue(validationErrors.Contains("Order ID is required"));
        Assert.IsTrue(validationErrors.Contains("Quantity must be a positive number"));
        Assert.IsTrue(validationErrors.Contains("Sku is required"));
    }
}