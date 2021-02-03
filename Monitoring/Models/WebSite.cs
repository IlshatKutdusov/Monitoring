namespace Monitoring.Models
{
    public class WebSite
    {
        private string _Name { get; set; }
        private string _URL { get; set; }
        private string _AvialabilityCheckInterval { get; set; }
        private string _Status { get; set; }
        private string _Ping { get; set; }

        public string Name { get => _Name; }
        public string URL { get => _URL; }
        public string AvialabilityCheckInterval { get => _AvialabilityCheckInterval; }
        public string Status { get => _Status; }
        public string Ping { get => _Ping; }

        public WebSite()
        {
            _Name = " ";
            _URL = " ";
            _AvialabilityCheckInterval = " ";
            _Status = " ";
            _Ping = " ";
        }

        public WebSite( string name, string url, string avialabilitycheckinterval)
        {
            _Name = name;
            _URL = url;
            _AvialabilityCheckInterval = avialabilitycheckinterval;
            _Status = "         null         ";
            _Ping = "    null    ";
        }

        public void SetName(string webSiteName)
        {
            _Name = webSiteName;
        }

        public void SetURL(string webSiteURL)
        {
            _URL = webSiteURL;
        }

        public void SetAvialabilityCheckInterval(string webSiteAvialabilityCheckInterval)
        {
            _AvialabilityCheckInterval = webSiteAvialabilityCheckInterval;
        }

        public void SetStatus(string status)
        {
            _Status = status;
        }

        public void SetPing(string ping)
        {
            _Ping = ping;
        }
    }
}