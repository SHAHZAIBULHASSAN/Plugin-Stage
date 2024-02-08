using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;

public class ContactStatusUpdatePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
        ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        try
        {
            tracingService.Trace("ContactStatusUpdatePlugin: Plugin execution started.");

            // Check if the plugin is triggered by the intended message and entity.
            if (context.MessageName.Equals("Update", StringComparison.OrdinalIgnoreCase) && context.PrimaryEntityName.Equals("contact", StringComparison.OrdinalIgnoreCase))
            {
                tracingService.Trace("ContactStatusUpdatePlugin: Update message triggered for contact entity. Proceeding with plugin logic.");

                // Retrieve the target entity from the context
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity contact)
                {
                    tracingService.Trace("ContactStatusUpdatePlugin: Target entity found in the update message.");

                    // Check if the status is updated to your desired value (customize this condition based on your business logic)
                    int newStatusValue = contact.GetAttributeValue<OptionSetValue>("statuscode")?.Value ?? -1;
                    if (newStatusValue == YourDesiredStatusValue)
                    {
                        tracingService.Trace("ContactStatusUpdatePlugin: Contact status is updated to the desired value. Assigning new owner.");

                        // Assign new owner to the contact (replace with your actual values)
                        Guid newOwnerId = new Guid("YourNewOwnerId");
                        AssignRequest assignRequest = new AssignRequest
                        {
                            Assignee = new EntityReference("systemuser", newOwnerId),
                            Target = new EntityReference("contact", contact.Id)
                        };

                        service.Execute(assignRequest);

                        tracingService.Trace("ContactStatusUpdatePlugin: New owner assigned to the contact.");
                    }
                    else
                    {
                        tracingService.Trace("ContactStatusUpdatePlugin: Contact status is not updated to the desired value. Skipping owner assignment.");
                    }
                }
            }
            else
            {
                tracingService.Trace("ContactStatusUpdatePlugin: Plugin not triggered for 'Update' message or 'Target' entity not found. Exiting.");
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions appropriately
            tracingService.Trace("ContactStatusUpdatePlugin: An error occurred - {0}", ex.ToString());
            throw new InvalidPluginExecutionException($"An error occurred in the ContactStatusUpdatePlugin: {ex.Message}", ex);
        }
        finally
        {
            tracingService.Trace("ContactStatusUpdatePlugin: Plugin execution completed.");
        }
    }

    // Replace YourDesiredStatusValue with the actual status code value you want to check
    private const int YourDesiredStatusValue = 2; // Example: Assuming "Approved" status code is 2
}
