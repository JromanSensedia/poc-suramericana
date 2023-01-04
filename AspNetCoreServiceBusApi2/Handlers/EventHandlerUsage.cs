using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusMessaging;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusReceiverApi.Handlers
{
    public static class EventHandlerUsage
    {
        private static IProcessData _processData;        
        private static ServiceBusClient _client;
        private static ILogger _logger;
        private static ServiceBusMessaging.ServiceBusSender _serviceBusSender;
        private static string QUEUE_NAME = "simplequeue";
        public static void UseEventHandler(this IApplicationBuilder app)
        {
            _processData = app.ApplicationServices.GetService<IProcessData>();
            IConfiguration _configuration = app.ApplicationServices.GetService<IConfiguration>();
            _serviceBusSender = app.ApplicationServices.GetService<ServiceBusMessaging.ServiceBusSender>();
            _logger = app.ApplicationServices.GetService<ILogger>();
            var connectionString = _configuration.GetConnectionString("ServiceBusConnectionString");
            QUEUE_NAME = _configuration["QuebeName"] ?? _configuration["QuebeName"].ToString();
            _client = new ServiceBusClient(connectionString);
            _ = RegisterOnMessageHandlerAndReceiveMessages();
        }
        public static async Task RegisterOnMessageHandlerAndReceiveMessages()
        {
            ServiceBusProcessor _processor;
            ServiceBusProcessorOptions _serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            _processor = _client.CreateProcessor(QUEUE_NAME, _serviceBusProcessorOptions);
            _processor.ProcessMessageAsync += ProcessMessagesAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;
            await _processor.StartProcessingAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// PRocesa Mensaje
        /// </summary>
        /// <param name="args">arg propertys</param>
        /// <returns></returns>
        private static async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            try
            {
                string ascii = Encoding.ASCII.GetString(args.Message.Body);

                dynamic myPayload = JsonConvert.DeserializeObject<MyPayload>(ascii);//Dictionary<string, object>
                
                if (myPayload is not null)
                {
                    
                    if (!string.IsNullOrEmpty(myPayload.FileSura))
                    {
                        var myPayloadDb = JsonConvert.DeserializeObject<MyPayload>(ascii);
                        await _processData.Process(myPayloadDb).ConfigureAwait(false);
                    }
                    else
                    {
                        var myPayloadDoc = JsonConvert.DeserializeObject<Documento>(ascii);
                        //Consultamos id documento base de datos yrepondemos el estado.
                        object mypayload = await _processData.GetDocument(myPayloadDoc).ConfigureAwait(false);
                        //Enviamos al Bus de salida
                        await _serviceBusSender.SendMessageOutPreparaDoc(mypayload).ConfigureAwait(false);
                    }
                }
                await args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Errores enviados al Dead Letter Quebe
        /// </summary>
        /// <param name="arg">Mesaage</param>
        /// <returns></returns>
        private static Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Message handler encountered an exception added to dealLetterQ");
            _logger.LogDebug($"- ErrorSource: {arg.ErrorSource}");
            _logger.LogDebug($"- Entity Path: {arg.EntityPath}");
            _logger.LogDebug($"- FullyQualifiedNamespace: {arg.FullyQualifiedNamespace}");

            return Task.CompletedTask;
        }
    }
}
