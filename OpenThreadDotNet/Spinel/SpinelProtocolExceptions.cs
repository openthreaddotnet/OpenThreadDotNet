using System;

namespace nanoFramework.OpenThread.Spinel
{
    public class SpinelProtocolExceptions : Exception
    {
        public SpinelProtocolExceptions(string message)
            : base(message)
        {
        }
    }

    public class SpinelFormatException : Exception
    {
        public SpinelFormatException(string message)
            : base(message)
        {
        }
    }
}
