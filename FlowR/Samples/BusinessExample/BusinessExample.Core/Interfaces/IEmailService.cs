using System.Threading.Tasks;
using BusinessExample.Core.Entities;

namespace BusinessExample.Core.Interfaces
{
    public interface IEmailService
    {
        Task Send(EmailCommunication emailCommunication);
    }
}