using System;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;


namespace Securrency.TDS.Web.Infrastructure.ClientRetryPolicy
{
    public class RetryPolicyOptions
    {
        public int RetryCount { get; set; } = 2;

        public int MedianFirstRetryDelayMilliseconds { get; set; } = 1000;

        public Func<int, TimeSpan, Func<int, TimeSpan>> SleepDurationProvider { get; set; } =
            (retryCount, medianFirstRetryDelay) => retryAttempt =>
                Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ElementAt(retryAttempt - 1);

        public Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> OnRetry { get; set; } =
            (_, _, _, _) => { };
    }
}
