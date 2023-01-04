using System.Threading.Tasks;

namespace ServiceBusReceiverApi.Handlers
{
    public interface IHandler<TEvent> where TEvent : class
    {
        Task Execute(TEvent @event);
    }
}
