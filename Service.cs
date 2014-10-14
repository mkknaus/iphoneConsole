using System.Net;

namespace iphoneConsole
{
    public class Service
    {
        private string _name = null;
        private string _hostName = null;
        private int _port = -1;
        private IPAddress _ip = IPAddress.Any;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        public IPAddress IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
    }
}
