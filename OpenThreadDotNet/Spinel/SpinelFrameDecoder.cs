using System;
using System.Collections;
using System.Text;

#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
using nanoFramework.OpenThread.Core;
namespace nanoFramework.OpenThread.Spinel
{
#else
using dotNETCore.OpenThread.NCP;
using dotNETCore.OpenThread.Core;
namespace dotNETCore.OpenThread.Spinel
{
#endif
    public static class SpinelFrameDecoder
    {
        private const byte SpinelHeaderFlag = 0x80;

        internal static void DecodeFrame(byte[] frameIn, out FrameData frameData)
        {

            SpinelDecoder mDecoder = new SpinelDecoder();
            object ncpResponse = null;
            mDecoder.Init(frameIn);

            byte header = mDecoder.FrameHeader;

            if ((SpinelHeaderFlag & header) != SpinelHeaderFlag)
            {
                throw new SpinelFormatException("Header parsing error.");
            }

            uint command = mDecoder.FrameCommand;
            uint properyId = mDecoder.FramePropertyId;

            //if (properyId == SpinelProperties.SPINEL_PROP_THREAD_CHILD_TABLE)
            //{
            //    if (command == SpinelCommands.RSP_PROP_VALUE_INSERTED || command == SpinelCommands.RSP_PROP_VALUE_REMOVED)
            //    {
            //        return null;
            //    }
            //}

            object tempObj = null;

            switch (properyId)
            {
                //********************************************************************************
                //
                //          Core properties
                //
                //********************************************************************************

                case SpinelProperties.SPINEL_PROP_NCP_VERSION:
                    ncpResponse = mDecoder.ReadUtf8();
                    break;

                case SpinelProperties.SPINEL_PROP_LAST_STATUS:
                    ncpResponse = mDecoder.ReadUintPacked();
                    break;

                case SpinelProperties.SPINEL_PROP_INTERFACE_TYPE:
                    ncpResponse = mDecoder.ReadUintPacked();
                    break;

                case SpinelProperties.SPINEL_PROP_VENDOR_ID:
                    ncpResponse = mDecoder.ReadUintPacked();
                    break;

                case SpinelProperties.SPINEL_PROP_PROTOCOL_VERSION:

                    tempObj = mDecoder.ReadFields("ii");

                    if (tempObj != null)
                    {
                        ArrayList protocol = (ArrayList)tempObj;
                        ncpResponse = (uint[])protocol.ToArray(typeof(uint));
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_CAPS:

                    tempObj = mDecoder.ReadFields("A(i)");

                    if (tempObj != null)
                    {
                        ArrayList caps = (ArrayList)tempObj;
                        Capabilities[] capsArray = new Capabilities[caps.Count];
                        int index = 0;

                        foreach (var capsValue in caps)
                        {
                            capsArray[index] = (Capabilities)(uint)(capsValue);
                            index++;
                        }

                        ncpResponse = capsArray;
                    }

                    break;

                //********************************************************************************
                //
                //          Phy properties
                //
                //********************************************************************************
                case SpinelProperties.SPINEL_PROP_PHY_TX_POWER:
                    ncpResponse = mDecoder.Readint8();
                    break;

                //********************************************************************************
                //
                //          Net properties
                //
                //********************************************************************************

                case SpinelProperties.SPINEL_PROP_NET_NETWORK_NAME:
                    ncpResponse = mDecoder.ReadUtf8();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_SAVED:
                    ncpResponse = mDecoder.ReadBool();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_IF_UP:
                    ncpResponse = mDecoder.ReadBool();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_STACK_UP:
                    ncpResponse = mDecoder.ReadBool();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING:
                    ncpResponse = mDecoder.ReadBool();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_KEY_SEQUENCE_COUNTER:
                    ncpResponse = mDecoder.ReadUint32();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_XPANID:
                    ncpResponse = mDecoder.ReadData();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_ROLE:
                    ncpResponse = mDecoder.ReadUint8();
                    break;

                case SpinelProperties.SPINEL_PROP_NET_NETWORK_KEY:
                    ncpResponse = mDecoder.ReadData();
                    break;

                //********************************************************************************
                //
                //          Mac properties
                //
                //********************************************************************************

                case SpinelProperties.SPINEL_PROP_MAC_SCAN_STATE:
                    ncpResponse = mDecoder.ReadUint8();
                    break;

                case SpinelProperties.SPINEL_PROP_MAC_SCAN_MASK:
                    tempObj = mDecoder.ReadFields("A(C)");

                    if (tempObj != null)
                    {
                        ArrayList channels = (ArrayList)tempObj;
                        ncpResponse = (byte[])channels.ToArray(typeof(byte));
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_MAC_SCAN_PERIOD:
                    ncpResponse = mDecoder.ReadUint16();
                    break;

                case SpinelProperties.SPINEL_PROP_MAC_SCAN_BEACON:
                    ncpResponse = mDecoder.ReadFields("Cct(ESSC)t(iCUdd)");
                    break;

                case SpinelProperties.SPINEL_PROP_MAC_ENERGY_SCAN_RESULT:
                    ncpResponse = mDecoder.ReadFields("Cc");
                    break;



                case SpinelProperties.SPINEL_PROP_MSG_BUFFER_COUNTERS:

                    tempObj = mDecoder.ReadFields("SSSSSSSSSSSSSSSS");

                    if (tempObj != null)
                    {
                        ArrayList counters = (ArrayList)tempObj;
                        ncpResponse = (ushort[])counters.ToArray(typeof(ushort));
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_PHY_CHAN:
                    ncpResponse = mDecoder.ReadUint8();
                    break;

                case SpinelProperties.SPINEL_PROP_PHY_CHAN_SUPPORTED:
                    tempObj = mDecoder.ReadFields("A(C)");

                    if (tempObj != null)
                    {
                        ArrayList channels = (ArrayList)tempObj;
                        ncpResponse = (byte[])channels.ToArray(typeof(byte));
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_IPV6_ADDRESS_TABLE:

                    tempObj = mDecoder.ReadFields("A(t(6CLL))");
                    ArrayList ipAddresses = new ArrayList();

                    if (tempObj != null)
                    {
                        ArrayList addressArray = tempObj as ArrayList;

                        foreach (ArrayList addrInfo in addressArray)
                        {
                            object[] ipProps = addrInfo.ToArray();
                            IPv6Address ipaddr = ipProps[0] as IPv6Address;
                            ipAddresses.Add(ipaddr);
                        }
                    }

                    if (ipAddresses.Count > 0)
                    {
                        ncpResponse = ipAddresses.ToArray(typeof(IPv6Address));
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_MAC_15_4_PANID:
                    ncpResponse = mDecoder.ReadUint16();
                    break;

                case SpinelProperties.SPINEL_PROP_MCU_POWER_STATE:
                    ncpResponse = mDecoder.ReadUint8();
                    break;


                case SpinelProperties.SPINEL_PROP_STREAM_NET:
                    tempObj = mDecoder.ReadFields("dD");
                    if (tempObj != null)
                    {
                        ArrayList responseArray = tempObj as ArrayList;
                        ncpResponse = responseArray[0];
                    }
                    break;

                case SpinelProperties.SPINEL_PROP_IPV6_LL_ADDR:
                    IPv6Address ipaddrLL = mDecoder.ReadIp6Address();
                    ncpResponse = ipaddrLL;
                    break;

                case SpinelProperties.SPINEL_PROP_IPV6_ML_ADDR:
                    IPv6Address ipaddrML = mDecoder.ReadIp6Address();
                    ncpResponse = ipaddrML;
                    break;

                case SpinelProperties.SPINEL_PROP_MAC_15_4_LADDR:
                    EUI64 eui64 = mDecoder.ReadEui64();
                    ncpResponse = eui64;
                    break;

                case SpinelProperties.SPINEL_PROP_HWADDR:
                    EUI64 hwaddr = mDecoder.ReadEui64();
                    ncpResponse = hwaddr;
                    break;

                    //case SpinelProperties.SPINEL_PROP_IPV6_ML_PREFIX:
                    //    ncpResponse = mDecoder.ReadFields("6C");
                    //    break;
            }

            frameData = new FrameData(mDecoder.FramePropertyId, mDecoder.FrameHeader, mDecoder.GetFrameLoad(), ncpResponse);
        }
    }
}
