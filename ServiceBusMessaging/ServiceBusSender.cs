using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceBusMessaging
{
    public class ServiceBusSender
    {
        private readonly ServiceBusClient _client;
        private readonly Azure.Messaging.ServiceBus.ServiceBusSender _clientSender;       
        private readonly string quebeNameOut;
        public ServiceBusSender(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ServiceBusConnectionString");
            _client = new ServiceBusClient(connectionString);
            string QUEUE_NAME =  configuration["QuebeName"]??configuration["QuebeName"].ToString();
            quebeNameOut= configuration["QuebeNameOut"] ?? configuration["QuebeNameOut"].ToString();
            _clientSender = _client.CreateSender(QUEUE_NAME);
        }

        public async Task SendMessage(MyPayload payload)
        {
            string messagePayload = JsonSerializer.Serialize(payload);
            ServiceBusMessage message = new ServiceBusMessage(messagePayload);
            await _clientSender.SendMessageAsync(message).ConfigureAwait(false);
        }
        public async Task SendMessageSolicitaDoc(Documento document)
        {
            string messagePayload = JsonSerializer.Serialize(document);
            ServiceBusMessage message = new ServiceBusMessage(messagePayload);
            await _clientSender.SendMessageAsync(message).ConfigureAwait(false);
        }
        public async Task SendMessageOutPreparaDoc(object documentState)
        {
            string messagePayload = JsonSerializer.Serialize(documentState);
            ServiceBusMessage message = new ServiceBusMessage(messagePayload);
            Azure.Messaging.ServiceBus.ServiceBusSender clientSenderOut = _client.CreateSender(quebeNameOut);
            await clientSenderOut. SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
