using System.Net;

namespace DataCatalog.Api.Data.Response
{
    public abstract class BaseResponse
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }
        public int HttpStatusCode { get; set; }

        public BaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public BaseResponse(bool success, string message, HttpStatusCode httpStatusCode)
        {
            Success = success;
            Message = message;
            HttpStatusCode = (int)httpStatusCode;
        }
    }
}
