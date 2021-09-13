////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Text;

#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.Net.Sockets;
namespace nanoFramework.OpenThread.Net
{ 
#else
using dotNETCore.OpenThread.Net.Sockets;
namespace dotNETCore.OpenThread.Net
{
#endif
    /// <devdoc>
    ///    <para>Provides an internet protocol (IP) address.</para>
    /// </devdoc>
    [Serializable]
    public class IPAddress : Object
    {
        internal const int IPv6AddressBytes = 16;

        public static readonly IPAddress Any = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);
        public static readonly IPAddress Loopback = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, 0);
        public static readonly IPAddress None = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);

        //public static readonly IPAddress IPv6Any = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);
        //public static readonly IPAddress IPv6Loopback = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, 0);
        //public static readonly IPAddress IPv6None = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);

        private byte[] m_Numbers = new byte[IPv6AddressBytes];

        private int m_HashCode = 0;
        private long m_ScopeId = 0;

        private AddressFamily m_Family = AddressFamily.InterNetworkV6;

        /// <devdoc>
        ///    <para>
        ///       Constructor for an IPv6 Address with a specified Scope.
        ///    </para>
        /// </devdoc>
        public IPAddress(byte[] address, long scopeid)
        {

            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (address.Length != IPv6AddressBytes)
            {
                throw new ArgumentException("dns_bad_ip_address");
            }

            m_Family = AddressFamily.InterNetworkV6;

            Array.Copy(address, m_Numbers, 16);

            if (scopeid < 0 || scopeid > 0x00000000FFFFFFFF)
            {
                throw new ArgumentOutOfRangeException("scopeid");
            }

            m_ScopeId = scopeid;
        }

        /// <devdoc>
        ///    <para>
        ///       Constructor for IPv4 and IPv6 Address.
        ///    </para>
        /// </devdoc>
        public IPAddress(byte[] address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Length != IPv6AddressBytes)
            {
                throw new ArgumentException("dns_bad_ip_address");
            }

            m_Family = AddressFamily.InterNetworkV6;
     
            Array.Copy(address, m_Numbers, 16);
        }

        public static IPAddress Parse(string ipString)
        {
            if (ipString == null)
                throw new ArgumentNullException();

            throw new NotImplementedException();

            //ulong ipAddress = 0L;
            //int lastIndex = 0;
            //int shiftIndex = 0;
            //ulong mask = 0x00000000000000FF;
            //ulong octet = 0L;
            //int length = ipString.Length;

            //for (int i = 0; i < length; ++i)
            //{
            //    // Parse to '.' or end of IP address
            //    if (ipString[i] == '.' || i == length - 1)
            //        // If the IP starts with a '.'
            //        // or a segment is longer than 3 characters or shiftIndex > last bit position throw.
            //        if (i == 0 || i - lastIndex > 3 || shiftIndex > 24)
            //        {
            //            throw new ArgumentException();
            //        }
            //        else
            //        {
            //            i = i == length - 1 ? ++i : i;
            //            octet = (ulong)(ConvertStringToInt32(ipString.Substring(lastIndex, i - lastIndex)) & 0x00000000000000FF);
            //            ipAddress = ipAddress + (ulong)((octet << shiftIndex) & mask);
            //            lastIndex = i + 1;
            //            shiftIndex = shiftIndex + 8;
            //            mask = (mask << 8);
            //        }
            //}

            //return new IPAddress((long)ipAddress);
        }

        public bool Equals(object comparandObj, bool compareScopeId)
        {
            IPAddress addr = comparandObj as IPAddress;

            if (addr == null) return false;

            if (m_Family != addr.m_Family)
            {
                return false;
            }

            for (int i = 0; i < IPv6AddressBytes; i++)
            {
                if (addr.m_Numbers[i] != this.m_Numbers[i])
                    return false;
            }

            if (addr.m_ScopeId == this.m_ScopeId)
                return true;
            else
                return (compareScopeId ? false : true);       
        }

        /// <devdoc>
        ///    <para>
        ///       Compares two IP addresses.
        ///    </para>
        /// </devdoc>
        public override bool Equals(object comparand)
        {
            return Equals(comparand, true);
        }

        public byte[] GetAddressBytes()
        {          
            return m_Numbers;
        }
        
        public override string ToString()
        {         
            int addressStringLength = 16;
            StringBuilder addressString = new StringBuilder(addressStringLength);

            const string numberFormat = "{0:x2}{1:x2}:{2:x2}{3:x2}:{4:x2}{5:x2}:{6:x2}{7:x2}:{8:x2}{9:x2}:{10:x2}{11:x2}:{12:x2}{13:x2}:{14:x2}{15:x2}";
            string address = String.Format(numberFormat, m_Numbers[0], m_Numbers[1], m_Numbers[2], m_Numbers[3], m_Numbers[4], m_Numbers[5], m_Numbers[6], m_Numbers[7], m_Numbers[8], m_Numbers[9], m_Numbers[10], m_Numbers[11], m_Numbers[12], m_Numbers[13], m_Numbers[14], m_Numbers[15]);

//#if NETMF
//            string address = m_Numbers[0].ToString("X2") + m_Numbers[1].ToString("X2") + m_Numbers[2].ToString("X2") + m_Numbers[3].ToString("X2") + m_Numbers[4].ToString("X2") + m_Numbers[5].ToString("X2") + m_Numbers[6].ToString("X2") + m_Numbers[7].ToString("X2") + m_Numbers[8].ToString("X2") + m_Numbers[9].ToString("X2") + m_Numbers[10].ToString("X2") + m_Numbers[11].ToString("X2") + m_Numbers[12].ToString("X2") + m_Numbers[13].ToString("X2") + m_Numbers[14].ToString("X2") + m_Numbers[15].ToString("X2");           
//#else
//            const string numberFormat = "{0:x2}{1:x2}:{2:x2}{3:x2}:{4:x2}{5:x2}:{6:x2}{7:x2}:{8:x2}{9:x2}:{10:x2}{11:x2}:{12:x2}{13:x2}:{14:x2}{15:x2}";
//            string address = String.Format(numberFormat, m_Numbers[0], m_Numbers[1], m_Numbers[2], m_Numbers[3], m_Numbers[4], m_Numbers[5], m_Numbers[6], m_Numbers[7], m_Numbers[8], m_Numbers[9], m_Numbers[10], m_Numbers[11], m_Numbers[12], m_Numbers[13], m_Numbers[14], m_Numbers[15]);
//#endif

            addressString.Append(address);

            if (m_ScopeId != 0)
            {
                addressString.Append('%').Append((uint)m_ScopeId);
            }

            return addressString.ToString();
        }

      

        public override int GetHashCode()
        {
            //int hashCode = -595054056;
            //hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_Numbers);
            //hashCode = hashCode * -1521134295 + m_ScopeId.GetHashCode();
            //hashCode = hashCode * -1521134295 + m_Family.GetHashCode();
            //return hashCode;

            if (m_HashCode == 0)
            {
                const int p = 16777619;
                m_HashCode = -595054056;

                for (int i = 0; i < m_Numbers.Length; i++)
                    m_HashCode = (m_HashCode ^ m_Numbers[i]) * p;

                m_HashCode += m_HashCode << 13;
                m_HashCode ^= m_HashCode >> 7;
                m_HashCode += m_HashCode << 3;
                m_HashCode ^= m_HashCode >> 17;
                m_HashCode += m_HashCode << 5;           
            }
               
            return m_HashCode;
        }
    } 
}


