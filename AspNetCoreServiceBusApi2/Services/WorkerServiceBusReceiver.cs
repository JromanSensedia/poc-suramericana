using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusMessaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusReceiverApi.Services
{
    public class WorkerServiceBusReceiver : IHostedService, IDisposable
    {
        private readonly ILogger<WorkerServiceBusReceiver> _logger;
        private readonly IServiceBusConsumer _serviceBusConsumer;
        private readonly IServiceBusTopicSubscription _serviceBusTopicSubscription;

        public WorkerServiceBusReceiver(IServiceBusConsumer serviceBusConsumer,
            IServiceBusTopicSubscription serviceBusTopicSubscription,
            ILogger<WorkerServiceBusReceiver> logger)
        {
            _serviceBusConsumer = serviceBusConsumer;
            _serviceBusTopicSubscription = serviceBusTopicSubscription;
            _logger = logger;
        }
        /// <summary>
        /// Start service
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting the service bus queue consumer and the subscription");
            await _serviceBusConsumer.RegisterOnMessageHandlerAndReceiveMessages().ConfigureAwait(false);
            await _serviceBusTopicSubscription.PrepareFiltersAndHandleMessages().ConfigureAwait(false);
        }
        /// <summary>
        /// Stop Service
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Stopping the service bus queue consumer and the subscription");
            await _serviceBusConsumer.CloseQueueAsync().ConfigureAwait(false);
            await _serviceBusTopicSubscription.CloseSubscriptionAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async void Dispose(bool disposing)
        {
            if (disposing)
            {
                await _serviceBusConsumer.DisposeAsync().ConfigureAwait(false);
                await _serviceBusTopicSubscription.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
