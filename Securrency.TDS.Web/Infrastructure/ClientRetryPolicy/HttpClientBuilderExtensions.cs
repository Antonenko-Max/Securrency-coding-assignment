using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Securrency.TDS.Web.Infrastructure.ClientRetryPolicy
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddDefaultPolicies(this IHttpClientBuilder httpClientBuilder)
        {
            var options = new HttpClientOptions();
            return httpClientBuilder.AddDefaultPolicies(options);
        }

        public static IHttpClientBuilder AddDefaultPolicies(this IHttpClientBuilder httpClientBuilder,
            HttpClientOptions options)
        {
            return httpClientBuilder.AddRetryPolicy(options.RetryPolicy)
                .AddCircuitBreakerPolicy(options.CircuitBreakerPolicy)
                .AddTimeoutPolicy(options.TimeoutPolicy);
        }

        private static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder httpClientBuilder,
            RetryPolicyOptions options)
        {
            return httpClientBuilder.AddPolicyHandler(
                HttpPolicyExtensions.HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .Or<HttpRequestException>()
                    .WaitAndRetryAsync(
                        options.RetryCount,
                        options.SleepDurationProvider(options.RetryCount, TimeSpan.FromMilliseconds(options.MedianFirstRetryDelayMilliseconds)),
                        options.OnRetry));
        }

        private static IHttpClientBuilder AddCircuitBreakerPolicy(this IHttpClientBuilder httpClientBuilder,
            CircuitBreakerPolicyOptions options)
        {
            return httpClientBuilder.AddPolicyHandler(
                HttpPolicyExtensions.HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .Or<HttpRequestException>()
                    .OrResult(m => m.StatusCode == HttpStatusCode.TooManyRequests)
                    .CircuitBreakerAsync(
                        options.HandledEventsAllowedBeforeBreaking,
                        TimeSpan.FromSeconds(options.DurationOfBreakSeconds),
                        options.OnBreak,
                        options.OnReset,
                        options.OnHalfOpen));
        }

        private static IHttpClientBuilder AddTimeoutPolicy(this IHttpClientBuilder httpClientBuilder,
            TimeoutPolicyOptions options)
        {
            return httpClientBuilder.AddPolicyHandler(
                Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(options.TimeoutMilliseconds)));
        }
    }
}
