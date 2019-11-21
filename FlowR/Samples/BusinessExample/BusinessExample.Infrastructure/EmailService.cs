using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly string _outputDirectory;

        public EmailService(string outputDirectory)
        {
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));

            Directory.CreateDirectory(outputDirectory);

            Directory.GetFiles(outputDirectory).ToList().ForEach(File.Delete);
        }

        public Task Send(EmailCommunication emailCommunication)
        {
            var fileName = $"{emailCommunication.Id}-{emailCommunication.Name}-{emailCommunication.EmailAddress}.txt";
            
            var filePath = Path.Combine(_outputDirectory, fileName);

            File.WriteAllText(filePath, emailCommunication.Content);

            return Task.CompletedTask;
        }
    }
}
