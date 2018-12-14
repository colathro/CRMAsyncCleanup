using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Dynamics_365_Async_Cleanup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Private Class Members

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

            // Handle the returned CRM connection object.  
            // On successful connection, display the CRM version and connected org name   
            if (ctrl.CrmConnectionMgr != null && ctrl.CrmConnectionMgr.CrmSvc != null && ctrl.CrmConnectionMgr.CrmSvc.IsReady)
            {
                MessageBox.Show("Connected to CRM! Version: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgVersion.ToString() +
                " Org: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgUniqueName, "Connection Status");

                // Perform your actions here  
            }
            else
            {
                MessageBox.Show("Cannot connect; try again!", "Connection Status");
            }
            // Set the disabled text field to display the OrganizationID
            this.LoggedInOrganizationID.Text = ctrl.CrmConnectionMgr.ConnectedOrgId.ToString();
            // Set the disabled text field to display the Organization SDK URL
            this.LoggedInOrganizationURL.Text = ctrl.CrmConnectionMgr.CrmSvc.CrmConnectOrgUriActual.ToString();
            // Set the disabled text ield to display the logged in user ID
            this.LoggedInUserDisplay.Text = ctrl.CrmConnectionMgr.CrmSvc.OAuthUserId;

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
    }
}
