using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using FlowR;
using MediatR;

namespace BusinessExample.Core.Exchanges.Communications
{
    public class SendEmail : FlowActivityRequest<SendEmail.Response>
    {
        [NotNullValue]
        public string TemplateName { get; set; }

        [BoundValue, NotNullValue, SensitiveValue]
        public string EmailAddress { get; set; }

        [BoundValue, NotNullValue]
        public string ParentId { get; set; }

        [BoundValue]
        public FlowValueDictionary<object> DataObjects { get; set; }

        public class Response
        {
        }
    }

    public class SendEmailHandler : IRequestHandler<SendEmail, SendEmail.Response>
    {
        private readonly ICommunicationContentGenerator _communicationContentGenerator;
        private readonly ICommunicationRepository _communicationRepository;
        private readonly IEmailService _emailService;

        public SendEmailHandler(ICommunicationContentGenerator communicationContentGenerator,
            ICommunicationRepository communicationRepository, IEmailService emailService)
        {
            _communicationContentGenerator =
                communicationContentGenerator ?? throw new ArgumentNullException(nameof(communicationContentGenerator));
            _communicationRepository =
                communicationRepository ?? throw new ArgumentNullException(nameof(communicationRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<SendEmail.Response> Handle(SendEmail request, CancellationToken cancellationToken)
        {
            var content = await _communicationContentGenerator.GenerateContent(request.TemplateName, request.DataObjects);

            var emailCommunication = new EmailCommunication
            {
                Name = request.TemplateName,
                EmailAddress = request.EmailAddress,
                Content = content,
                ParentId = request.ParentId,
                Status = EmailCommunicationStatus.Pending,
            };

            var emailCommunicationId = await _communicationRepository.CreateEmail(emailCommunication);

            EmailCommunicationStatus postSendStatus;
            try
            {
                await _emailService.Send(emailCommunication);

                postSendStatus = EmailCommunicationStatus.Sent;
            }
            catch (Exception e)
            {
                // TODO: Log the exception

                postSendStatus = EmailCommunicationStatus.Failed;
            }

            await _communicationRepository.UpdateEmailStatus(emailCommunicationId, postSendStatus);

            // TODO: Return the postSendStatus with the response
            return new SendEmail.Response();
        }
    }

    public static class SendEmailMocks
    {
        public static FlowContext MockSendEmail(this FlowContext flowContext, List<string> actualTemplateNames = null)
        {
            flowContext.MockActivity<SendEmail, SendEmail.Response>(req =>
            {
                actualTemplateNames?.Add(req.TemplateName);
                return new SendEmail.Response();
            });

            return flowContext;
        }
    }

}