using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceBusReceiverApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBusReceiverApi.Controllers
{
    [Route("api/[controller]",Name ="Messages")]
    [ApiController]
    public class ViewPayloadMessagesController : Controller
    {
        private readonly PayloadContext _context;
        private readonly string quebeNameOut;
        private readonly ServiceBusClient _client;
        public ViewPayloadMessagesController(PayloadContext context, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ServiceBusConnectionString");
            _client = new ServiceBusClient(connectionString);
            _context = context;
            quebeNameOut = configuration["QuebeNameOut"] ?? configuration["QuebeNameOut"].ToString();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Get()
        {            
            // create a receiver that we can use to receive the message
            ServiceBusReceiver sbReceiver = _client.CreateReceiver(quebeNameOut);
            var messagesOfQueue = new List<object>();
            var previousSequenceNumber = -1L;
            var sequenceNumber = 0L;
            // the received message is a different type as it contains some service set properties           
            do
            {
                var messageBatch = await sbReceiver.PeekMessagesAsync(int.MaxValue, sequenceNumber);
               
                if (messageBatch.Count > 0)
                {
                    sequenceNumber = messageBatch[^1].SequenceNumber;

                    if (sequenceNumber == previousSequenceNumber)
                        break;
                    var result = messageBatch.Select(x => new
                    {
                        messageId = x.MessageId,
                        Body = JsonConvert.SerializeObject(x.Body.ToString(), Formatting.Indented)
                    });
                    foreach (var item in result)
                    {
                        messagesOfQueue.Add(item);
                    }
                    //messagesOfQueue.AddRange(messageBatch);

                    previousSequenceNumber = sequenceNumber;
                }
                else
                {
                    break;
                }
            } while (true);
            return Ok(messagesOfQueue);
            //return Ok(_context.Payloads.ToList());
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Payload> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }

            var result = _context.Payloads.FirstOrDefault(d => d.Name == name);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
