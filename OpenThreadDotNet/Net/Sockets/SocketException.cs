using System;

#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.OpenThread.Net.Sockets
{ 
#else
namespace dotNETCore.OpenThread.Net.Sockets
{
#endif
    public class SocketException : Exception
    {        
        public SocketException(string message) : base(message)
        {
        }

        private int _errorCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public SocketException(SocketError errorCode)
        {
            _errorCode = (int)errorCode;
        }

        /// <summary>
        /// Gets the error code that is associated with this exception.
        /// </summary>
        /// <remarks>
        /// <para>The ErrorCode property contains the error code that is associated with the error that caused the exception.</para>
        /// <para>The default constructor for <see cref="SocketException"/> sets the ErrorCode property to the last operating system error that occurred. For more information about socket error codes, see the Windows Sockets version 2 API error code documentation in MSDN.</para>
        /// </remarks>
        public int ErrorCode
        {
            get { return _errorCode; }
        }
    }
}
