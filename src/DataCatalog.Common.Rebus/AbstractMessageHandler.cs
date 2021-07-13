using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Serilog.Context;

namespace DataCatalog.Common.Rebus
{
    public abstract class AbstractMessageHandler<T> : IHandleMessages<T>, IHandleMessages<IFailed<T>>
    {
        private readonly ILogger _logger;
        private readonly RebusOptions _options;
        private readonly IBus _bus;

        protected AbstractMessageHandler(ILogger logger, IOptions<RebusOptions> options, IBus bus)
        {
            _logger = logger;
            _options = options.Value;
            _bus = bus;
        }
        
        public abstract Task Handle(T message);
        
        public Task Handle(IFailed<T> message)
        {
            var optionalHeaders = new Dictionary<string, string>();
            int currentAttempt;
            if (message.Headers != null && message.Headers.ContainsKey(RebusOptions.AttemptNumberKey))
            {
                var tryParseSuccess = int.TryParse(message.Headers[RebusOptions.AttemptNumberKey], out var attemptNumber);
                if (!tryParseSuccess)
                {
                    throw new ArgumentException("The header of message contained an " + RebusOptions.AttemptNumberKey + " with an unexpected format. Expected an int, value: " + message.Headers[RebusOptions.AttemptNumberKey]);
                }
                currentAttempt = ++attemptNumber;
                optionalHeaders.Add(RebusOptions.AttemptNumberKey, attemptNumber.ToString());
                if (attemptNumber > _options.MaxAttempts)
                {
                    // To make the message end on the error queue, we throw an exception to fail the task
                    // and make Rebus's SimpleRetryStrategy move the message to error queue.
                    // Note that the implementation of SimpleRetryStrategy will make this second level
                    // retry mechanism retry until the MaxAttempts is exceeded, like the ordinary message.
                    //
                    // Creator of Rebus, Mogens Heller Grabe, recommends this approach throwing exceptions
                    // to mimic the Rebus' behaviour of forwarding messages to the error queue:
                    // https://stackoverflow.com/a/43769002/617413
                    using (LogContext.PushProperty("Message", message, true))
                    {
                        _logger.LogError("Unable to deliver message. Forwarding message to the error queue");
                    }
                    throw new RebusFailedMessageMaxAttemptsException();
                }
            }
            else
            {
                currentAttempt = 1;
                optionalHeaders.Add(RebusOptions.AttemptNumberKey, currentAttempt.ToString());
            }

            // if message failed to process, defer processing for ? minutes and try again
            return DeferMessageAsync(message, optionalHeaders, currentAttempt);
        }
        
        private async Task DeferMessageAsync(IFailed<T> message, Dictionary<string, string> optionalHeaders, int currentAttempt)
        {
            using (LogContext.PushProperty("Message", message, true))
            {
                _logger.LogWarning("Attempt number {AttemptNumber} failed. Deferring message for {RetryInMinutes} minutes before trying again", currentAttempt, _options.RetryInMinutes);
            }

            await _bus.Advanced.TransportMessage.Defer(TimeSpan.FromMinutes(_options.RetryInMinutes), optionalHeaders);
        }
    }
}
