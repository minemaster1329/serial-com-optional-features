using System.IO.Ports;

namespace SerialCom.Backend.Config
{
    public class SerialConfig
    {
        private string _terminator = string.Empty;
        private int _dataBits = 8;

        internal StopBits stopBits = System.IO.Ports.StopBits.One;
        internal Parity parity = System.IO.Ports.Parity.None;

        public string PortName { get; set; } = string.Empty;

        public BaudRateValue BaudRate { get; set; }

        public int DataBits
        {
            get => _dataBits;
            set
            {
                if (value > 8 || value < 5)
                {
                    throw new InvalidConfigException("Invalid data bit field length");
                }
                _dataBits = value;
            }
        }

        public ParityType Parity
        {
            get => _parityToParityType(parity);
            set => parity = _parityTypeToParity(value);
        }

        public StopBitsCount StopBits
        {
            get => _stopBitsToStopBitsCount(stopBits);
            set => stopBits = _stopBitsCountToStopBits(value);
        }

        public FlowControlType FlowControl { get; set; }

        public string Terminator
        {
            get => _terminator;
            set
            {
                if (value.Length > 2 || value.Length < 1)
                {
                    throw new InvalidConfigException("Invalid terminator length");
                }
                _terminator = value;
            }
        }

        #region Converters

        private static ParityType _parityToParityType(Parity parity)
        {
            switch (parity)
            {
                case System.IO.Ports.Parity.None:
                    return ParityType.None;
                case System.IO.Ports.Parity.Even:
                    return ParityType.Even;
                case System.IO.Ports.Parity.Odd:
                    return ParityType.Odd;
                default:
                    return ParityType.None;
            }
        }

        private static Parity _parityTypeToParity(ParityType parity)
        {
            switch (parity)
            {
                case ParityType.None:
                    return System.IO.Ports.Parity.None;
                case ParityType.Even:
                    return System.IO.Ports.Parity.Even;
                case ParityType.Odd:
                    return System.IO.Ports.Parity.Odd;
                default:
                    return System.IO.Ports.Parity.None;
            }
        }

        private static StopBitsCount _stopBitsToStopBitsCount(StopBits stopBits)
        {
            switch (stopBits)
            {
                case System.IO.Ports.StopBits.None:
                    return StopBitsCount.None;
                case System.IO.Ports.StopBits.One:
                    return StopBitsCount.One;
                case System.IO.Ports.StopBits.Two:
                    return StopBitsCount.Two;
                default:
                    return StopBitsCount.One;
            }
        }

        private static StopBits _stopBitsCountToStopBits(StopBitsCount stopBitsCount)
        {
            switch (stopBitsCount)
            {
                case StopBitsCount.None:
                    return System.IO.Ports.StopBits.None;
                case StopBitsCount.One:
                    return System.IO.Ports.StopBits.One;
                case StopBitsCount.Two:
                    return System.IO.Ports.StopBits.Two;
                default:
                    return System.IO.Ports.StopBits.One;
            }
        }

        #endregion
    }
}
