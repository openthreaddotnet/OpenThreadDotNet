using System.Text;
#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.OpenThread.Net.Lowpan
{ 
#else
namespace dotNETCore.OpenThread.Net.Lowpan
{
#endif
    public class LowpanIdentity
    {                      
      //  private WpanApi wpanApi;
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
                networkName = value;
                //if (value != networkName)
                //{
                //    if(wpanApi.DoNetworkName(value))
                //    {
                //        networkName = value;
                //    }
                //    else
                //    {
                //        throw new Exception("Exception setting property value.");
                //    }
                //}                          
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
                panid = value;
                //if (value != panid)
                //{
                //    if (wpanApi.DoPanId(value))
                //    {
                //        panid = value;
                //    }
                //}              
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
                channel = value;
                //if (value != channel)
                //{
                //    if (wpanApi.DoChannel(value))
                //    {
                //        channel = value;
                //    }
                //}
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
                xpanid = value;
                //if (value != xpanid)
                //{
                //    if (wpanApi.DoXpanId(value))
                //    {
                //        xpanid = value;
                //    }
                //}              
            }
        }

        public LowpanIdentity(string networkName, ushort panid, byte channel,byte[] xpanid)
        {
          //  this.wpanApi = wpanApi;
            this.networkName = networkName;
            this.panid = panid;
            this.channel = channel;
            this.xpanid = xpanid;
        }

        //public LowpanIdentity(WpanApi wpanApi)
        //{
        //    this.wpanApi = wpanApi;
        //    this.networkName=wpanApi.DoNetworkName();
        //    this.panid= wpanApi.DoPanId();
        //    this.channel= wpanApi.DoChannel();
        //    this.xpanid= wpanApi.DoXpanId();
        //}

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
