namespace SerialCom.Backend
{
    public class DataReceivedEventArgs: EventArgs
    {
        public string Data { get; }
        public Exception? Exception { get; }

        public DataReceivedEventArgs(string data, Exception? exception = null)
        {
            Data = data;
            Exception = exception;
        }
    }
}
