using CSV_File_Upload_Utility_Backend.Models;

namespace CSV_File_Upload_Utility_Backend.Interfaces;

public interface ISalesOrderService
{
    Task<(int SuccessCount, List<SaveError> Errors)> SaveBulkAsync(List<SalesOrder> orders);
    Task<SalesOrder?> GetOrderByIdAsync(string orderId);
    Task<List<SalesOrder>> GetAllOrdersAsync(int limit = 100);
}