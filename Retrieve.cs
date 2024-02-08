using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace PreValidation
{
    public class Retrieve : IPlugin
    {
        private string unsecureConfig;
        private string secureConfig;
        private ITracingService tracingService;

        public Retrieve(string unsecureConfig, string secureConfig)
        {
            this.unsecureConfig = unsecureConfig;
            this.secureConfig = secureConfig;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            Microsoft.Xrm.Sdk.IPluginExecutionContext context = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
        serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            if (context.Depth == 1)
            {
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                // Obtain the target entity from the input parmameters.
                EntityReference entity = (EntityReference)context.InputParameters["Target"];

                ColumnSet cols = new ColumnSet(
                                     new String[] { "lastname", "firstname", "address1_name" });

                var contact = service.Retrieve("contact", entity.Id, cols);

                if (contact != null)
                {
                    if (contact.Attributes.Contains("address1_name") == false)
                    {
                        Random rndgen = new Random();
                        contact.Attributes.Add("address1_name", "first time value: " + rndgen.Next().ToString());
                    }
                    else
                    {
                        contact["address1_name"] = "i already exist";
                    }
                    service.Update(contact);
                }
            }
        }
    }
}
