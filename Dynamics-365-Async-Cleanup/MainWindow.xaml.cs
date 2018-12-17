using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;
using Dynamics_365_Async_Cleanup.Job_Logic;
using Microsoft.Xrm.Sdk;

namespace Dynamics_365_Async_Cleanup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Private Class Members
        CRMLoginForm1 _ctrl;
        Thread _workercontroller;
        bool _loggedin = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Top Github Navigation URL - Starts process of default browser.
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Establish the Login control.  
            CRMLoginForm1 ctrl = new CRMLoginForm1();

            // Wire event to login response.   
            ctrl.ConnectionToCrmCompleted += ctrl_ConnectionToCrmCompleted;

            // Show the login control.   
            ctrl.ShowDialog();

            if(ctrl.CrmConnectionMgr.CrmSvc != null)
            {
                // Set the disabled text field to display the OrganizationID
                this.LoggedInOrganizationID.Text = ctrl.CrmConnectionMgr.ConnectedOrgId.ToString();
                // Set the disabled text field to display the Organization SDK URL
                this.LoggedInOrganizationURL.Text = ctrl.CrmConnectionMgr.CrmSvc.CrmConnectOrgUriActual.ToString();
                // Set the disabled text ield to display the logged in user ID
                this.LoggedInUserDisplay.Text = ctrl.CrmConnectionMgr.CrmSvc.OAuthUserId;
                this._ctrl = ctrl;
                this._loggedin = true;
            }
            

        }

        private void ctrl_ConnectionToCrmCompleted(object sender, EventArgs e)
        {
            if (sender is CRMLoginForm1)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ((CRMLoginForm1)sender).Close();
                });
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // Set self to disabled + set cancelbutton to enabled
            if (_loggedin && this.FetchXMLBox.Text != "")
            {
                this.StartButton.IsEnabled = false;
                this.CancelButton.IsEnabled = true;
                SettingsContainer settings = BuildSettingsContainer(this, _ctrl);
                _workercontroller = new Thread(() => { WorkerController controller = new WorkerController(settings); });
                _workercontroller.Start();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Set self to disabled + set startbutton to enabled
            this.StartButton.IsEnabled = true;
            this.CancelButton.IsEnabled = false;
            this.StatusBox.Text = "Canceling";
            // Abort the primary worker controller thread
            _workercontroller.Abort();
            this.StatusBox.Text = "Stopped";
        }

        private void BatchSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Set Display to the value of slider
            if (this.BatchSizeDisplay != null){
                this.BatchSizeDisplay.Text = this.BatchSizeSlider.Value.ToString();
            }
        }

        private SettingsContainer BuildSettingsContainer(MainWindow window, CRMLoginForm1 CRMLogin)
        {
            // thsi settings contained is used to pass variables to the workercontroller so it has the settings to submit batch jobs.
            SettingsContainer settings = new SettingsContainer();
            settings._batchsize = (int)window.BatchSizeSlider.Value;
            settings._fetchxml = window.FetchXMLBox.Text;
            settings._isdelete = window.DeleteRadioButton.IsEnabled;
            settings._window = window;
            settings._CRMLogin = CRMLogin;
            return settings;
        }

    }
}
