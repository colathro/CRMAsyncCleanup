using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Diagnostics;


namespace Dynamics_365_Async_Cleanup.Job_Logic
{
    // Retriever controller is used to populate the queue constantly, no more then 10,000 in the queue at a time.
    class RetrieverController
    {
        public SettingsContainer _settings;
        public QueueContainer _queue;


        public RetrieverController(SettingsContainer settings, QueueContainer queue)
        {
            _settings = settings;
            _queue = queue;
        }
    }
}
