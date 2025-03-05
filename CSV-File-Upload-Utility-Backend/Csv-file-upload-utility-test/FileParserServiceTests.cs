using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CSV_File_Upload_Utility_Backend.Models;
using CSV_File_Upload_Utility_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Csv_file_upload_utility_test;
[TestClass]
public class FileParserServiceTests
{
     private FileParserService _parserService;
        
        [TestInitialize]
        public void Setup()
        {
            _parserService = new FileParserService();
        }
        
        [TestMethod]
        public async Task ParseFileAsync_WithCsvFile_ReturnsSalesOrders()
        {
            // Arrange
            var csvContent = "OrderNumber,ShipToName,Quantity\n123,Test Customer,5";
            var mockFile = CreateMockFile(csvContent, "test.csv");
            
            // Act
            var result = await _parserService.ParseFileAsync(mockFile);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("123", result[0].OrderNumber);
            Assert.AreEqual("Test Customer", result[0].ShipToName);
            Assert.AreEqual(5, result[0].Quantity);
        }
        
        [TestMethod]
        public async Task ValidateSalesOrdersAsync_WithMixedData_ReturnsValidAndInvalidOrders()
        {
            // Arrange
            var orders = new List<SalesOrder>
            {
                new()
                { 
                    OrderNumber = "Valid-1", 
                    OrderDate = new DateTime(2025, 3, 1),
                    ShipToName = "Customer", 
                    ShipToCompany= "Zach",
                    ShipToAddress1 = "Address",
                    ShipToCity = "City",
                    ShipToState = "ST",
                    ShipToPostalCode = "12345",
                    ShipToCountry = "US",
                    Sku = "Sku",
                    Quantity = 1,
                    RequestedWarehouse = "Warehouse"
                },
                new()
                { 
                    OrderNumber = "", 
                    ShipToName = "Customer", 
                    ShipToAddress1 = "Address",
                    ShipToCity = "",
                    ShipToState = "ST",
                    ShipToPostalCode = "12345",
                    ShipToCountry = "US",
                    Quantity = -2,
                    RequestedWarehouse = "Warehouse"
                }
            };
            
            // Act
            var (validOrders, invalidOrders) = await _parserService.ValidateSalesOrdersAsync(orders);
            
            // Assert
            Assert.AreEqual(1, validOrders.Count);
            Assert.AreEqual(1, invalidOrders.Count);
            Assert.AreEqual("Valid-1", validOrders[0].OrderNumber);
        }
        
        private IFormFile CreateMockFile(string content, string fileName)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.UTF8);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            
            return fileMock.Object;
        }
}