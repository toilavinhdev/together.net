using System.Net;

namespace Infrastructure.SharedKernel.Exceptions;

public sealed class TogetherException : Exception
{
    public HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
    
    public string Code { get; }
    
    public string? Parameter { get; }

    public TogetherException(string code) : base(code)
    {
        Code = code;
    }

    public TogetherException(string code, 
        string? parameter = null, 
        HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(code)
    {
        Code = code;
        Parameter = parameter;
        StatusCode = statusCode;
    }
}