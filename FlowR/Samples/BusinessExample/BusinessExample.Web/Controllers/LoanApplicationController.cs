using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Exceptions;
using BusinessExample.Core.Exchanges;
using BusinessExample.Core.Exchanges.LoanApplications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessExample.Web.Controllers
{
    [Route("api/loan-application")]
    [ApiController]
    public class LoanApplicationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoanApplicationController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateLoanApplication([FromBody] LoanApplication loanApplication)
        {
            try
            {
                var createRequest = new CreateLoanApplication { LoanApplication = loanApplication };
                var createResponse = await _mediator.Send(createRequest);

                return new { Id = createResponse.LoanApplicationId };
            }
            catch (NotFoundResourceException)
            {
                return BadRequest();
            }
            catch (UnauthorizedResourceException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost("{id}/make-decision")]
        public async Task<ActionResult<object>> MakeLoanApplicationDecision(string id)
        {
            try
            {
                // TODO: Merge these into one flow
                var retrieveRequest = new RetrieveLoanApplication { LoanApplicationId = id };
                var retrieveResponse = await _mediator.Send(retrieveRequest);
                var loanApplication = retrieveResponse.LoanApplication;

                var decisionRequest = new MakeLoanApplicationDecision { LoanApplication = loanApplication };
                var decisionResponse = await _mediator.Send(decisionRequest);
                return new { decisionResponse.LoanDecision, Trace = decisionResponse.Trace.ToString() };
            }
            catch (NotFoundResourceException)
            {
                return NotFound();
            }
            catch (UnauthorizedResourceException)
            {
                return Unauthorized();
            }
            // TODO: Think about https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling
            //catch (Exception e)
            //{
            //    return StatusCode((int)HttpStatusCode.InternalServerError, e);
            //}
        }
    }
}
