using System;
using System.Collections;



#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
using nanoFramework.OpenThread.Core;
namespace nanoFramework.OpenThread.Net
{
#else
using dotNETCore.OpenThread.NCP;
using dotNETCore.OpenThread.Core;
namespace dotNETCore.OpenThread.Net
{
#endif
    internal static class NetUtilities
    {
        /// <summary>
        /// This is a simple method for computing the 16-bit one's complement
        /// checksum of a byte buffer. The byte buffer will be padded with
        /// a zero byte if an uneven number.
        /// </summary>
        /// <param name="payLoad">Byte array to compute checksum over</param>
        /// <returns></returns>
        internal static ushort ComputeChecksum(uint sum, byte[] payLoad, bool inverseSum = true)
        {
            uint xsum = sum;
            ushort shortval = 0,
                    hiword = 0,
                    loword = 0;

            // Sum up the 16-bits
            for (int i = 0; i < payLoad.Length / 2; i++)
            {
                hiword = (ushort)(((ushort)payLoad[i * 2]) << 8);
                loword = (ushort)payLoad[(i * 2) + 1];

                shortval = (ushort)(hiword | loword);

                xsum = xsum + (uint)shortval;
            }
            // Pad if necessary
            if ((payLoad.Length % 2) != 0)
            {
                xsum += (uint)payLoad[payLoad.Length - 1];
            }

            xsum = ((xsum >> 16) + (xsum & 0xFFFF));
            xsum = (xsum + (xsum >> 16));

            if (inverseSum == false)
            {
                shortval = (ushort)xsum;
            }
            else
            {
                shortval = (ushort)(~xsum);
            }
            
            return shortval;
        }

        internal static ushort ToLittleEndian(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
#if NETCORE
                Array.Reverse(data);               
#else
               ArrayExtensions.Reverse(data);
#endif
            }

            return BitConverter.ToUInt16(data, 0);
        }

        internal static ushort ToLittleEndian(ushort data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)((((int)data & 0xFF) << 8) | (int)((data >> 8) & 0xFF));
            }

            return data;
        }

        internal static byte[] FromLittleEndian(ushort data)
        {
            byte[] value = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian)
            {
#if NETCORE
                Array.Reverse(value);
#else               
                ArrayExtensions.Reverse(value);
#endif          
            }
            return value;
        }

        internal static IPAddress SpinelIPtoSystemIP(IPv6Address ipAddress)
        {
            if (ipAddress == null) return null;

            return new IPAddress(ipAddress.bytes);
        }

        internal static IPAddress[] SpinelIPtoSystemIP(IPv6Address[] ipAddresses)
        {
            if (ipAddresses == null) return null;

            ArrayList ipAddr = new ArrayList();

            foreach (IPv6Address iPv6Address in ipAddresses)
            {
                ipAddr.Add(SpinelIPtoSystemIP(iPv6Address));
            }

            if (ipAddr.Count > 0)
            {
                return ipAddr.ToArray(typeof(IPAddress)) as IPAddress[];
            }

            return null;
        }
    }
}
