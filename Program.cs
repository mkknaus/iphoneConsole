using System;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log4Net.config", Watch = true)]

namespace iphoneConsole
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static ManualResetEvent _mre = null;
        static void Main(string[] args)
        {
            log.Debug("Main");

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            ConsoleReader r = new ConsoleReader();
            r.ExitReq += OnExitReq;
            r.Init();


            _mre = new ManualResetEvent(false);
            _mre.WaitOne();

            r.Stop();
        }

        private static void OnExitReq()
        {
            _mre.Set();
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            log.ErrorFormat("Uncaught Exception was thrown! message = {0}", e.Message);
        }
    }
}
