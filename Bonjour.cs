using Bonjour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApplication11
{
    class BonjourListner
    {
        private static Bonjour.DNSSDEventManager eventManager = null;
        private static Bonjour.DNSSDService service = null;
        private static Bonjour.DNSSDService browser = null;
        private static Bonjour.DNSSDService resolver = null;

        private const string _svc = "_WahooFitnessBroadcaster._udp.";
        private const string _domain = "local.";
        private const string _hostName = null;

        private static Bonjour.DNSSDRecord record = null;

        public static void init()
        {
            eventManager = new DNSSDEventManager();
            eventManager.ServiceFound += new _IDNSSDEvents_ServiceFoundEventHandler(ServiceFound);
            eventManager.ServiceLost += new _IDNSSDEvents_ServiceLostEventHandler(ServiceLost);
            eventManager.ServiceResolved += new _IDNSSDEvents_ServiceResolvedEventHandler(ServiceResolved);
            eventManager.OperationFailed += new _IDNSSDEvents_OperationFailedEventHandler(OperationFailed);
            eventManager.QueryRecordAnswered += new _IDNSSDEvents_QueryRecordAnsweredEventHandler(QueryRecordAnswered);

            service = new DNSSDService();

            browser = service.Browse(0, 0, _svc, _domain, eventManager);
            //service.Resolve(0,0,
        }

        private static void QueryRecordAnswered(DNSSDService service, DNSSDFlags flags, uint ifIndex, string fullname, DNSSDRRType rrtype, DNSSDRRClass rrclass, object rdata, uint ttl)
        {
            
        }

        private static void OperationFailed(DNSSDService service, DNSSDError error)
        {
            
        }

        private static void ServiceResolved(DNSSDService service, DNSSDFlags flags, uint ifIndex, string fullname, string hostname, ushort port, TXTRecord record)
        {
            
        }

        private static void ServiceLost(DNSSDService browser, DNSSDFlags flags, uint ifIndex, string serviceName, string regtype, string domain)
        {
            
        }

        private static void ServiceFound(DNSSDService browser, DNSSDFlags flags, uint ifIndex, string serviceName, string regtype, string domain)
        {
            
        }
    }
}
