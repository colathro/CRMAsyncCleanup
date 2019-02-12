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
        bool _morerecords = true;
        List<Entity> _entitylist;
        DateTime _start = DateTime.Now;

        public WorkerController(SettingsContainer settings)
        {
            _settings = settings;
            this.SetStatus();
            // We only want 2 active threads executing the executemultiple - CRM Online is hard limited to 2 concurrent sessions
            while (_morerecords)
            {
                // Fetch records - 5000 typically means we need to fetch more 
                // TODO: Error parsing XML crashes app - need to handle here
                EntityCollection modifyset = _settings._CRMLogin.CrmConnectionMgr.CrmSvc.RetrieveMultiple(new FetchExpression(_settings._fetchxml));
                // EntityCollection has non-useful so we convert explicitly to List<Entity> for future use
                List<Entity> _entitylist = modifyset.Entities.ToList<Entity>();
                // check if we can stop - less than 5000 typically means records can be removed
                if (_entitylist.Count < 5000)
                {
                    _morerecords = false;
                }
                // method to check how many batches we need for a given set of records returned
                int batchcount = GetBatchCount(_entitylist.Count, _settings._batchsize);
                // we batch out the records based on the number of batches we need in relation to the batch size.
                for (int i = 1; i <= batchcount; i++)
                {
                    // verify we have enough records in the active list to put into the batch
                    if (_entitylist.Count >= _settings._batchsize)
                    {
                        Execute(_entitylist.GetRange(0, _settings._batchsize), _settings._CRMLogin.CrmConnectionMgr.CrmSvc, _settings._isdelete);
                        // remove the first batches
                        _entitylist.RemoveRange(0, _settings._batchsize);
                        // Dispatch a command to update the UI
                        _settings._window.Dispatcher.Invoke((Action)(() =>
                        {
                            int next = Convert.ToInt32(_settings._window.TotalRecordsBox.Text) + _settings._batchsize;
                            _settings._window.TotalRecordsBox.Text = next.ToString();
                            _settings._window.RecordsPerSecondBox.Text = (Convert.ToInt32(_settings._window.TotalRecordsBox.Text) / (DateTime.Now - _start).TotalSeconds).ToString();
                        }));
                    }
                    // Else block for possible last count sizes
                    else
                    {
                        Execute(_entitylist, _settings._CRMLogin.CrmConnectionMgr.CrmSvc, _settings._isdelete);
                        // Dispatch a command to update the UI
                        _settings._window.Dispatcher.Invoke((Action)(() =>
                        {
                            int next = Convert.ToInt32(_settings._window.TotalRecordsBox.Text) + _entitylist.Count;
                            _settings._window.TotalRecordsBox.Text = next.ToString();
                            _settings._window.RecordsPerSecondBox.Text = Math.Round((Convert.ToInt32(_settings._window.TotalRecordsBox.Text) / (DateTime.Now - _start).TotalSeconds), 2).ToString();
                        }));
                        _entitylist = new List<Entity>();
                    }
                }
            }
            _settings._window.Dispatcher.Invoke((Action)(() => {
                _settings._window.StatusBox.Text = "Finished";
                _settings._window.RecordsPerSecondBox.Text = "0";
                _settings._window.StartButton.IsEnabled = true;
                _settings._window.CancelButton.IsEnabled = false;
            }));
        }

        public int GetBatchCount(int count, int batchsize)
        {   
            if (batchsize == 0)
            {
                return 0;
            }
            if ((count % batchsize) == 0)
            {
                return (count / batchsize);
            }
            else
            {
                return (count / batchsize) + 1;
            }
        }

        public void SetStatus()
        {
            // called when started - we are inside workercontroller
            _settings._window.Dispatcher.Invoke((Action)(() => { _settings._window.StatusBox.Text = "Running";
                _settings._window.RecordsPerSecondBox.Text = "0";
            }));
        }

        public void Execute(List<Entity> entities, CrmServiceClient service, bool delete)
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
            // Build new ExecuteMultipleSettings request from SDK
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
            // Build new ExecuteMultipleSettings request from SDK
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

            // iterate over the passed entities to delete adding the cancel request to the execute multiple request
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

//        var possibleclients = new CRMServiceProvider(_settings._CRMLogin.CrmConnectionMgr.CrmSvc, _settings._CRMLogin.CrmConnectionMgr.CrmSvc.Clone());
//        start = DateTime.Now;
//            bool morerecords = true;
            
//            while (morerecords)
//            {
//                EntityCollection modifyset = _settings._CRMLogin.CrmConnectionMgr.CrmSvc.RetrieveMultiple(new FetchExpression(_settings._fetchxml));
//        // EntityCollection has non-useful so we convert explicitly to List<Entity> for future use
//        List<Entity> entitieslist = modifyset.Entities.ToList<Entity>();

//                // if we get lessthan 5000 records we don't need to retry and all the records we are looking for should be gone
//                if (entitieslist.Count< 5000)
//                {
//                    morerecords = false;
//                }
//    int activethreads = 0;
//    int threadtracker = 0;
//    bool truefalse;
//                while(entitieslist.Count > 0 || activethreads > 0)
//                {
//                    Thread.Sleep(250);
//                    threadtracker++;
//                    truefalse = true;
//                    if (threadtracker % 2 == 0)
//                    {
//                        truefalse = false;
//                    }
//List<Entity> trimmedentitylist;
//                    if (entitieslist.Count > 0)
//                    {
//                        if (entitieslist.Count >= _settings._batchsize)
//                        {
//                            trimmedentitylist = entitieslist.GetRange(0, settings._batchsize);
//                            entitieslist.RemoveRange(0, settings._batchsize);
//                        }
//                        else
//                        {
//                            trimmedentitylist = entitieslist;
//                        }
//                        activethreads += 1;
//                        ThreadPool.QueueUserWorkItem(new WaitCallback(_ =>
//                        {
//    Stopwatch stopWatch = new Stopwatch();
//    stopWatch.Start();
//    WorkerThreadStarter(trimmedentitylist, possibleclients.GetClient(truefalse), _settings._isdelete);
//    stopWatch.Stop();
//    _settings._window.Dispatcher.Invoke((Action)(() =>
//    {
//        int next = Convert.ToInt32(_settings._window.TotalRecordsBox.Text) + _settings._batchsize;
//        _settings._window.TotalRecordsBox.Text = next.ToString();
//        _settings._window.RecordsPerSecondBox.Text = (Convert.ToInt32(_settings._window.TotalRecordsBox.Text) / (DateTime.Now - start).TotalSeconds).ToString();
//        activethreads -= 1;
//    }));
//}));
//                    }
//                }
//            }
