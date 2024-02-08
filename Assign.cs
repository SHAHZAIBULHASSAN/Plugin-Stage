using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace PreValidation
{
    public class Assign : IPlugin
    {
        private IOrganizationService service;
        private ITracingService tracingService;

        public Assign()
        {
        }

        public Assign(ITracingService tracingService)
        {
            this.tracingService = tracingService;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = serviceFactory.CreateOrganizationService(context.UserId);
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.MessageName.ToLower() != "assign" || context.PrimaryEntityName.ToLower() != "contact")
            {
                return;
            }

            if (context.Depth == 1)
            {
                EntityReference entityRef = (EntityReference)context.InputParameters["Target"];

                try
                {
                    // Retrieve the entity record after the assignment.
                    Entity entity = service.Retrieve(entityRef.LogicalName, entityRef.Id, new ColumnSet(true));

                    // Retrieve the "ownerid" attribute which holds the information about the assigned user or team.
                    Guid ownerId = entity.GetAttributeValue<EntityReference>("ownerid").Id;

                    // Retrieve the User or Team entity based on the ownerid.
                    Entity ownerEntity = service.Retrieve("systemuser", ownerId, new ColumnSet("fullname"));

                    // Get the name of the assigned user.
                    string assignedUserName = ownerEntity.GetAttributeValue<string>("fullname");

                    // Log or use the assigned user name as required.
                    tracingService.Trace($"Entity has been assigned to: {assignedUserName}");
                }
                catch (Exception ex)
                {
                    tracingService.Trace($"An error occurred: {ex.Message}");
                    throw new InvalidPluginExecutionException($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
