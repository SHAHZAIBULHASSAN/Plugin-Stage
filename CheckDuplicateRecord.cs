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
    public class CheckDuplicateRecord : IPlugin
    {
        private IOrganizationService service;
        private ITracingService tracingService;
        public CheckDuplicateRecord()
        {

        }
        public CheckDuplicateRecord(ITracingService tracingService)
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
            if (context.MessageName.ToLower() != "create" || context.PrimaryEntityName.ToLower() != "crb7b_teacher")
            {
                return;
            }

            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    tracingService.Trace("Start Plugin ");
                // Replace "your_autonumber_field_name" with the name of your auto-number field.
                    if (entity.Attributes.Contains("crb7b_name") && entity.Attributes["crb7b_name"] is string)
                    {
                        string autoNumberValue = entity.GetAttributeValue<string>("crb7b_name");

                        // Query to check for duplicates of the auto number.
                        var query = new QueryExpression(entity.LogicalName)
                        {
                             ColumnSet = new ColumnSet("crb7b_name"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                            {
                                new ConditionExpression("crb7b_name", ConditionOperator.Equal, autoNumberValue)
                            }
                            }
                        };

                        EntityCollection results = service.RetrieveMultiple(query);

                        // If a duplicate auto number is found, throw an exception.
                        if (results.Entities.Count > 1 || (results.Entities.Count == 1 && results.Entities[0].Id != entity.Id))
                        {
                            throw new InvalidPluginExecutionException("Duplicate  number found. Please enter a unique value.");
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

