using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessExample.Core.Interfaces;

namespace BusinessExample.Infrastructure
{
    public class LoanEventService : ILoanEventService
    {
        private readonly string _outputDirectory;

        public LoanEventService(string outputDirectory)
        {
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));

            Directory.CreateDirectory(outputDirectory);

            Directory.GetFiles(outputDirectory).ToList().ForEach(File.Delete);
        }

        public Task RaiseEvent(LoanEventType loanEventType, string loanApplicationId, string loanDecisionId)
        {
            var fileName = $"{loanEventType}-{loanApplicationId}-{loanDecisionId}.txt";

            var filePath = Path.Combine(_outputDirectory, fileName);

            File.WriteAllText(filePath, string.Empty);

            return Task.CompletedTask;
        }
    }
}
