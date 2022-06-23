namespace SerialCom.Backend
{
    public class DataReceivedEventArgs: EventArgs
    {
        public string Data { get; }

        public DataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }

    public delegate void DataReceivedEventHandler(object? sender, DataReceivedEventArgs e);
}
