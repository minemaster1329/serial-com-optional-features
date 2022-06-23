using SerialCom.Backend;
using SerialCom.Backend.Config;

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

        public Rs232Test()
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
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            _port2.DataReceived += (sender, args) =>
            {
                response = args.Data;
                waitHandle.Set();
            };

            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line);

            Assert.True(waitHandle.WaitOne(5000, false));
            Assert.Equal(line, response);
        }

        [Fact]
        public void ShouldReadAfterSend()
        {
            const string line1 = "Test";
            const string line2 = "MoRbIuS";
            AutoResetEvent waitHandle1 = new AutoResetEvent(false);
            AutoResetEvent waitHandle2 = new AutoResetEvent(false);
            string? resp1 = null, resp2 = null;

            _port1.DataReceived += (sender, args) =>
            {
                resp2 = args.Data;
                waitHandle1.Set();
            };
            _port2.DataReceived += (sender, args) =>
            {
                resp1 = args.Data;
                waitHandle2.Set();
            };

            _port1.Open();
            _port2.Open();
            _port1.WriteLine(line1);
            _port2.WriteLine(line2);

            Assert.True(waitHandle1.WaitOne(5000, false));
            Assert.True(waitHandle2.WaitOne(5000, false));

            Assert.NotNull(resp1);
            Assert.NotNull(resp2);

            Assert.Equal(line1, resp1);
            Assert.Equal(line2, resp2);
        }
    }
}
