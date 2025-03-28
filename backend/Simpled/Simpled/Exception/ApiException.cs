namespace Simpled.Exception
{
    public class ApiException : System.Exception
    {
        public int StatusCode { get; }

        public ApiException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
