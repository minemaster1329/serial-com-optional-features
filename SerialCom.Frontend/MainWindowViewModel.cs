using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Windows;
using SerialCom.Backend;
using SerialCom.Backend.Config;
using DataReceivedEventArgs = SerialCom.Backend.DataReceivedEventArgs;

namespace SerialCom.Frontend
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly int[] _baudRatesValues = new int[]
        {
            150, 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 31250, 38400, 56000, 57600, 76800, 115200
        };

        private readonly int[] _dataBits = new int[]
        {
            7, 8
        };

        private bool _connected = false;
        private Rs232? _portRs232;
        private int _selectedTerminatorIndex = -1;
        private readonly ObservableCollection<string> _receivedList;
        private bool _ctsState;
        private bool _dsrState;

        public event PropertyChangedEventHandler PropertyChanged;
        public RelaySyncCommand ConnectButtonCommand { get; set; }
        public RelaySyncCommand PingButtonCommand { get; set; }
        public RelaySyncCommand SendMessageButtonCommand { get; set; } 
        public RelaySyncCommand ClearMessageInput { get; set; }
        public bool CtsState
        {
            get => _ctsState;
            set
            {
                _ctsState = value;
                OnPropertyChanged();
            }
        }

        public bool DsrState
        {
            get => _dsrState;
            set
            {
                _dsrState = value;
                OnPropertyChanged();
            }
        }
        public string Message { get; set; }
        public bool Connected
        {
            get => _connected;
            set
            {
                _connected = value;
                OnPropertyChanged();
                OnPropertyChanged("ConnectButtonText");
                OnPropertyChanged(nameof(ConfigEnabled));
            }
        }
        public bool ConfigEnabled { get => !_connected; }
        public string CustomTerminator { get; set; }

        public int SelectedPort { get; set; } = -1;
        public int SelectedBaudRateValue { get; set; } = -1;
        public int SelectedDataBitsCount { get; set; } = -1;
        public int SelectedFlowControlValues { get; set; } = -1;
        public int SelectedParity { get; set; } = -1;
        public int SelectedStopBitsCount { get; set; } = -1;

        public int SelectedTerminator
        {
            get => _selectedTerminatorIndex;
            set
            {
                _selectedTerminatorIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("CustomTerminatorInputEnabled");
            }
        }


        public ObservableCollection<string> ReceivedList
        {
            get => _receivedList;
        }
        public ObservableCollection<string> PortNames { get; } = new ObservableCollection<string>();

        public int[] BaudRateValues => _baudRatesValues;
        public string[] FlowControlValues => Enum.GetNames(typeof(FlowControlType));
        public string[] StopBitsCountValues => Enum.GetNames(typeof(StopBitsCount));
        public string[] ParityValues => Enum.GetNames(typeof(ParityType));
        public bool CustomTerminatorInputEnabled => SelectedTerminator == 4;

        public MainWindowViewModel()
        {
            _portRs232 = null;
            ConnectButtonCommand = new RelaySyncCommand(ConnectButtonCommandExecute);
            PingButtonCommand = new RelaySyncCommand(SendPing);
            SendMessageButtonCommand = new RelaySyncCommand(SendMessageButtonCommandExecute);
            ClearMessageInput = new RelaySyncCommand(ClearMessageInputCommandExecute);
            _receivedList = new ObservableCollection<string>();
            GetAllRS232PortsNames();
        }

        private void ClearMessageInputCommandExecute(object parameter)
        {
            Message = "";
        }

        private void SendMessageButtonCommandExecute(object parameter)
        {
            if (Connected)
            {
                _portRs232?.WriteLine(Message);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ConnectButtonCommandExecute(object parameter)
        {
            if (Connected)
            {
                _portRs232?.Close();
                _portRs232?.Dispose();
                Connected = false;
            }
            else
            {
                if (CheckConfig())
                {
                    string serialPortName = PortNames[SelectedPort];
                    SerialConfig serialConfig = new SerialConfig(serialPortName);
                    serialConfig.BaudRate = (BaudRateValue)_baudRatesValues[SelectedBaudRateValue];
                    serialConfig.DataBits = _dataBits[SelectedDataBitsCount];
                    serialConfig.FlowControl = (FlowControlType)SelectedFlowControlValues;
                    serialConfig.Parity = (ParityType)SelectedParity;
                    serialConfig.ReadTimeout = 500;
                    serialConfig.WriteTimeout = 500;
                    serialConfig.StopBits = (StopBitsCount)SelectedStopBitsCount;
                    serialConfig.Terminator = SelectedTerminator switch
                    {
                        0 => "\0",
                        1 => "\r",
                        2 => "\n",
                        3 => "\r\n",
                        4 => CustomTerminator,
                        _ => "\n"
                    };

                    _portRs232 = new Rs232(serialConfig);
                    try
                    {
                        _portRs232.Open();
                        _portRs232.DataReceived += HandleMessageReceived;
                        Connected = true;
                        CtsState = false;
                        DsrState = false;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid config");
                }
            }
        }

        private void SendPing(object parameter)
        {
            if (Connected)
            {
                _portRs232?.Ping(PingPortCallback);
            }
        }

        private void PingPortCallback(long obj)
        {
            MessageBox.Show(obj.ToString());
        }

        private void GetAllRS232PortsNames()
        {
            PortNames.Clear();
            foreach (string portName in SerialPort.GetPortNames())
            {
                PortNames.Add(portName);
            }
        }

        private void HandleMessageReceived(object? sender, DataReceivedEventArgs args)
        {
            if (args.Exception is not null)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    ReceivedList.Add($"Error: {args.Exception.Message}");
                });
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    ReceivedList.Add(args.Data);
                });
            }
        }

        private void HandleCtsDsrChanged(object sender, CtsDsrChangedEventArgs e)
        {

        }

        private bool CheckConfig()
        {
            var configValues = new List<int>
            {
                SelectedPort,
                SelectedBaudRateValue,
                SelectedDataBitsCount,
                SelectedFlowControlValues,
                SelectedParity,
                SelectedStopBitsCount,
                SelectedTerminator
            };

            return !configValues.Contains(-1);
        }
    }
}
