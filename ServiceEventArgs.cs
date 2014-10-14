using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iphoneConsole
{
    public class ServiceEventArgs : EventArgs
    {
        private Service _service;

        public ServiceEventArgs(Service s)
        {
            _service = s;
        }

        public Service Service
        {
            get { return _service; }
            set { _service = value; }
        }
    }
}
