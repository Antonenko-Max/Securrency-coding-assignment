namespace Securrency.TDS.Web.Infrastructure.ClientRetryPolicy
{
    public class TimeoutPolicyOptions
    {
        public int TimeoutMilliseconds { get; set; } = 10000;
    }
}
