namespace CSV_File_Upload_Utility_Backend.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public class UploadSummary
{
    public int TotalRecords { get; set; }
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
    public int SavedRecords { get; set; }
    public int ErrorCount { get; set; }
    public List<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();
    public List<SaveError> SaveErrors { get; set; } = new List<SaveError>();
}

public class ValidationError
{
    public SalesOrder Order { get; set; }
    public List<string> Errors { get; set; }
}

public class SaveError
{
    public string OrderId { get; set; }
    public string Error { get; set; }
}