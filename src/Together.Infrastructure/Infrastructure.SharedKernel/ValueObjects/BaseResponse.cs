using System.Net;
using System.Text.Json.Serialization;

namespace Infrastructure.SharedKernel.ValueObjects;

public class BaseResponse
{
    public HttpStatusCode StatusCode { get; set; }
    
    public bool Success => (int)StatusCode >= 200 && (int)StatusCode <= 299;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public List<BaseError>? Errors { get; set; }
}

public class BaseResponse<T> : BaseResponse
{
    public T Data { get; set; } = default!;
}

public class BaseError(string code, string message, string? parameter = null)
{
    public string Code { get; set; } = code;

    public string Message { get; set; } = message;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Parameter { get; set; } = parameter;
}