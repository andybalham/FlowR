using System;
using System.Collections.Generic;
using System.Text;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class CommunicationRepositoryTests
    {
        [Fact]
        public async void Can_create_and_retrieve_email_communication()
        {
            // Arrange

            var emailCommunication = new EmailCommunication
            {
                EmailAddress = "barnaby.fudge@andybalham.com"
            };

            var sut = new CommunicationRepository();

            // Act

            var emailCommunicationId = await sut.CreateEmail(emailCommunication);

            var retrievedEmailCommunication = await sut.RetrieveEmail(emailCommunicationId);

            // Assert

            Assert.Equal(emailCommunication.EmailAddress, retrievedEmailCommunication.EmailAddress);
        }

        [Fact]
        public async void Can_create_and_retrieve_email_communication_with_id()
        {
            // Arrange

            var emailCommunication = new EmailCommunication
            {
                Id = Guid.NewGuid().ToString(),
                EmailAddress = "barnaby.fudge@andybalham.com"
            };

            var sut = new CommunicationRepository();

            // Act

            var emailCommunicationId = await sut.CreateEmail(emailCommunication);

            Assert.Equal(emailCommunication.Id, emailCommunicationId);

            var retrievedEmailCommunication = await sut.RetrieveEmail(emailCommunicationId);

            // Assert

            Assert.Equal(emailCommunication.EmailAddress, retrievedEmailCommunication.EmailAddress);
        }

        [Fact]
        public async void Null_returned_for_non_existent_email_communication()
        {
            // Arrange

            var sut = new CommunicationRepository();

            // Act

            var retrievedEmailCommunication = await sut.RetrieveEmail(Guid.NewGuid().ToString());

            // Assert

            Assert.Null(retrievedEmailCommunication);
        }

        [Fact]
        public async void Can_update_email_status()
        {
            // Arrange

            var emailCommunication = new EmailCommunication
            {
                Status = EmailCommunicationStatus.Pending
            };

            var sut = new CommunicationRepository();

            // Act

            var emailCommunicationId = await sut.CreateEmail(emailCommunication);

            var retrievedEmailCommunication = await sut.RetrieveEmail(emailCommunicationId);

            Assert.Equal(EmailCommunicationStatus.Pending, retrievedEmailCommunication.Status);

            await sut.UpdateEmailStatus(retrievedEmailCommunication.Id, EmailCommunicationStatus.Sent);

            var updatedEmailCommunication = await sut.RetrieveEmail(emailCommunicationId);

            // Assert

            Assert.Equal(EmailCommunicationStatus.Sent, updatedEmailCommunication.Status);
        }
    }
}
