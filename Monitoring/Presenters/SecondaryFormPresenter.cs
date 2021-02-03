using System;
using System.Windows.Forms;
using System.Security.Policy;
using System.Configuration;
using Monitoring.Common;

using Monitoring.Views;
using Monitoring.Models;


namespace Monitoring.Presenters
{
    public class SecondaryFormPresenter : Presener<ISecondaryFormView, WebSite>
    {
        private readonly ISecondaryFormView _secondaryFormView;
        private TextBox _textBoxName, _textBoxURL, _textBoxCheckInterval;
        private WebSite _webSite = new WebSite();
        private readonly string _dataFileName = ConfigurationSettings.AppSettings["dataFileName"];

        public SecondaryFormPresenter(IApplicationController applicationController ,ISecondaryFormView secondaryFormView) 
            : base (applicationController, secondaryFormView)
        {
            _secondaryFormView = secondaryFormView;
            _secondaryFormView.WebSiteUpload += () => WebSiteUpload();
            _textBoxName = secondaryFormView.GetTextBoxName();
            _textBoxURL = secondaryFormView.GetTextBoxURL();
            _textBoxCheckInterval = secondaryFormView.GetTextBoxCheckInterval();
        }

        public void Run()
        {
            _secondaryFormView.Show();
        }

        public override void Run(WebSite webSite)
        {
            _webSite = webSite;
            _textBoxName.Text = _webSite.Name;
            _textBoxURL.Text = _webSite.URL;
            _textBoxCheckInterval.Text = _webSite.AvialabilityCheckInterval;
            _secondaryFormView.Show();
        }

        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage);
        }

        private void WebSiteUpload()
        {
            try
            {
                Url url = new Url(_textBoxURL.Text);
                _textBoxCheckInterval.Text = Convert.ToString(Convert.ToInt16(_textBoxCheckInterval.Text));
                _webSite.SetName(_textBoxName.Text);
                _webSite.SetURL(url.Value);
                _webSite.SetAvialabilityCheckInterval(_textBoxCheckInterval.Text);
                url = null;
                _secondaryFormView.Close();
            }
            catch(Exception ex)
            {
                ShowErrorMessage("" + ex);
            }

        }
    }
}
