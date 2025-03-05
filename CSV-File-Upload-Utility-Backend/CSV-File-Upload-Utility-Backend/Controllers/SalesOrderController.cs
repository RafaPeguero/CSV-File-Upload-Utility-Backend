using CSV_File_Upload_Utility_Backend.Interfaces;
using CSV_File_Upload_Utility_Backend.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CSV_File_Upload_Utility_Backend.Controllers;
[ApiController]
[Route("api")]
[EnableCors("CorsPolicy")]
public class SalesOrderController(
    IFileParseService fileParserService,
    ISalesOrderService salesOrderService)
    : ControllerBase
{
    [HttpPost("upload-sales-data")]
    public async Task<IActionResult> UploadSalesData(IFormFile file)
    {
        try
        {
            if ( file is null || file.Length == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No file uploaded"
                });
            }

            var salesOrders = await fileParserService.ParseFileAsync(file);
            
            var (validOrders, invalidOrders) = await fileParserService.ValidateSalesOrdersAsync(salesOrders);
            
            var (successCount, saveErrors) = await salesOrderService.SaveBulkAsync(validOrders);
            
            var summary = new UploadSummary
            {
                TotalRecords = salesOrders.Count,
                ValidRecords = validOrders.Count,
                InvalidRecords = invalidOrders.Count,
                SavedRecords = successCount,
                ErrorCount = saveErrors.Count,
                ValidationErrors = invalidOrders,
                SaveErrors = saveErrors
            };
            
            return Ok(new ApiResponse<UploadSummary>
            {
                Success = true,
                Message = "File processed successfully",
                Data = summary
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = "Error processing upload",
                Data = ex.Message
            });
        }
    }
    
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(int limit = 100)
    {
        try
        {
            var orders = await salesOrderService.GetAllOrdersAsync(limit);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Retrieved {orders.Count} orders",
                Data = orders
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = "Error retrieving orders",
                Data = ex.Message
            });
        }
    }
    
    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderById(string orderId)
    {
        try
        {
            var order = await salesOrderService.GetOrderByIdAsync(orderId);
            
            if (order == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Order with ID {orderId} not found"
                });
            }
            
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Order retrieved successfully",
                Data = order
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = "Error retrieving order",
                Data = ex.Message
            });
        }
    }
}