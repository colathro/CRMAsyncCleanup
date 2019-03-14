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
    // Batch controller used to execute the jobs from the queue. Can have several running as settings dictate.
    class BatchController
    {
        public SettingsContainer _settings;
        public QueueContainer _queue;

        public BatchController(SettingsContainer settings, QueueContainer queue)
        {
            _settings = settings;
            _queue = queue;
        }
    }
}
