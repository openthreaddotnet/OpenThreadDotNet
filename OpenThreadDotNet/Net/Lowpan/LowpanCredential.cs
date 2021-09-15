#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
namespace nanoFramework.OpenThread.Net.Lowpan
{
#else
using dotNETCore.OpenThread.NCP;
namespace dotNETCore.OpenThread.Net.Lowpan
{
#endif
    public class LowpanCredential
    {      
        private byte[] masterKey;
        private WpanApi wpanApi;

        public byte[] MasterKey
        {
            get
            {
                return masterKey;
            }

            set
            {              
                if (value != masterKey)
                {
                    if (wpanApi.SetNetNetworkKey(value))
                    {
                        masterKey = value;
                    }
                }
            }
        }

        internal LowpanCredential(WpanApi wpanApi)
        {
            this.wpanApi = wpanApi;
            wpanApi.GetNetNetworkKey();                
        }
    }
}
