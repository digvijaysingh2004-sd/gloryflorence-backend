using System.Collections.Generic;

namespace GloryFlorence.API.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Request processed successfully.")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> FailureResponse(IEnumerable<string> errors, string message = "One or more errors occurred.")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }

        public static ApiResponse<T> FailureResponse(string error, string message = "An error occurred.")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = new[] { error }
            };
        }
    }
}
