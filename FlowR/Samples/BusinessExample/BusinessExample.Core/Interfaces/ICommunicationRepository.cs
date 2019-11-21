using System.Threading.Tasks;
using BusinessExample.Core;
using BusinessExample.Core.Entities;

namespace BusinessExample.Core.Interfaces
{
    public interface ICommunicationRepository
    {
        Task<string> CreateEmail(EmailCommunication emailCommunication);

        Task<EmailCommunication> RetrieveEmail(string id);

        Task UpdateEmailStatus(string id, EmailCommunicationStatus status);
    }
}