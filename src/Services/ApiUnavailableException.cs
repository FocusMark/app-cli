using System;

namespace FocusMark.App.Cli.Services
{
    [Serializable]
    internal class ApiUnavailableException : Exception
    {
        public ApiUnavailableException(string targetApi) : base($"The {targetApi} API is not available at this time.")
        {
            if (string.IsNullOrWhiteSpace(targetApi))
            {
                throw new ArgumentException("message", nameof(targetApi));
            }

            TargetApi = targetApi;
        }

        public string TargetApi { get; }
    }
}