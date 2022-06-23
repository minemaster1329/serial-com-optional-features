namespace SerialCom.Backend.Config
{
    public enum ParityType
    {
        None,
        Even,
        Odd
    }

    public enum FlowControlType
    {
        None,
        RtsCts,
        DtrDsr,
        Software
    }

    public enum BaudRateValue
    {
        BR150 = 150,
        BR300 = 300,
        BR600 = 600,
        BR1200 = 1200,
        BR1800 = 1800,
        BR2400 = 2400,
        BR4800 = 4800,
        BR7200 = 7200,
        BR9600 = 9600,
        BR14400 = 14400,
        BR19200 = 19200,
        BR31250 = 31250,
        BR38400 = 38400,
        BR56000 = 56000,
        BR57600 = 57600,
        BR76800 = 76800,
        BR115200 = 115200,
    }

    public enum StopBitsCount
    {
        None,
        One,
        Two
    }
}