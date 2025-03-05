using CSV_File_Upload_Utility_Backend.Models;

namespace CSV_File_Upload_Utility_Backend.Interfaces;

public interface IFileParseService
{
    Task<List<SalesOrder>> ParseFileAsync(IFormFile file);
    Task<(List<SalesOrder> ValidOrders, List<ValidationError> InvalidOrders)> ValidateSalesOrdersAsync(List<SalesOrder> orders);
}