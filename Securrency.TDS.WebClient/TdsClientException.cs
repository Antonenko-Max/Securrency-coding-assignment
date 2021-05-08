using System;
using System.Net.Http;

namespace Securrency.TDS.WebClient
{
    public sealed class TdsClientException: ApplicationException
    {
        public TdsClientException(string message, HttpResponseMessage details)
            : base(message)
        {
            this.Details = details;
        }

        public HttpResponseMessage Details { get; }
    }
}