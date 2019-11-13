using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class CommunicationRepository : ICommunicationRepository
    {
        private readonly IDictionary<string, EmailCommunication> _emailCommunications = new Dictionary<string, EmailCommunication>();

        public Task<string> CreateEmail(EmailCommunication emailCommunication)
        {
            emailCommunication.Id = emailCommunication.Id ?? Guid.NewGuid().ToString();

            _emailCommunications.Add(emailCommunication.Id, emailCommunication);

            return Task.FromResult(emailCommunication.Id);
        }

        public Task<EmailCommunication> RetrieveEmail(string id)
        {
            return _emailCommunications.TryGetValue(id, out var emailCommunication) 
                ? Task.FromResult(emailCommunication) 
                : Task.FromResult<EmailCommunication>(null);
        }

        public async Task UpdateEmailStatus(string id, EmailCommunicationStatus status)
        {
            var emailCommunication = await RetrieveEmail(id);

            if (emailCommunication == null)
            {
                throw new InvalidOperationException($"No EmailCommunication found for id: {id}");
            }

            emailCommunication.Status = status;
        }
    }
}
