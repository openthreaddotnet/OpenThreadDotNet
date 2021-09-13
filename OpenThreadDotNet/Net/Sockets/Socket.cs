using System;
using System.Threading;

#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.Net.IPv6;
namespace nanoFramework.OpenThread.Net.Sockets
{
#else
using dotNETCore.OpenThread.Net.IPv6;
namespace dotNETCore.OpenThread.Net.Sockets
{
#endif
    public enum SelectMode
    {
        SelectRead = 0,
        SelectWrite = 1,
        SelectError = 2
    }

    public class Socket : IDisposable
    {
        internal class ReceivedPacketBuffer
        {
            internal readonly int UDPPacketSize = 1232;// 
            public byte[] Buffer;
            public Int32 BytesReceived;
            public bool IsEmpty;
            public object LockObject;
        }

        protected const Int32 SELECT_MODE_READ = 0;
        protected const Int32 SELECT_MODE_WRITE = 1;
        protected const Int32 SELECT_MODE_ERROR = 2;

        protected bool _isDisposed = false;

        protected const UInt16 IPPortAny = 0x0000;
        protected readonly IPAddress IPv6Any = IPAddress.Any;

        public IPAddress sourceIpAddress = IPAddress.Any;
        public ushort sourcePort = IPPortAny;

        internal IPAddress destinationIpAddress = IPAddress.Any;      
        internal ushort destinationPort = IPPortAny;

        private  ReceivedPacketBuffer receivedPacketBuffer = new ReceivedPacketBuffer();
        private AutoResetEvent receivedPacketBufferFilledEvent = new AutoResetEvent(false);

        private bool _sourceIpAddressAndPortAssigned = false;

        public int ReceiveTimeout { get; set; }
        protected bool Active { get; set; }

        public int Available
        {
            get
            {              
                return receivedPacketBuffer.BytesReceived;
            }
        }

        //public int ReceiveTimeout
        //{
        //    get
        //    {
        //        return m_recvTimeout;
        //    }

        //    set
        //    {
        //        if (value < Timeout.Infinite) throw new ArgumentOutOfRangeException();

        //        // desktop implementation treats 0 as infinite
        //        m_recvTimeout = ((value == 0) ? Timeout.Infinite : value);
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the Socket class using the specified address family, socket type and protocol.
        /// </summary>
        /// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
        /// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
        /// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
        /// <remarks>
        /// The addressFamily parameter specifies the addressing scheme that the <see cref="Socket"/> class uses, the socketType parameter specifies the type of the <see cref="Socket"/> class, 
        /// and the protocolType parameter specifies the protocol used by <see cref="Socket"/>. The three parameters are not independent. Some address families restrict which 
        /// protocols can be used with them, and often the <see cref="Socket"/> type is implicit in the protocol. If the combination of address family, <see cref="Socket"/> type, and protocol type
        /// esults in an invalid Socket, this constructor throws a SocketException.
        /// </remarks>
        public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            InitializeSocket();
        }

        public Socket()
        {
            InitializeSocket();
        }

        private void InitializeSocket()
        {
            InitializeReceivedPacketBuffer(receivedPacketBuffer);
        }

        /// <summary>
        /// Establishes a connection to a remote host.
        /// </summary>
        /// <param name="remoteEP">An <see cref="EndPoint"/> that represents the remote device.</param>        
        public void Connect(EndPoint remoteEP)
        {
            IPEndPoint iPEndPoint = (IPEndPoint)remoteEP;
            Connect(iPEndPoint.Address, iPEndPoint.Port);
        }

        public void Connect(IPAddress ipAddress, int ipPort)
        {
            if (!_sourceIpAddressAndPortAssigned)
                Bind(IPv6Any, IPPortAny);

            // UDP is connectionless, so the Connect function sets the default destination IP address and port values.
            destinationIpAddress = ipAddress;
            destinationPort = (ushort) ipPort;
        }

        public void Bind(EndPoint localEP)
        {
            EndPoint endPointSnapshot = localEP;
            IPEndPoint ipSnapshot = localEP as IPEndPoint;
            Bind(ipSnapshot.Address, (ushort)ipSnapshot.Port);
        }
            
        public void Bind(IPAddress ipAddress, ushort ipPort)
        {
            if (_sourceIpAddressAndPortAssigned)
                throw new SocketException("Socket is connected.");
             
            _sourceIpAddressAndPortAssigned = true;

            // if ipAddress is IP_ADDRESS_ANY, then change it to to our actual ipAddress.
            if (ipAddress.Equals (IPv6Any))
            {
                sourceIpAddress = NetworkInterface.IPAddress;
            }
            else
            {
                sourceIpAddress = ipAddress;
            }

            if (ipPort == IPPortAny)
            {
                sourcePort = NetworkInterface.GetEphemeralPort();
            }
            else
            {
                sourcePort = ipPort;
            }
                

            // verify that this source IP address is correct
            if (sourceIpAddress != NetworkInterface.IPAddress)
                throw new SocketException("Source address is not correct.");

            NetworkInterface.CreateSocket(this);
            ReceiveTimeout = 5000;
        }

        public int Send(byte[] buffer)
        {
             return Send(buffer, buffer != null ? buffer.Length : 0);
        }

