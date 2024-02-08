using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace PreValidation
{
    public class SetState : IPlugin
    {
        private IOrganizationService service;
        private ITracingService trace;
        private ITracingService tracingService;
        public SetState()
        {

        }
        public SetState(ITracingService tracingService)
        {
            this.tracingService = tracingService;
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                var context = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
                    serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

                if (context.InputParameters.Contains("EntityMoniker") &&
                    context.InputParameters["EntityMoniker"] is EntityReference)
                {
                    var myEntity = (EntityReference)context.InputParameters["EntityMoniker"];
                    var state = (OptionSetValue)context.InputParameters["State"];
                    var status = (OptionSetValue)context.InputParameters["Status"];

                    throw new InvalidPluginExecutionException(string.Format("Entity Name: {0}, State: {1}, Status: {2}", myEntity.LogicalName, state.Value.ToString(), status.Value.ToString()));

                }
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
           
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An Exception occurred in the SetStatePlugin plug-in.", ex);
            }

        }
    }
}

