using SerialCom.Backend.Config;
using SerialCom.Backend.Util;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;

namespace SerialCom.Backend
{
    public class Rs232 : IDisposable
    {
        private SerialPort _port;
        private SerialEnumConverter _converter;
        private SerialConfig _config;

        private bool _disposed = false;

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
            _port.WriteLine(line);
        }

        public string ReadLine()
        {
            return _port.ReadLine();
        }

        public async Task WriteLineAsync(string line)
        {
            await Task.Run(() => _port.WriteLine(line));
        }

        public async Task<string> ReadLineAsync()
        {
            return await Task.Run(() => _port.ReadLine());
        }

        public long Ping()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            WriteLine("");
            ReadLine();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
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