        public int Send(byte[] buffer, int length)
        {
            // make sure that a default destination IPEndpoint has been configured.
            if ((destinationIpAddress == IPv6Any) || (destinationPort == IPPortAny))
                throw new SocketException("Socket is not connected.");

            UdpDatagram udpDatagram = new UdpDatagram();

            udpDatagram.DestinationPort = destinationPort;
            udpDatagram.SourcePort = sourcePort;
            udpDatagram.AddPayload(buffer);            
            udpDatagram.Checksum = 0;


            IPv6Packet packetUDP = new IPv6Packet();
            packetUDP.SourceAddress = sourceIpAddress;
            packetUDP.DestinationAddress = destinationIpAddress;
            packetUDP.NextHeader =IPv6Protocol.Udp;
            packetUDP.Payload = udpDatagram;
            packetUDP.PayloadLength = udpDatagram.Length;

          
            IPv6PseudoHeader ipv6PseudoHeader = new IPv6PseudoHeader(packetUDP.SourceAddress, packetUDP.DestinationAddress, packetUDP.PayloadLength, (byte)packetUDP.NextHeader);
            ushort checkSum = ipv6PseudoHeader.GetCheckSum();
            checkSum = NetUtilities.ComputeChecksum(checkSum, udpDatagram.ToBytes(), true);

            udpDatagram.Checksum = checkSum;
          
            NetworkInterface.Send(packetUDP.ToBytes());

            //just workaround for compatability with NETMF socket

            return length;
        }

        public void PacketHandler(IPv6Packet ipv6Packet)
        {
            UdpDatagram udpDatagram = (UdpDatagram)ipv6Packet.Payload;
            destinationIpAddress = ipv6Packet.SourceAddress;
            destinationPort = udpDatagram.SourcePort;

            /* if we do not have enough room for the incoming frame, discard it */
            if (receivedPacketBuffer.IsEmpty == false)
                return;

            lock (receivedPacketBuffer.LockObject)
            {
                int bytesReceived = udpDatagram.Payload.Length;

                Array.Copy(udpDatagram.Payload, receivedPacketBuffer.Buffer, bytesReceived);
                receivedPacketBuffer.IsEmpty = false;
                receivedPacketBuffer.BytesReceived = bytesReceived;

                receivedPacketBufferFilledEvent.Set();
            }
        }

        public bool Poll(int microSeconds, SelectMode mode)
        {
            switch (mode)
            {
                case SelectMode.SelectRead:
                    /* [source: MSDN documentation]
                     * return true when:
                     *   - if Listen has been called and a connection is pending
                     *   - if data is available for reading
                     *   - if the connection has been closed, reset, or terminated
                     * otherwise return false */
                    {
                        /* TODO: check if listen has been called and a connection is pending */
                        //return true;

                        if (receivedPacketBuffer.BytesReceived > 0)
                            return true;

                        /* TODO: check if connection has been closed, reset, or terminated */
                        //return true;

                        /* TODO: only check _isDisposed if the connection hasn't been closed/reset/terminated; those other cases should return TRUE */
                        if (_isDisposed) return false;

                        // in all other circumstances, return false.
                        return false;
                    }
                case SelectMode.SelectWrite:
                    /* [source: MSDN documentation]
                     * return true when:
                     *   - processing a Connect and the connection has succeeded
                     *   - if data can be sent
                     * otherwise return false */
                    {
                        if (_isDisposed) return false;
                        
                        if ((destinationIpAddress != IPv6Any) && (destinationPort != 0))
                            return true;
                        else
                            return false;
                    }
                case SelectMode.SelectError:
                    /* [source: MSDN documentation]
                     * return true when:
                     *   - if processing a Connect that does not block--and the connection has failed
                     *   - if OutOfBandInline is not set and out-of-band data is available 
                     * otherwise return false */
                    {
                        if (_isDisposed) return false;

                        return false;
                    }
                default:
                    {
                        // the following line should never be executed
                        return false;
                    }
            }
        }

        public byte[] ReceiveFrom(IPEndPoint remoteEPt)
        {           
            throw new NotImplementedException();
        }

        public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
        {
            throw new NotImplementedException();
        }

        public int Receive(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int SendTo(byte[] buffer, EndPoint remoteEP)
        {
            throw new NotImplementedException();
        }

        public byte[] Receive(ref IPEndPoint remoteEP)
        {
            if (!receivedPacketBufferFilledEvent.WaitOne(ReceiveTimeout, false))
            {
                throw new SocketException("UDP recieve data timeout.");
            }

            byte[] newBuffer = new byte[receivedPacketBuffer.BytesReceived];

            lock (receivedPacketBuffer.LockObject)
            {                                             
                Array.Copy(receivedPacketBuffer.Buffer, 0, newBuffer, 0, receivedPacketBuffer.BytesReceived);
                // now empty our datagram buffer
                InitializeReceivedPacketBuffer(receivedPacketBuffer);               
            }

            IPEndPoint endPoint = new IPEndPoint(destinationIpAddress, destinationPort);

            remoteEP = endPoint;

            return newBuffer;
        }

        internal void InitializeReceivedPacketBuffer(ReceivedPacketBuffer buffer)
        {         
            if (buffer.Buffer == null)
                buffer.Buffer = new byte[buffer.UDPPacketSize];
            buffer.BytesReceived = 0;

            buffer.IsEmpty = true;
            if (buffer.LockObject == null)
                buffer.LockObject = new object();
        }

        public void Close()
        {
            _isDisposed = true;

            NetworkInterface.CloseSocket(this);
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}
