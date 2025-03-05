using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CSV_File_Upload_Utility_Backend.Controllers;
using CSV_File_Upload_Utility_Backend.Interfaces;
using CSV_File_Upload_Utility_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Csv_file_upload_utility_test;

[TestClass]
public class SalesControllerTest
{
    [TestClass]
    public class SalesOrderControllerTests
    {
        private Mock<IFileParseService> _mockFileParser;
        private Mock<ISalesOrderService> _mockSalesOrderService;
        private SalesOrderController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockFileParser = new Mock<IFileParseService>();
            _mockSalesOrderService = new Mock<ISalesOrderService>();
            _controller = new SalesOrderController(_mockFileParser.Object, _mockSalesOrderService.Object);
        }

        [TestMethod]
        public async Task UploadSalesData_WithValidFile_ReturnsSuccess()
        {
            // Arrange
            var data = new Dictionary<string, string>
            {
                { "OrderNumber", "12345" },
                { "OrderDate", "3/1/2025" },
                { "ShipToName", "Zach" },
                { "ShipToCompany", "Zach's Company" },
                { "ShipToAddress1", "123 Main St" },
                { "ShipToCity", "New York" },
                { "ShipToState", "NY" },
                { "ShipToPostalCode", "10001" },
                { "ShipToCountry", "USA" },
                { "Sku", "SKU-001" },
                { "Quantity", "5" },
                { "RequestedWarehouse", "Warehouse 1" },
                { "DeliveryInstructions", "Leave at the door" }
            };
            var salesOrders = new List<SalesOrder>
            {
                new(data)
            };

            _mockFileParser
                .Setup(x => x.ParseFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(salesOrders);

            _mockFileParser
                .Setup(x => x.ValidateSalesOrdersAsync(It.IsAny<List<SalesOrder>>()))
                .ReturnsAsync((salesOrders, new List<ValidationError>()));

            _mockSalesOrderService
                .Setup(x => x.SaveBulkAsync(It.IsAny<List<SalesOrder>>()))
                .ReturnsAsync((1, new List<SaveError>()));

            // Create mock file
            var fileMock = CreateMockFile(data.ToString() ?? throw new InvalidOperationException(), "test.csv");

            // Act
            var result = await _controller.UploadSalesData(fileMock);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.IsNotNull(okResult.Value);

            // Check the actual response
            var response = okResult.Value as ApiResponse<UploadSummary>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(1, response.Data.TotalRecords);
        }

        [TestMethod]
        public async Task UploadSalesData_WithNoFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadSalesData(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
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
}