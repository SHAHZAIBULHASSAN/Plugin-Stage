using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace RetrieveAllPlugin
{
    public class RetrieveAll : IPlugin
    {
        private string unsecureConfig;
        private string secureConfig;
        private ITracingService tracingService;
        public RetrieveAll()
        {
            
        }
        public RetrieveAll(ITracingService tracingService)
        {
            this.tracingService = tracingService;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (context.Depth == 1)
            {
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                QueryExpression query = new QueryExpression("lead");
                query.ColumnSet = new ColumnSet("firstname"); // Retrieve all attributes
                EntityCollection contactEntities = service.RetrieveMultiple(query);

                // Access retrieved Contact entities
                foreach (Entity contactEntity in contactEntities.Entities)
                {
                    if (contactEntity.Attributes.Contains("firstname"))
                    {
                        var contactName = contactEntity.GetAttributeValue<string>("firstname");
                        // Perform operations with contactName or other attributes.
                        tracingService.Trace($"Retrieved lead: {contactName}");
                    }
                }

                // Log execution or perform other necessary tasks.
                tracingService.Trace("RetrieveAll plugin executed successfully.");
            }

        }


    }
}
