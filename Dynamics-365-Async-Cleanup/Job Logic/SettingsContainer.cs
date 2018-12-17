using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics_365_Async_Cleanup.Job_Logic
{
    public class SettingsContainer
    {
        public string _fetchxml;
        public int _batchsize;
        // If is delete is true we delete, if false we cancel system jobs.
        public bool _isdelete;

        public MainWindow _window;
        public CRMLoginForm1 _CRMLogin;
    }
}
