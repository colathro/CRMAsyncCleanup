using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;

namespace Dynamics_365_Async_Cleanup.Job_Logic
{
    class Worker
    {

        public Worker(List<Entity> entities, CrmServiceClient service, bool delete)
        {
            // Uneccesary instiation; but trivial and we can refactor later
            ExecuteMultipleRequest request = new ExecuteMultipleRequest();
            // Switch case to determine what type of ExecuteMultipleRequest to call
            switch (delete)
            {
                case true:
                    request = GetRequestDelete(entities);
                    break;
                case false:
                    request = GetRequestCancel(entities);
                    break;
            }

            ExecuteMultipleResponse response = (ExecuteMultipleResponse)service.Execute(request);

        }

        public ExecuteMultipleRequest GetRequestDelete(List<Entity> entities)
        {
            ExecuteMultipleSettings executeSettings = new ExecuteMultipleSettings()
            {
                ContinueOnError = true,
                ReturnResponses = true
            };

            var requestWithResults = new ExecuteMultipleRequest()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = executeSettings
            };

            // iterate over the passed entities to delete adding the delete request to the execute multiple request
            foreach (Entity entity in entities)
            {
                DeleteRequest del = new DeleteRequest { Target = new EntityReference("asyncoperation", entity.Id) };
                requestWithResults.Requests.Add(del);
            }
            return requestWithResults;
        }

        public ExecuteMultipleRequest GetRequestCancel(List<Entity> entities)
        {
            ExecuteMultipleSettings executeSettings = new ExecuteMultipleSettings()
            {
                ContinueOnError = true,
                ReturnResponses = true
            };

            var requestWithResults = new ExecuteMultipleRequest()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = executeSettings
            };

            // iterate over the passed entities to delete adding the delete request to the execute multiple request
            foreach (Entity entity in entities)
            {
                UpdateRequest upd = new UpdateRequest();
                entity["statecode"] = new OptionSetValue(3);
                entity["statuscode"] = new OptionSetValue(0);
                upd.Target = entity;
                requestWithResults.Requests.Add(upd);
            }
            return requestWithResults;
        }
    }
}
