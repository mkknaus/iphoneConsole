using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
namespace iphoneConsole
{
    class WahooListener
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool _listen = false;
        private static Thread _listenThr = null;
        
        private  string _hostName = null;
        private  int _port = -1;
        private  IPAddress _ip = null;

        private UdpClient _udpListener = null;

        public delegate void DataReadDel(WahooData data);
        public delegate void ConnectedDel(String name);

        public DataReadDel DataReceived;
        public ConnectedDel Connected;

        private JavaScriptSerializer _JSSer = null;

        public WahooListener(string hostName, int port, IPAddress ip)
        {
            _hostName = hostName;
            _port = port;
            _ip = ip;

            _listen = true;

            _JSSer = new JavaScriptSerializer();
        }

        public void Init()
        {
            log.Debug("Init");

            WakeUp();

            _listenThr = new Thread(new ThreadStart(ListenThread));
            _listenThr.Start();
        }

        private void WakeUp()
        {
            log.Debug("WakeUp");

            UdpClient sender = new UdpClient(_port);

            sender.Connect(_ip, _port);

            // Sends a message to the host to which you have connected.
            Byte[] sendBytes = Encoding.ASCII.GetBytes("{ \"Status\" : \"OK\"}");

            sender.Send(sendBytes, sendBytes.Length);

            OnConnected(_hostName);
        }

        private void ListenThread()
        {
            log.Debug("ListenThread");

            _udpListener = new UdpClient(_port + 2);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Broadcast, _port);
            String data = String.Empty;

            byte[] rec_array;
            try
            {
                while (_listen)
                {
                    rec_array = _udpListener.Receive(ref ipe);

                    data = Encoding.UTF8.GetString(rec_array, 0, rec_array.Length);
                    WahooData wd = _JSSer.Deserialize<WahooData>(data);

                    OnDataReceived(wd);

                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("ListenThread - error {0}", e.Message);
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

        private void OnConnected(String name)
        {
            if (Connected != null)
            {
                Connected(name);
            }
        }

        private void OnDataReceived(WahooData data)
        {
            if (DataReceived != null)
            {
                DataReceived(data);
            }
        }

        public void Stop()
        {
            log.Debug("Stop");

            _listen = false;
        }
    }
}
