using Microsoft.Xrm.Sdk;
using System;

public class Organization
{
    private IPluginExecutionContext context;
    private IOrganizationService service;
    private ITracingService trace;

    public void GetOrganization(IServiceProvider serviceProvider)
    {
        try
        {
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            service = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(context.UserId);
            trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
        }
        catch (Exception ex)
        {


        }

    }
}