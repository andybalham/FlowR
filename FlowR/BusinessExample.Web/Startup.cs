using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using BusinessExample.Infrastructure;
using FlowR;
using FlowR.Microsoft.Extensions.Logging;
using FlowR.StepLibrary.Decisions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessExample.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: See https://stackify.com/net-core-loggerfactory-use-correctly/

            services
                .AddMediatR(typeof(LoanApplication).Assembly)
                .AddMediatR(typeof(BoolFlowValueDecision).Assembly)

                .AddTransient(typeof(IFlowLogger<>), typeof(CoreFlowLogger<>))

                .AddSingleton<ILoanApplicationRepository>(new LoanApplicationRepository())
                .AddSingleton<ILoanDecisionRepository>(new LoanDecisionRepository())
                .AddSingleton<ICommunicationRepository>(new CommunicationRepository())

                .AddSingleton<IEmailService>(new EmailService("TestEmails"))
                .AddSingleton<ILoanEventService>(new LoanEventService("TestEvents"))

                .AddTransient<IAffordabilityEngine, AffordabilityEngine>()
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<IEligibilityVerifier, EligibilityVerifier>()
                .AddTransient<ICommunicationContentGenerator, CommunicationContentGenerator>()
                ;

            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
