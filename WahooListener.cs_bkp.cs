using Apple.DNSSD;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApplication11
{
    class WahooListener
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string _svc = "_WahooFitnessBroadcaster._udp.";
        private const string _domain = "local.";

        private static bool _done = false;
        private static bool _listen = false;
        private static Thread _listenThr = null;
        
        private  string _svcHostName = null;
        private  int _svcPort = -1;
        private  IPAddress _svcIPAddr = null;

        private DNSService.BrowseReply _browseReply = null;
        private DNSService.ResolveReply _resolveReply = null;
        private DNSService.QueryRecordReply _queryRecordReply = null;
        private ServiceRef _service = null;
        private UdpClient _udpListener = null;

        public WahooListener()
        {
            _browseReply = new DNSService.BrowseReply(BrowseReply);
            _resolveReply = new DNSService.ResolveReply(ResolveReply);
            _queryRecordReply = new DNSService.QueryRecordReply(QueryRecordReply);
            _listen = true;
            _done = false;

        }

        public void Init()
        {
            log.Debug("Init");
            log.Debug("browsing for services...");
          
            DNSService.Browse(0, 0, _svc, _domain, _browseReply);

            while (!_done)
            {
                Thread.Sleep(250);
            }           

            WakeUp();

            _listenThr = new Thread(new ThreadStart(ListenThread));
            _listenThr.Start();
        }

        private void WakeUp()
        {
            log.Debug("WakeUp");

            UdpClient sender = new UdpClient(_svcPort);

            sender.Connect(_svcIPAddr, _svcPort);

            // Sends a message to the host to which you have connected.
            Byte[] sendBytes = Encoding.ASCII.GetBytes("{ \"Status\" : \"OK\"}");

            sender.Send(sendBytes, sendBytes.Length);
        }

        private void ListenThread()
        {
            log.Debug("ListenThread");

            _udpListener = new UdpClient(_svcPort + 2);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Broadcast, _svcPort);
            String data = String.Empty;

            byte[] rec_array;
            try
            {
                while (_listen)
                {
                    rec_array = _udpListener.Receive(ref ipe);
                    data = Encoding.ASCII.GetString(rec_array, 0, rec_array.Length);
                    log.DebugFormat("Received a broadcast from {0}. data = {1}", ipe.ToString(), data);
                    
                    Thread.Sleep(250);
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                try
                {
                    _udpListener.Close();
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }

            log.Debug("ListenThread - exiting");
        }

        public void Stop()
        {
            log.Debug("Stop");

            _listen = false;
        }

        private void QueryRecordReply(ServiceRef sdRef, ServiceFlags flags, int interfaceIndex, ErrorCode errorCode, string fullName, int rrtype, int rrclass, byte[] rdata, int ttl)
        {
            uint bits = BitConverter.ToUInt32((Byte[])rdata, 0);
            _svcIPAddr = new System.Net.IPAddress(bits);

            log.DebugFormat("host ip addr = {0}", _svcIPAddr);

            _done = true;
        }

        private void ResolveReply(ServiceRef sdRef, ServiceFlags flags, int interfaceIndex, ErrorCode errorCode, string fullName, string hostName, int port, byte[] txtRecord)
        {
            _service = sdRef;
            _svcHostName = hostName;
            _svcPort = port;

            log.DebugFormat("host = {0}, port = {1}", hostName, port);

            DNSService.QueryRecord(0, interfaceIndex, hostName, 1, 1, _queryRecordReply);
        }

        public void BrowseReply(ServiceRef sdRef, ServiceFlags flags, int interfaceIndex, ErrorCode errorCode, string name, string type, string domain)
        {
            log.DebugFormat("service found on host = {0}", name);

            _service = DNSService.Resolve(0, interfaceIndex, name, _svc, _domain, _resolveReply);
        }
    }
}
