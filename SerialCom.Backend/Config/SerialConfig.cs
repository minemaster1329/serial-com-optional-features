using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SerialCom.Backend.Config
{
    public class SerialConfig: INotifyPropertyChanged
    {
        public const int NoTimeout = -1;

        private string _terminator = "\n";
        private int _dataBits = 8;
        private string _portName = string.Empty;
        private BaudRateValue _baudRate = BaudRateValue.BR9600;
        private ParityType _parity = ParityType.None;
        private StopBitsCount _stopBits = StopBitsCount.One;
        private FlowControlType _flowControl = FlowControlType.None;
        private int _readTimeout = NoTimeout;
        private int _writeTimeout = NoTimeout;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string PortName
        { 
            get => _portName; 
            set
            {
                if (value != _portName)
                {
                    _portName = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public BaudRateValue BaudRate
        {
            get => _baudRate;
            set
            {
                if (value != _baudRate)
                {
                    _baudRate = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public int DataBits
        {
            get => _dataBits;
            set
            {
                if (value != _dataBits)
                {
                    if (value > 8 || value < 5)
                    {
                        throw new InvalidConfigException("Invalid data bit field length");
                    }
                    _dataBits = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public ParityType Parity
        {
            get => _parity;
            set
            {
                if (value != _parity)
                {
                    _parity = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public StopBitsCount StopBits
        {
            get => _stopBits;
            set
            {
                if (value != _stopBits)
                {
                    _stopBits = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public FlowControlType FlowControl
        {
            get => _flowControl; 
            set
            {
                if (value != _flowControl)
                {
                    _flowControl = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public string Terminator
        {
            get => _terminator;
            set
            {
                if (value != _terminator)
                {
                    if (value.Length > 2 || value.Length < 1)
                    {
                        throw new InvalidConfigException("Invalid terminator length");
                    }
                    _terminator = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public int ReadTimeout
        {
            get => _readTimeout;
            set
            {
                if (value != _readTimeout)
                {
                    if (value < 0 && value != NoTimeout)
                    {
                        throw new ArgumentException("Invalid timeout value");
                    }
                    _readTimeout = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public int WriteTimeout
        {
            get => _writeTimeout;
            set
            {
                if (value != _writeTimeout)
                {
                    if (value < 0 && value != NoTimeout)
                    {
                        throw new ArgumentException("Invalid timeout value");
                    }
                    _writeTimeout = value;
                    _notifyPropertyChanged();
                }
            }
        }

        public SerialConfig(string portName)
        {
            PortName = portName;
        }

        private void _notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
