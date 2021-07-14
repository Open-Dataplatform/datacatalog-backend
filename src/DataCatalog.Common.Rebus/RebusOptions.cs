namespace DataCatalog.Common.Rebus
{
    public class RebusOptions
    {
        public static string AttemptNumberKey => "attemptNumber";
        public int MaxAttempts { get; set; }
        public int RetryInMinutes { get; set; }
    }
}