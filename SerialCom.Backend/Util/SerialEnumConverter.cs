using SerialCom.Backend.Config;
using System.IO.Ports;

namespace SerialCom.Backend.Util
{
    internal class SerialEnumConverter
    {
        public ParityType ParityToParityType(Parity parity)
        {
            switch (parity)
            {
                case Parity.None:
                    return ParityType.None;
                case Parity.Even:
                    return ParityType.Even;
                case Parity.Odd:
                    return ParityType.Odd;
                default:
                    return ParityType.None;
            }
        }

        public Parity ParityTypeToParity(ParityType parity)
        {
            switch (parity)
            {
                case ParityType.None:
                    return Parity.None;
                case ParityType.Even:
                    return Parity.Even;
                case ParityType.Odd:
                    return Parity.Odd;
                default:
                    return Parity.None;
            }
        }

        public StopBitsCount StopBitsToStopBitsCount(StopBits stopBits)
        {
            switch (stopBits)
            {
                case StopBits.One:
                    return StopBitsCount.One;
                case StopBits.Two:
                    return StopBitsCount.Two;
                default:
                    return StopBitsCount.One;
            }
        }

        public StopBits StopBitsCountToStopBits(StopBitsCount stopBitsCount)
        {
            switch (stopBitsCount)
            {
                case StopBitsCount.One:
                    return StopBits.One;
                case StopBitsCount.Two:
                    return StopBits.Two;
                default:
                    return StopBits.One;
            }
        }

        public Handshake FlowControlTypeToHandshake(FlowControlType flowControl)
        {
            switch (flowControl)
            {
                case FlowControlType.None:
                    return Handshake.None;
                case FlowControlType.RtsCts:
                    return Handshake.RequestToSend;
                case FlowControlType.DtrDsr:
                    return Handshake.None;
                case FlowControlType.Software:
                    return Handshake.XOnXOff;
                default:
                    return Handshake.None;
            }
        }
    }
}
