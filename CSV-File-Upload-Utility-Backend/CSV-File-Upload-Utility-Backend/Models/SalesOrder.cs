using Newtonsoft.Json;

namespace CSV_File_Upload_Utility_Backend.Models;

public class SalesOrder
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    
    [JsonProperty(PropertyName = "orderNumber")]
    public string OrderNumber { get; set; }

    [JsonProperty(PropertyName = "alternateOrderNumber")]
    public string AlternateOrderNumber { get; set; }

    [JsonProperty(PropertyName = "orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonProperty(PropertyName = "shipToName")]
    public string ShipToName { get; set; }

    [JsonProperty(PropertyName = "shipToCompany")]
    public string ShipToCompany { get; set; }

    [JsonProperty(PropertyName = "shipToAddress1")]
    public string ShipToAddress1 { get; set; }

    [JsonProperty(PropertyName = "shipToAddress2")]
    public string ShipToAddress2 { get; set; }

    [JsonProperty(PropertyName = "shipToAddress3")]
    public string ShipToAddress3 { get; set; }

    [JsonProperty(PropertyName = "shipToCity")]
    public string ShipToCity { get; set; }

    [JsonProperty(PropertyName = "shipToState")]
    public string ShipToState { get; set; }

    [JsonProperty(PropertyName = "shipToPostalCode")]
    public string ShipToPostalCode { get; set; }

    [JsonProperty(PropertyName = "shipToCountry")]
    public string ShipToCountry { get; set; }

    [JsonProperty(PropertyName = "shipToPhone")]
    public string ShipToPhone { get; set; }

    [JsonProperty(PropertyName = "sku")]
    public string Sku { get; set; }

    [JsonProperty(PropertyName = "quantity")]
    public int Quantity { get; set; }

    [JsonProperty(PropertyName = "requestedWarehouse")]
    public string RequestedWarehouse { get; set; }

    [JsonProperty(PropertyName = "deliveryInstructions")]
    public string DeliveryInstructions { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public string Tags { get; set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; } = "salesOrder";

    public SalesOrder () {}
    public SalesOrder(dynamic data)
    {
        OrderNumber = NormalizeString(GetPropertyValue(data, "OrderNumber", "OrderNumber*"));
        AlternateOrderNumber = NormalizeString(GetPropertyValue(data, "AlternateOrderNumber", "AlternateOrderNumber*"));
        
        var dateStr = GetPropertyValue(data, "OrderDate", "OrderDate*");
        OrderDate = DateTime.TryParse(dateStr, out DateTime date) ? date : DateTime.Now;
        
        ShipToName = NormalizeString(GetPropertyValue(data, "ShipToName", "ShipToName*"));
        ShipToCompany = NormalizeString(GetPropertyValue(data, "ShipToCompany"));
        ShipToAddress1 = NormalizeString(GetPropertyValue(data, "ShipToAddress1", "ShipToAddress1*"));
        ShipToAddress2 = NormalizeString(GetPropertyValue(data, "ShipToAddress2"));
        ShipToAddress3 = NormalizeString(GetPropertyValue(data, "ShipToAddress3"));
        ShipToCity = NormalizeString(GetPropertyValue(data, "ShipToCity", "ShipToCity*"));
        ShipToState = NormalizeString(GetPropertyValue(data, "ShipToState", "ShipToState*"));
        ShipToPostalCode = NormalizeString(GetPropertyValue(data, "ShipToPostalCode", "ShipToPostalCode*"));
        ShipToCountry = NormalizeString(GetPropertyValue(data, "ShipToCountry", "ShipToCountry*"));
        ShipToPhone = NormalizeString(GetPropertyValue(data, "ShipToPhone"));
        Sku = NormalizeString(GetPropertyValue(data, "Sku", "Sku*"));
        
        var qtyStr = GetPropertyValue(data, "Quantity", "Quantity*");
        Quantity = int.TryParse(qtyStr, out int qty) ? qty : 0;
        
        RequestedWarehouse = NormalizeString(GetPropertyValue(data, "RequestedWarehouse", "RequestedWarehouse*"));
        DeliveryInstructions = NormalizeString(GetPropertyValue(data, "DeliveryInstructions"));
        Tags = NormalizeString(GetPropertyValue(data, "Tags"));
        Id = OrderNumber;
    }
    
    private string GetPropertyValue(dynamic data, params string[] possibleNames)
    {
        if (data == null)
            return string.Empty;

        foreach (var name in possibleNames)
        {
            // Try dictionary access
            string dictValue = TryGetFromDictionary(data, name);
            if (!string.IsNullOrEmpty(dictValue))
                return dictValue;
                
            // Try indexer access
            string indexerValue = TryGetFromIndexer(data, name);
            if (!string.IsNullOrEmpty(indexerValue))
                return indexerValue;
                
            // Try property access
            string propertyValue = TryGetFromProperty(data, name);
            if (!string.IsNullOrEmpty(propertyValue))
                return propertyValue;
                
            // Try dynamic access
            string dynamicValue = TryGetFromDynamic(data, name);
            if (!string.IsNullOrEmpty(dynamicValue))
                return dynamicValue;
        }
        
        return string.Empty;
    }

    private static string TryGetFromDictionary(dynamic data, string name)
    {
        try
        {
            if (data is IDictionary<string, object> dict)
            {
                if (dict.ContainsKey(name))
                    return dict[name]?.ToString() ?? string.Empty;
                
                foreach (var key in dict.Keys)
                {
                    if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
                        return dict[key]?.ToString() ?? string.Empty;
                }
            }
        }
        catch
        {
            // Silently continue to the next method
        }
        
        return string.Empty;
    }

    private string TryGetFromIndexer(dynamic data, string name)
    {
        try
        {
            var type = data.GetType();
            var indexerProperty = type.GetProperty("Item");
            if (indexerProperty != null)
            {
                var value = indexerProperty.GetValue(data, new object[] { name });
                return value?.ToString() ?? string.Empty;
            }
        }
        catch
        {
            // Silently continue to the next method
        }
        
        return string.Empty;
    }

    private string TryGetFromProperty(dynamic data, string name)
    {
        try
        {
            var property = data.GetType().GetProperty(name);
            if (property != null)
            {
                var value = property.GetValue(data, null);
                return value?.ToString() ?? string.Empty;
            }
        }
        catch
        {
            // Silently continue to the next method
        }
        
        return string.Empty;
    }

    private static string TryGetFromDynamic(dynamic data, string name)
    {
        try
        {
            var result = data[name];
            return result?.ToString() ?? string.Empty;
        }
        catch
        {
            // Silently continue
        }
        
        return string.Empty;
    }
    private static string NormalizeString(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }
    
    public List<string> Validate()
    {
        var errors = new List<string>();
            
        if (string.IsNullOrWhiteSpace(OrderNumber))
            errors.Add("Order ID is required");
                
        if (string.IsNullOrWhiteSpace(ShipToName))
            errors.Add("Customer name is required");
                
        if (string.IsNullOrWhiteSpace(ShipToAddress1))
            errors.Add("Customer Address is required");
        
        if (string.IsNullOrWhiteSpace(ShipToCity))
            errors.Add("City is required");
        
        if (string.IsNullOrWhiteSpace(ShipToState))
            errors.Add("State is required");
        
        if (string.IsNullOrWhiteSpace(ShipToPostalCode))
            errors.Add("Postal Code is required");
        
        if (string.IsNullOrWhiteSpace(ShipToCountry))
            errors.Add("Country is required");
        
        if (string.IsNullOrWhiteSpace(Sku))
            errors.Add("Sku is required");
                
        if (Quantity <= 0)
            errors.Add("Quantity must be a positive number");
        
        if (string.IsNullOrWhiteSpace(RequestedWarehouse))
            errors.Add("Requested Warehouse is required");
            
        return errors;
    }
}