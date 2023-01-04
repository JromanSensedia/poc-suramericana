using System.Threading.Tasks;

namespace ServiceBusMessaging
{
    public interface IProcessData
    {
        Task Process(MyPayload myPayload);
        Task<dynamic> GetDocument(Documento documentId);
    }
}
