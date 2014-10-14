using Mono.Zeroconf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace iphoneConsole
{
    class ServiceFinder : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SVC = "_WahooFitnessBroadcaster._udp.";
        private const string DOMAIN = "local.";

        private Service _service = null;
        private ServiceBrowser _browser = null;
        private bool _svcFound = false;
        private Dictionary<String, IResolvableService> _serviceDict;


        public delegate void ServiceFoundDel(List<String> serviceList);
        public delegate void ServiceResolveDel(Service service);

        public event ServiceFoundDel ServiceFound;
        public event ServiceResolveDel ServiceResolved;

        public ServiceFinder()
        {
            _browser = new ServiceBrowser();
            _serviceDict = new Dictionary<String, IResolvableService>();
            _svcFound = false;
        }

        public void Dispose()
        {
            _browser.Dispose();
        }

        public void Find()
        {
            log.Debug("Find - browsing for service");

            _browser.ServiceAdded +=_browser_ServiceAdded;
            _browser.ServiceRemoved += _browser_ServiceRemoved;
            _browser.Browse(SVC, DOMAIN);
        }

        void _browser_ServiceRemoved(object o, ServiceBrowseEventArgs args)
        {
            log.DebugFormat("Service removed - {0}", args.Service.Name);
        }

        private void _browser_ServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            var bytes = Encoding.Default.GetBytes(args.Service.Name);
            var text = Encoding.UTF8.GetString(bytes);

            _serviceDict.Add(text, args.Service);

            log.DebugFormat("Service added - {0}", text);

            // use the first service that is found on the network
            if (!_svcFound)
            {
                //List<string> list = new List<string>(_serviceDict.Keys);
                //OnServiceFound(list);

                // resolve the service
                args.Service.Resolved += Service_Resolved;
                args.Service.Resolve();
                
                // ignore any other services that are found
                _svcFound = true;
            }
        }

        public void Resolve(string name)
        {
            _service = new Service();
            _service.Name = name;
            IResolvableService svc = null;
            if (_serviceDict.TryGetValue(name, out svc))
            {
                svc.Resolved += Service_Resolved;
                svc.Resolve();
            }
        }

        private void Service_Resolved(object o, ServiceResolvedEventArgs args)
        {
            Service s = new Service();

            s.HostName = args.Service.HostEntry.HostName;
            s.IP = args.Service.HostEntry.AddressList[0];
            s.Name= args.Service.Name;
            s.Port= args.Service.Port;

            OnServiceResolved(s);
        }

        private void OnServiceResolved(Service service)
        {
            if (ServiceResolved != null)
            {
                ServiceResolved(service);
            }
        }

        private void OnServiceFound(List<String> serviceNames)
        {
            if (ServiceFound != null)
            {
                ServiceFound(serviceNames);
            }
        }
    }
}
