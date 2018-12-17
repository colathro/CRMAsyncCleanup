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
    public class WorkerController
    {
        public SettingsContainer _settings;
        DateTime start;

        public WorkerController(SettingsContainer settings)
        {
            _settings = settings;
            this.SetStatus();
            // We only want 2 active threads executing the executemultiple - CRM Online is hard limited to 2 concurrent sessions
            ThreadPool.SetMaxThreads(2, 2);
            start = DateTime.Now;
            bool morerecords = true;
            
            while (morerecords)
            {
                EntityCollection modifyset = _settings._CRMLogin.CrmConnectionMgr.CrmSvc.RetrieveMultiple(new FetchExpression(_settings._fetchxml));
                // EntityCollection has non-useful so we convert explicitly to List<Entity> for future use
                List<Entity> entitieslist =  modifyset.Entities.ToList<Entity>();

                // if we get lessthan 5000 records we don't need to retry and all the records we are looking for should be gone
                if (entitieslist.Count < 5000)
                {
                    morerecords = false;
                }
                int activethreads = 0;
                while(entitieslist.Count > 0 || activethreads > 0)
                {
                    Thread.Sleep(250);
                    List<Entity> trimmedentitylist;
                    if (entitieslist.Count > 0)
                    {
                        if (entitieslist.Count >= _settings._batchsize)
                        {
                            trimmedentitylist = entitieslist.GetRange(0, settings._batchsize);
                            entitieslist.RemoveRange(0, settings._batchsize);
                        }
                        else
                        {
                            trimmedentitylist = entitieslist;
                        }
                        activethreads += 1;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(_ =>
                        {
                            Stopwatch stopWatch = new Stopwatch();
                            stopWatch.Start();
                            WorkerThreadStarter(trimmedentitylist, _settings._CRMLogin.CrmConnectionMgr.CrmSvc, _settings._isdelete);
                            stopWatch.Stop();
                            _settings._window.Dispatcher.Invoke((Action)(() =>
                            {
                                int next = Convert.ToInt32(_settings._window.TotalRecordsBox.Text) + _settings._batchsize;
                                _settings._window.TotalRecordsBox.Text = next.ToString();
                                _settings._window.RecordsPerSecondBox.Text = (Convert.ToInt32(_settings._window.TotalRecordsBox.Text) / (DateTime.Now - start).TotalSeconds).ToString();
                                activethreads -= 1;
                            }));
                        }));
                    }
                }
            }
        }  


        public void SetStatus()
        {
            _settings._window.Dispatcher.Invoke((Action)(() => { _settings._window.StatusBox.Text = "Running";}));
        }

        void WorkerThreadStarter(List<Entity> entities, CrmServiceClient proxy, bool delete)
        {
            Worker worker = new Worker(entities, proxy, delete);
        }

    }
}
