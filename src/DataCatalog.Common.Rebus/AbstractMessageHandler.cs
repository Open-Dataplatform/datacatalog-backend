using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.Bus;
using Rebus.Exceptions;
using Rebus.Handlers;
using Rebus.Messages;
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
            var maxDeferCount = _options.MaxAttempts;
            var deferCount = Convert.ToInt32(message.Headers.GetValueOrDefault(Headers.DeferCount));
            if (deferCount >= maxDeferCount) {
                using (LogContext.PushProperty("Message", message, true))
                {
                    _logger.LogError("Unable to deliver message. Forwarding message to the error queue");
                }
                return _bus.Advanced.TransportMessage.Deadletter($"Failed after {deferCount} deferrals\n\n{message.ErrorDescription}");
            }
            
            // if message failed to process, defer processing for _options.RetryInMinutes minutes and try again
            return DeferMessageAsync(message, deferCount);
        }
        
        private async Task DeferMessageAsync(IFailed<T> message, int currentAttempt)
        {
            using (LogContext.PushProperty("Message", message, true))
            {
                _logger.LogWarning("Attempt number {AttemptNumber} failed. Deferring message for {RetryInMinutes} minutes before trying again", currentAttempt, _options.RetryInMinutes);
            }

            await _bus.Advanced.TransportMessage.Defer(TimeSpan.FromMinutes(_options.RetryInMinutes));
        }
    }
}
