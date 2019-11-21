using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BusinessExample.Core.Entities;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class EmailServiceTests
    {
        [Fact]
        public async void Can_send_email()
        {
            // Arrange

            var outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailServiceTests", "Can_send_email");

            var emailCommunication = new EmailCommunication
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TestEmail",
                EmailAddress = "barnabyfudge@andybalham.com",
                Content = "Hello!"
            };

            var sut = new EmailService(outputDirectory);

            // Act

            await sut.Send(emailCommunication);

            // Assert

            Assert.Single(Directory.GetFiles(outputDirectory));
        }
    }
}
