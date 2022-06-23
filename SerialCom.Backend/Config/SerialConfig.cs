using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SerialCom.Backend.Config
{
    public class SerialConfig: INotifyPropertyChanged
    {
        public const int NoTimeout = -1;

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _portName = string.Empty;
        public string PortName
        { 
            get => _portName; 
            set
            {
                if (value != _portName)
                {
                    _portName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private BaudRateValue _baudRate = BaudRateValue.BR9600;
        public BaudRateValue BaudRate
        {
            get => _baudRate;
            set
            {
                if (value != _baudRate)
                {
                    _baudRate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _dataBits = 8;
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
                    NotifyPropertyChanged();
                }
            }
        }

        private ParityType _parity = ParityType.None;
        public ParityType Parity
        {
            get => _parity;
            set
            {
                if (value != _parity)
                {
                    _parity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private StopBitsCount _stopBits = StopBitsCount.One;
        public StopBitsCount StopBits
        {
            get => _stopBits;
            set
            {
                if (value != _stopBits)
                {
                    _stopBits = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private FlowControlType _flowControl = FlowControlType.None;
        public FlowControlType FlowControl
        {
            get => _flowControl; 
            set
            {
                if (value != _flowControl)
                {
                    _flowControl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _terminator = "\n";
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
                    NotifyPropertyChanged();
                }
            }
        }

        private int _readTimeout = NoTimeout;
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
                    NotifyPropertyChanged();
                }
            }
        }

        private int _writeTimeout = NoTimeout;
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
                    NotifyPropertyChanged();
                }
            }
        }

        public SerialConfig(string portName)
        {
            PortName = portName;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
