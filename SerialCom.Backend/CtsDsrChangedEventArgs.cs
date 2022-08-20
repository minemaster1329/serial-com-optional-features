using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCom.Backend
{
    public class CtsDsrChangedEventArgs : EventArgs
    {
        public bool _ctsState;
        public bool _dsrState;
    }
}
