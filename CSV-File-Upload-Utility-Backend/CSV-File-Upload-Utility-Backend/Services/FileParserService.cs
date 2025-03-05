using System.Globalization;
using System.Text;
using CSV_File_Upload_Utility_Backend.Interfaces;
using CSV_File_Upload_Utility_Backend.Models;
using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;

namespace CSV_File_Upload_Utility_Backend.Services;

public class FileParserService : IFileParseService
{
    public FileParserService()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    
    public async Task<List<SalesOrder>> ParseFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return fileExtension switch
        {
            ".csv" => await ParseCsvAsync(file),
            ".xlsx" or ".xls" => await ParseExcelAsync(file),
            _ => throw new ArgumentException("Unsupported file format. Please upload CSV or Excel files only.")
        };
    }
     private async Task<List<SalesOrder>> ParseCsvAsync(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<dynamic>().ToList();

            return records.Select(record => new SalesOrder(record)).ToList();
        }

        private async Task<List<SalesOrder>> ParseExcelAsync(IFormFile file)
        {
            var salesOrders = new List<SalesOrder>();

            await using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();
            var table = result.Tables[0];
            
            var headers = new List<string>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                headers.Add(table.Rows[0][i].ToString());
            }
            
            for (int rowIndex = 1; rowIndex < table.Rows.Count; rowIndex++)
            {
                var row = table.Rows[rowIndex];
                var recordData = new Dictionary<string, string>();
                
                for (int colIndex = 0; colIndex < headers.Count; colIndex++)
                {
                    if (colIndex < table.Columns.Count)
                    {
                        recordData[headers[colIndex]] = row[colIndex]?.ToString() ?? string.Empty;
                    }
                }
                        
                salesOrders.Add(new SalesOrder(recordData));
            }

            return salesOrders;
        }

    public async Task<(List<SalesOrder> ValidOrders, List<ValidationError> InvalidOrders)> ValidateSalesOrdersAsync(List<SalesOrder> orders)
    {
        var validOrders = new List<SalesOrder>();
        var invalidOrders = new List<ValidationError>();
            
        foreach (var order in orders)
        {
            var errors = order.Validate();
                
            if (errors.Count == 0)
            {
                validOrders.Add(order);
            }
            else
            {
                invalidOrders.Add(new ValidationError
                {
                    Order = order,
                    Errors = errors
                });
            }
        }
            
        return (validOrders, invalidOrders);
    }
}