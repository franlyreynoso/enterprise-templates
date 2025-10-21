namespace EnterpriseTemplate.Infrastructure.Resilience;

/// <summary>
/// Configuration options for resilience policies
/// </summary>
public class ResilienceOptions
{
    public const string SectionName = "Resilience";

    /// <summary>
    /// Retry policy configuration
    /// </summary>
    public RetryOptions Retry { get; set; } = new();

    /// <summary>
    /// Circuit breaker policy configuration
    /// </summary>
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();

    /// <summary>
    /// Timeout policy configuration
    /// </summary>
    public TimeoutOptions Timeout { get; set; } = new();

    public class RetryOptions
    {
        /// <summary>
        /// Maximum number of retry attempts
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Base delay between retries in milliseconds
        /// </summary>
        public int BaseDelayMs { get; set; } = 1000;

        /// <summary>
        /// Maximum delay between retries in milliseconds
        /// </summary>
        public int MaxDelayMs { get; set; } = 30000;

        /// <summary>
        /// Whether to use exponential backoff
        /// </summary>
        public bool UseExponentialBackoff { get; set; } = true;

        /// <summary>
        /// Whether to add jitter to prevent thundering herd
        /// </summary>
        public bool UseJitter { get; set; } = true;
    }

    public class CircuitBreakerOptions
    {
        /// <summary>
        /// Number of consecutive failures before opening the circuit
        /// </summary>
        public int HandledEventsAllowedBeforeBreaking { get; set; } = 5;

        /// <summary>
        /// Duration to keep circuit open in seconds
        /// </summary>
        public int DurationOfBreakSeconds { get; set; } = 30;

        /// <summary>
        /// Minimum number of actions through circuit before failure statistics are considered
        /// </summary>
        public int MinimumThroughput { get; set; } = 10;

        /// <summary>
        /// Time window for failure rate calculation in seconds
        /// </summary>
        public int SamplingDurationSeconds { get; set; } = 60;

        /// <summary>
        /// Failure rate threshold (0.0 to 1.0) that triggers circuit breaker
        /// </summary>
        public double FailureRateThreshold { get; set; } = 0.5; // 50%
    }

    public class TimeoutOptions
    {
        /// <summary>
        /// Default timeout for operations in seconds
        /// </summary>
        public int DefaultTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Timeout for external API calls in seconds
        /// </summary>
        public int ExternalApiTimeoutSeconds { get; set; } = 10;

        /// <summary>
        /// Timeout for database operations in seconds
        /// </summary>
        public int DatabaseTimeoutSeconds { get; set; } = 15;
    }
}
