using SerialCom.Backend;
using SerialCom.Backend.Config;
using Xunit.Abstractions;

namespace SerialCom.Test
{
    public class Rs232Test : IDisposable
    {
        private SerialConfig _conf1;
        private Rs232 _port1;

        private SerialConfig _conf2;
        private Rs232 _port2;

        private readonly string _port1Name = "COM1";
        private readonly string _port2Name = "COM2";
        private readonly int _timeout = 2000;

        public Rs232Test(ITestOutputHelper output)
        {
            _conf1 = new SerialConfig(_port1Name)
            {
                ReadTimeout = _timeout,
                WriteTimeout = _timeout
            };
            _port1 = new Rs232(_conf1);

            _conf2 = new SerialConfig(_port2Name)
            {
                ReadTimeout = _timeout,
                WriteTimeout = _timeout
            };
            _port2 = new Rs232(_conf2);
        }

        public void Dispose()
        {
            _port1.Dispose();
            _port2.Dispose();
        }

        [Fact]
        public void ShouldFindCorrectTwoPorts()
        {
            var portNames = Rs232.GetPortNames();

            Assert.Contains(_port1Name, portNames);
            Assert.Contains(_port2Name, portNames);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldSendFromFirstToSecond(string line)
        {
            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line);
            string response = _port2.ReadLine();

            Assert.Equal(line, response);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldNotReceiveDataWithInvalidTerminator(string line)
        {
            _port1.Config.Terminator = @"\/";

            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line);
            string? response = null;

            Assert.Throws<TimeoutException>(() => response = _port2.ReadLine());
            Assert.NotEqual(line, response);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldTransmitDataWithHardwareHandshake(string line)
        {
            _port1.Config.FlowControl = FlowControlType.RtsCts;
            _port2.Config.FlowControl = FlowControlType.RtsCts;

            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line);
            string response = _port2.ReadLine();

            Assert.Equal(line, response);
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { "Test" },
                new object[] { "kn1273128" },
                new object[] { "asdf eqwert" },
                new object[] { "" }
            };
    }
}
