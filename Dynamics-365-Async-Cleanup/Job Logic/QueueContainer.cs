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
    public class QueueContainer
    {
        public readonly object _queueLock = new object();
        public List<Entity> _queue;
        public SettingsContainer _settings;

        public QueueContainer(SettingsContainer settings)
        {
            _queue = new List<Entity>();
            _settings = settings;
        }

        public void Add(List<Entity> input)
        {
            lock (_queueLock)
            {
                _queue.AddRange(input);
            }
        }

        public List<Entity> Get()
        {
            List<Entity> output;
            lock (_queueLock)
            {
                output = _queue.GetRange(0, _settings._batchsize);
                _queue.RemoveRange(0, _settings._batchsize);
            }
            return output;
        }
    }
}
