using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace iphoneConsole
{
    class ConsoleReader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SEP = "|";
        private const string ENDLINE = "\r\n";
        private const string VER = "0.1.20120312.2";

        private bool _done = false;
        private Stream _iStream = null; 
        private Stream _oStream = null;

        private ServiceFinder _finder = null;

        const int BUFFER_SIZE = 8124;

        private AsyncCallback _readCallBack = null;
 
        private Thread _readThread = null;

        private WahooListener _wahooListener = null;

        public delegate void ExitReqDel();
        public event ExitReqDel ExitReq;

        public ConsoleReader()
        {
            _done = false;
            _iStream = Console.OpenStandardInput();
            _oStream = Console.OpenStandardOutput();
            _readCallBack = new AsyncCallback(ProcessRead);
            _readThread = new Thread(new ThreadStart(ReadThread));
            _readThread.IsBackground = true;

            _finder = new ServiceFinder();
            _finder.ServiceFound += _finder_ServiceFound;
            _finder.ServiceResolved += _finder_ServiceResolved;
        }

        void _finder_ServiceResolved(Service service)
        {
            _wahooListener = new WahooListener(service.Name, service.Port, service.IP);
            _wahooListener.DataReceived += OnDataReceived;
            _wahooListener.Connected += OnConnected;
            _wahooListener.Init();
        }

        private void OnConnected(String name)
        {
            WriteConnected(name);
        }

        private void OnDataReceived(WahooData data)
        {
            //_oStream.Write(data, 0, data.Length);
            WriteData(data);
        }

        void _finder_ServiceFound(List<string> serviceList)
        {
            //StringBuilder sb = new StringBuilder();
            //foreach (string s in serviceList)
            //{
            //    sb.Append(s);
            //    sb.Append(";");
            //}
            //sb.Append("\r\n");
            //byte[] b = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //_oStream.Write(b, 0, b.Length);
        }
        
        public void Init()
        {
            WriteVersion();
            log.Debug("Init");
            _readThread.Start();
            _finder.Find();
            WriteSearching();
        }

        public void Stop()
        {
            _done = true;

            if (_iStream != null)
            {
                _iStream.Close();
            }

            if (_oStream != null)
            {
                _oStream.Close();
            }

            _wahooListener.Stop();

        }

        private void ReadThread()
        {
            log.Debug("ReadThread - entering");

 	        while(!_done)
            {
                 byte[] buff = new byte[8124];
                 _iStream.BeginRead(buff, 0, BUFFER_SIZE, ProcessRead, new AsyncRead(buff, _iStream));
            }

            log.Debug("ReadThread - exiting");
        }

        private void ProcessRead(IAsyncResult ar)
        {
            AsyncRead asyRead = ar.AsyncState as AsyncRead;

            try
            {
                int bRead = asyRead.AStream.EndRead(ar);

                ProcessMessage(asyRead.Data, bRead);

                //_oStream.Write(asyRead.Data, 0, bRead);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcessMessage(byte[] data, int length)
        {
            String msg = System.Text.Encoding.UTF8.GetString(data,0,length);

            //if(msg.StartsWith(Command.GET_SERVICES))
            //{
            //    //byte[] b = System.Text.Encoding.ASCII.GetBytes("Matt's iPhone\r\n");
            //    //_oStream.Write(b, 0, b.Length);

            //    _finder.Find();
            //}
            //else if (msg.StartsWith(Command.CONNECT))
            //{
            //    string[] s = msg.Split(':');
            //    _finder.Resolve(s[1].Trim());
            //}

            if(msg.Contains(Command.EXIT))
            {
                if(ExitReq!=null)
                {
                    ExitReq();
                }
            }

        }

        private void WriteConnected(String name)
        {
            StringBuilder sb = new StringBuilder(Command.CONNECTED);
            sb.Append(SEP);
            sb.Append(name);
            sb.Append(SEP);
            Console.WriteLine(sb.ToString());
        }

        private void WriteSearching()
        {
            StringBuilder sb = new StringBuilder(Command.SEARCHING);
            sb.Append(SEP);
            sb.Append("WahooService");
            sb.Append(SEP);
            Console.WriteLine(sb.ToString());
        }

        private void WriteSearchFailed()
        {
            StringBuilder sb = new StringBuilder(Command.FAILED);
            sb.Append(SEP);
            sb.Append("WahooService");
            sb.Append(SEP);
            Console.WriteLine(sb.ToString());
        }

        private void WriteVersion()
        {
            StringBuilder sb = new StringBuilder(Command.VERSION);
            sb.Append(SEP);
            sb.Append(VER);
            sb.Append(SEP);
            Console.WriteLine(sb.ToString());
        }

        private void WriteData(WahooData data)
        {
            //cout << "DATA" << SEP << power << SEP << cadence << SEP << hr << SEP << speed << SEP

            StringBuilder sb = new StringBuilder(Command.DATA);
            sb.Append(SEP);
            sb.Append(data.BikePowerInstant);
            sb.Append(SEP);
            sb.Append("1");
            sb.Append(SEP);
            sb.Append(data.HeartrateInstant);
            sb.Append(SEP);
            sb.Append(data.SpeedInstant);
            sb.Append(SEP);
            Console.WriteLine(sb.ToString());
        }
    }
}
