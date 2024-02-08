using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
namespace CreationAccount
{
public class CreateAccountOnContactCreate : IPlugin
{
private ITracingService tracingService;
public CreateAccountOnContactCreate()
{

}
public CreateAccountOnContactCreate(ITracingService tracingService)
{
this.tracingService = tracingService;
}
public void Execute(IServiceProvider serviceProvider)
{
// Obtain the execution context from the service provider.
IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

// Check if the plugin is fired on the Create message of the Contact entity.
if (context.MessageName.ToLower() == "create" && context.PrimaryEntityName == "contact")
{
tracingService.Trace("Plugin execution started.");

// Ensure the target entity is present in the context and is of the Contact type.
if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
{
// Get the target entity (newly created contact record).
Entity contactEntity = (Entity)context.InputParameters["Target"];
tracingService.Trace("Some processing completed.");

// Check if required attributes are present in the contact record (e.g., Full Name and Email).
if (contactEntity.Attributes.Contains("fullname") && contactEntity.Attributes.Contains("emailaddress1"))
{
// Create an account entity.
Entity accountEntity = new Entity("account");

// Set attributes for the account record.
accountEntity["name"] = contactEntity.GetAttributeValue<string>("fullname") + "'s Account"; // Adjust this as needed.
accountEntity["emailaddress1"] = contactEntity.GetAttributeValue<string>("emailaddress1");
tracingService.Trace("Plugin execution finished.");

// Add additional attributes as needed.

// Obtain the organization service from the service provider.
IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

// Create the account record.
service.Create(accountEntity);
}
}
}
}
}
}


