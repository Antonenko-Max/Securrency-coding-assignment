namespace Securrency.TDS.Web.Infrastructure.ClientRetryPolicy
{
    public class HttpClientOptions
    {
        public RetryPolicyOptions RetryPolicy { get; set; } = new RetryPolicyOptions();

        public CircuitBreakerPolicyOptions CircuitBreakerPolicy { get; set; } = new CircuitBreakerPolicyOptions();

        public TimeoutPolicyOptions TimeoutPolicy { get; set; } = new TimeoutPolicyOptions();
    }
}
