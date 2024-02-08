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
    public class Associate : IPlugin
    {
        private IOrganizationService service;
        private ITracingService trace;
        private ITracingService tracingService;
        public Associate()
        {

        }
        public Associate(ITracingService tracingService)
        {
            this.tracingService = tracingService;
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                // Check if the plugin is triggered by Associate message and the related entities are Contacts and Accounts.
                if (context.MessageName.Equals("Associate") && context.InputParameters.Contains("RelatedEntities"))
                {
                    string relationshipSchemaName = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                    tracingService.Trace("relationshipSchemaName" + relationshipSchemaName);
                    if (relationshipSchemaName.Equals("contact_customer_accounts")) // Replace this with your actual relationship schema name
                    {
                        tracingService.Trace("Check Relationship");
                        EntityReferenceCollection relatedEntities = null;

                        if (context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                        {
                            tracingService.Trace("Check RelatedEntities");
                            relatedEntities = (EntityReferenceCollection)context.InputParameters["RelatedEntities"];
                        }

                        if (relatedEntities != null && relatedEntities.Count > 0)
                        {
                            tracingService.Trace("Check RelatedEntities" + relatedEntities.Count);
                            foreach (var relatedEntity in relatedEntities)
                            {
                                tracingService.Trace("for RelatedEntities" + relatedEntity.LogicalName);
                                if (relatedEntity.LogicalName.Equals("contact")) // Assuming the related entity is Contact
                                {
                                    // Retrieve the Contact record to get the full name.
                                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                                    ColumnSet columns = new ColumnSet("fullname");
                                    Entity contact = service.Retrieve("contact", relatedEntity.Id, columns);

                                    if (contact != null && contact.Contains("fullname"))
                                    {
                                        string contactFullName = contact.GetAttributeValue<string>("fullname");
                                       tracingService.Trace($"Associated Contact's Full Name: {contactFullName}");
                                        // Perform further logic or operations with the contactFullName
                                    }
                                }
                                // Add other conditions and retrieval logic for different related entities if needed
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }
    }
}









