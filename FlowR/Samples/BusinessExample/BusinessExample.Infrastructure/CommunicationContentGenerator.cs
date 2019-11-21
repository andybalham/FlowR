using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessExample.Core.Interfaces;
using Jint;

namespace BusinessExample.Infrastructure
{
    public class CommunicationContentGenerator : ICommunicationContentGenerator
    {
        public Task<string> GenerateContent(string templateName, IDictionary<string, object> dataObjects)
        {
            var templateContent = GetTemplateContent(templateName);

            var mergedContent = GetMergedContent(templateContent, dataObjects);

            return Task.FromResult(mergedContent);
        }

        private static string GetMergedContent(string templateContent, IDictionary<string, object> dataObjects)
        {
            var engine = new Engine(cfg => cfg.AllowClr());

            foreach (var dataObjectsKey in dataObjects.Keys)
                engine.SetValue(dataObjectsKey, dataObjects[dataObjectsKey]);

            var mergedContent =
                Regex.Replace(templateContent, "{{(?<script>[^}]*)}}", m =>
                {
                    var script = m.Groups["script"].Value;
                    var scriptValue = GetScriptValue(script, engine);
                    return scriptValue;
                });

            return mergedContent;
        }

        private static string GetScriptValue(string script, Engine engine)
        {
            var scriptValue = engine.Execute(script).GetCompletionValue().ToString();
            return scriptValue;
        }

        private string GetTemplateContent(string templateName)
        {
            switch (templateName)
            {
                case "AcceptConfirmation":
                    return
                        @"Dear {{LoanApplication.ApplicantName}},
We are pleased to say that you have been accepted for a loan for £{{LoanApplication.LoanAmount.toFixed(2)}}.
Your application reference is {{LoanApplication.Reference}}.
Regards,
FlowR Lending Plc";

                case "ReferNotification":
                    return
                        @"Dear {{LoanApplication.ApplicantName}},
Your application for a loan for £{{LoanApplication.LoanAmount.toFixed(2)}} has been referred to our underwriters.
Your application reference is {{LoanApplication.Reference}}.
Regards,
FlowR Lending Plc";

                case "DeclineConfirmation":
                    return
                        @"Dear {{LoanApplication.ApplicantName}},
I am sorry to say that we have declined you application for a loan of £{{LoanApplication.LoanAmount.toFixed(2)}}.
Your application reference is {{LoanApplication.Reference}}.
Regards,
FlowR Lending Plc";

                default:
                    throw new ArgumentException($"Unhandled template name: {templateName}", nameof(templateName));
            }
        }
    }
}
