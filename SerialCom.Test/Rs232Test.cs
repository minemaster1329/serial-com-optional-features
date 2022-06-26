using SerialCom.Backend;
using SerialCom.Backend.Config;

namespace SerialCom.Test
{
    public class Rs232Test : IDisposable
    {
        private SerialConfig _conf1;
        private Rs232 _port1;
        private readonly AutoResetEvent _wait1;

        private SerialConfig _conf2;
        private Rs232 _port2;
        private readonly AutoResetEvent _wait2;

        private readonly string _port1Name = "COM6";
        private readonly string _port2Name = "COM7";
        private readonly int _timeout = 2000;

        public Rs232Test()
        {
            _conf1 = new SerialConfig(_port1Name)
            {
                ReadTimeout = _timeout,
                WriteTimeout = _timeout
            };
            _port1 = new Rs232(_conf1);
            _wait1 = new AutoResetEvent(false);

            _conf2 = new SerialConfig(_port2Name)
            {
                ReadTimeout = _timeout,
                WriteTimeout = _timeout
            };
            _port2 = new Rs232(_conf2);
            _wait2 = new AutoResetEvent(false);
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

        [Fact]
        public void ShouldPongAfterPing()
        {
            long? time = null;
            _port1.Open();
            _port2.Open();

            time = _port1.Ping();

            Assert.NotNull(time);
        }

        [Fact]
        public void ShouldReadEventMessage()
        {
            const string line = "Test";
            string? response = null;
            _port2.DataReceived += (sender, args) =>
            {
                response = args.Data;
                _wait1.Set();
            };

            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line);

            Assert.True(_wait1.WaitOne(5000, false));
            Assert.Equal(line, response);
        }

        [Fact]
        public void ShouldReadAfterSend()
        {
            const string line1 = "Test";
            const string line2 = "MoRbIuS";
            string? resp1 = null, resp2 = null;

            _port1.DataReceived += (sender, args) =>
            {
                resp2 = args.Data;
                _wait1.Set();
            };
            _port2.DataReceived += (sender, args) =>
            {
                resp1 = args.Data;
                _wait2.Set();
            };

            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line1);
            _port2.WriteLine(line2);

            Assert.True(_wait1.WaitOne(5000, false));
            Assert.True(_wait2.WaitOne(5000, false));

            Assert.NotNull(resp1);
            Assert.NotNull(resp2);

            Assert.Equal(line1, resp1);
            Assert.Equal(line2, resp2);
        }

        [Fact]
        public void ShouldTransmitWithDtr()
        {
            const string line1 = "Test";
            const string line2 = "MoRbIuS";
            string? resp1 = null, resp2 = null;
            _port1.Config.FlowControl = FlowControlType.DtrDsr;
            _port1.DataReceived += (sender, args) =>
            {
                resp2 = args.Data;
                _wait1.Set();
            };

            _port2.Config.FlowControl = FlowControlType.DtrDsr;
            _port2.DataReceived += (sender, args) =>
            {
                resp1 = args.Data;
                _wait2.Set();
            };

            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line1);
            _port2.WriteLine(line2);

            Assert.True(_wait1.WaitOne(5000, false));
            Assert.True(_wait2.WaitOne(5000, false));

            Assert.NotNull(resp1);
            Assert.NotNull(resp2);

            Assert.Equal(line1, resp1);
            Assert.Equal(line2, resp2);
        }

        [Fact]
        public void ShouldPongAfterPingWithDtr()
        {
            _port1.Config.FlowControl = FlowControlType.DtrDsr;
            _port2.Config.FlowControl = FlowControlType.DtrDsr;
            long? time = null;
            
            _port1.Open();
            _port2.Open();

            time = _port1.Ping();

            Assert.NotNull(time);
        }
    }
}
