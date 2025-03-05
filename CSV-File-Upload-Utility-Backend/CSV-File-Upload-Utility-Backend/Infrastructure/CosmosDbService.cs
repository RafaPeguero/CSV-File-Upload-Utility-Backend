using CSV_File_Upload_Utility_Backend.Interfaces;
using CSV_File_Upload_Utility_Backend.Models;
using Microsoft.Azure.Cosmos;

namespace CSV_File_Upload_Utility_Backend.Infrastructure;

public class CosmosDbService(CosmosClient cosmosClient, string databaseName, string containerName)
    : ISalesOrderService
{
    private readonly List<SalesOrder> _salesOrders = [];
    
    private readonly Container _container = cosmosClient.GetContainer(databaseName, containerName);


    public async Task<(int SuccessCount, List<SaveError> Errors)> SaveBulkAsync(List<SalesOrder> orders)
    {
        var errors = new List<SaveError>();
        var successCount = 0;

        foreach (var order in orders)
        {
            try
            {
                await _container.UpsertItemAsync(order, new PartitionKey(order.Id));
                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new SaveError 
                { 
                    OrderId = order.Id, 
                    Error = ex.Message 
                });
            }
        }

        return (successCount, errors);
    }

    public async Task<SalesOrder> GetOrderByIdAsync(string orderId)
    {
        try
        {
            var response = await _container.ReadItemAsync<SalesOrder>(
                orderId, new PartitionKey(orderId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new SalesOrder();
        }
    }

    public async Task<List<SalesOrder>> GetAllOrdersAsync(int limit = 100)
    {
        var query = _container.GetItemQueryIterator<SalesOrder>(
            new QueryDefinition("SELECT * FROM c WHERE c.type = 'salesOrder' ORDER BY c.orderDate DESC OFFSET 0 LIMIT @limit")
                .WithParameter("@limit", limit));

        var results = new List<SalesOrder>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }
}