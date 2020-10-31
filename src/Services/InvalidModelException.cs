using FocusMark.App.Cli.Models;
using System;
using System.Runtime.Serialization;

namespace FocusMark.App.Cli.Services
{
    [Serializable]
    internal class InvalidModelException<T> : Exception
    {
        private ApiResponse<T> apiResponse;

        public InvalidModelException()
        {
        }

        public InvalidModelException(ApiResponse<T> apiResponse)
        {
            this.apiResponse = apiResponse;
        }

        public InvalidModelException(string message) : base(message)
        {
        }

        public InvalidModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidModelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}