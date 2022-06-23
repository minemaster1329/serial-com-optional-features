using SerialCom.Backend.Config;
using SerialCom.Backend.Util;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;

namespace SerialCom.Backend
{
    public class Rs232 : IDisposable
    {
        private enum MessageType
        {
            Text,
            Ping
        }
        private static readonly IReadOnlyDictionary<MessageType, string> _messageHeaders =
            new Dictionary<MessageType, string>()
            {
                [MessageType.Text] = @"\t\",
                [MessageType.Ping] = @"\p\",
            };
        
        private SerialPort _port;
        private SerialEnumConverter _converter;

        private bool _disposed = false;

        public event DataReceivedEventHandler? DataReceived;

        private SerialConfig _config;
        public SerialConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                _initPortFromConfig();
            }
        }

        public bool IsOpen{ get => _port.IsOpen; }

        public Rs232(string portName) : this(new SerialConfig(portName))
        { }

        public Rs232(SerialConfig config)
        {
            _config = config;
            Config.PropertyChanged += _configChanged;

            _port = new SerialPort();
            _converter = new SerialEnumConverter();

            _initPortFromConfig();

            _port.DataReceived += _handleDataReceived;
        }

        public void Open()
        {
            _port.Open();
        }

        public void Close()
        {
            _port.Close();
        }

        public void WriteLine(string line)
        {
            _port.WriteLine(_messageHeaders[MessageType.Text] + line);
        }

        public async Task WriteLineAsync(string line)
        {
            await Task.Run(() => WriteLine(line));
        }

        private string _readPing()
        {
            string data = _port.ReadLine();
            if (!data.StartsWith(_messageHeaders[MessageType.Ping]))
            {
                throw new Exception("Invalid ping header"); // TODO
            }
            return data.Substring(_messageHeaders[MessageType.Ping].Length);
        }

        private void _writePing()
        {
            _port.WriteLine(_messageHeaders[MessageType.Ping]);
        }

        public long Ping(Action? callback = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _writePing();
            
            _port.DataReceived -= _handleDataReceived;
            string resp = _readPing();
            stopwatch.Stop();
            _port.DataReceived += _handleDataReceived;

            callback?.Invoke();
            return stopwatch.ElapsedMilliseconds;
        }

        public async Task<long> PingAsync()
        {
            return await Task.Run(() => Ping());
        }

        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        private void _configChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.PropertyName))
            {
                // TODO: Check if the change can be made.

                switch (args.PropertyName)
                {
                    // Simple:
                    case nameof(Config.PortName):
                        _port.PortName = Config.PortName;
                        break;
                    case nameof(Config.BaudRate):
                        _port.BaudRate = (int)Config.BaudRate;
                        break;
                    case nameof(Config.DataBits):
                        _port.DataBits = Config.DataBits;
                        break;
                    case nameof(Config.Terminator):
                        _port.NewLine = Config.Terminator;
                        break;
                    
                    // Enums:
                    case nameof(Config.Parity):
                        _port.Parity = _converter.ParityTypeToParity(Config.Parity);
                        break;
                    case nameof(Config.StopBits):
                        _port.StopBits = _converter.StopBitsCountToStopBits(Config.StopBits);
                        break;
                    case nameof(Config.FlowControl):
                        _port.Handshake = _converter.FlowControlTypeToHandshake(Config.FlowControl);
                        break;
                    
                    // Timeouts:
                    case nameof(Config.ReadTimeout):
                        _port.ReadTimeout = Config.ReadTimeout;
                        break;
                    case nameof(Config.WriteTimeout):
                        _port.ReadTimeout = Config.WriteTimeout;
                        break;
                }
            }
        }

        private void _initPortFromConfig()
        {
            _port.PortName = Config.PortName;
            _port.BaudRate = (int)Config.BaudRate;
            _port.DataBits = Config.DataBits;
            _port.NewLine = Config.Terminator;

            _port.Parity = _converter.ParityTypeToParity(Config.Parity);
            _port.StopBits = _converter.StopBitsCountToStopBits(Config.StopBits);
            _port.Handshake = _converter.FlowControlTypeToHandshake(Config.FlowControl);

            _port.ReadTimeout = Config.ReadTimeout;
            _port.WriteTimeout = Config.WriteTimeout;
        }

        private void _handleDataReceived(object? sender, SerialDataReceivedEventArgs args)
        {
            string data = _port.ReadLine();
            if (data.StartsWith(_messageHeaders[MessageType.Ping]))
            {
                _writePing();
            }
            if (data.StartsWith(_messageHeaders[MessageType.Text]))
            {
                string dataToSend = data.Substring(_messageHeaders[MessageType.Text].Length);
                DataReceived?.Invoke(this, new DataReceivedEventArgs(dataToSend));
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_port.IsOpen)
                    {
                        _port.Close();
                    }
                    _port.DataReceived -= _handleDataReceived;
                    _port.Dispose();
                }
                _disposed = true;
            }
        }

        ~Rs232()
        {
            Dispose(false);
        }

        #endregion
    }
}
