using System;
using System.Text;
#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
namespace nanoFramework.OpenThread.Net.Lowpan
{ 
#else
using dotNETCore.OpenThread.NCP;
namespace dotNETCore.OpenThread.Net.Lowpan
{
#endif
    public class LowpanIdentity
    {                      
        private WpanApi wpanApi;
        private string networkName;
        private ushort panid;
        private byte channel;
        private byte[] xpanid;

        public string NetworkName
        {
            get
            {
                return networkName;              
            }
            set
            {                
                if (value != networkName)
                {
                    if (wpanApi.SetNetNetworkName(value))
                    {
                        networkName = value;
                    }
                    else
                    {
                        throw new Exception("Exception setting property value.");
                    }
                }
            }
        }

        public ushort Panid
        {
            get
            {
                return panid;
            }
            set
            {                
                if (value != panid)
                {
                    if (wpanApi.SetMac_15_4_PanId(value))
                    {
                        panid = value;
                    }
                }
            }
        }

        public byte Channel
        {
            get
            {
                return channel;
            }
            set
            {                
                if (value != channel)
                {
                    if (wpanApi.SetPhyChan(value))
                    {
                        channel = value;
                    }
                }
            }
        }

        public byte[] Xpanid
        {
            get
            {
                return xpanid;
            }
            set
            {                
                if (value != xpanid)
                {
                    if (wpanApi.SetNetXpanId(value))
                    {
                        xpanid = value;
                    }
                }
            }
        }

        internal LowpanIdentity(WpanApi wpanApi)
        {
            this.wpanApi = wpanApi;
            this.networkName = wpanApi.GetNetNetworkName();
            this.panid = wpanApi.GetMac_15_4_PanId();
            this.channel = wpanApi.GetPhyChan();
            this.xpanid = wpanApi.GetNetXpanId();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Name:").Append(NetworkName);

            //if (mType.Length > 0)
            //{
            //    sb.Append(", Type:").Append(mType);
            //}

            //if (mXpanid.Length > 0)
            //{
            //    sb.Append(", XPANID:").Append(HexDump.toHexString(mXpanid));
            //}

            //if (mPanid != UNSPECIFIED_PANID)
            //{
            //    sb.Append(", PANID:").Append(string.Format("0x{0:X4}", mPanid));
            //}

            //if (mChannel != UNSPECIFIED_CHANNEL)
            //{
            //    sb.Append(", Channel:").Append(mChannel);
            //}

            return sb.ToString();
        }
    }
}
