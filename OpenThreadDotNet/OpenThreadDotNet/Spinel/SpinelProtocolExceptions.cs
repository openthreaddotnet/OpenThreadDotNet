using System;

#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
namespace nanoFramework.OpenThread.Spinel
{ 
#else
using dotNETCore.OpenThread.NCP;
namespace dotNETCore.OpenThread.Spinel
{
#endif  
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
