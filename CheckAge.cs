using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;

namespace PreValidation
{
    public class CheckAge : IPlugin
    {
        private IOrganizationService service;
        private ITracingService trace;
        private ITracingService tracingService;
        public CheckAge()
        {

        }
        public CheckAge(ITracingService tracingService)
        {
            this.tracingService = tracingService;
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            service = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(context.UserId);
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));


            // Check if the plugin is triggered by the intended message and entity.
            if (context.MessageName.ToLower() != "create" || context.PrimaryEntityName.ToLower() != "contact")
            {
                return;
            }

            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];

                    DateTime birthDate = entity.GetAttributeValue<DateTime>("birthdate");
                    DateTime currentdate = DateTime.Today;
                    int age = currentdate.Year - birthDate.Year;
                    service.Retrieve("contact", new Guid("bbad1369-af7c-ee11-8179-000d3a6c20b5"), new ColumnSet(true));

                    if (entity.Attributes.Contains("birthdate"))
                    {
                        

                        // Log the age value using tracing.
                        tracingService.Trace("Age: " + age);

                        // Perform your comparison logic.
                        if (age <= 18)
                        {
                            tracingService.Trace("inside age ");
                            throw new InvalidPluginExecutionException("Age Should Be Greater then 18...");
                        }

                    }

                   
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions appropriately
                throw new InvalidPluginExecutionException($"An error occurred in the PreValidationPlugin: {ex.Message}", ex);
            }
        }
    }
}

