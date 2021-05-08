using System;
using System.Net.Http;
using Polly;
using Polly.CircuitBreaker;

namespace Securrency.TDS.Web.Infrastructure.ClientRetryPolicy
{
    public class CircuitBreakerPolicyOptions
    {
        public int HandledEventsAllowedBeforeBreaking { get; set; } = 5;

        public int DurationOfBreakSeconds { get; set; } = 10000;

        public Action<DelegateResult<HttpResponseMessage>, CircuitState, TimeSpan, Context> OnBreak { get; set; } =
            (_, _, _, _) => { };

        public Action<Context> OnReset { get; set; } = _ => { };

        public Action OnHalfOpen { get; set; } = () => { };

    }
}
