using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessExample.Core.Interfaces
{
    public interface ICommunicationContentGenerator
    {
        Task<string> GenerateContent(string templateName, IDictionary<string, object> dataObjects);
    }
}