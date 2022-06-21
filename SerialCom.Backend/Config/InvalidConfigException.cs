namespace SerialCom.Backend.Config
{
    public class InvalidConfigException : Exception
    {
        public InvalidConfigException(string? msg = null) : base(msg)
        { }
    }
}
