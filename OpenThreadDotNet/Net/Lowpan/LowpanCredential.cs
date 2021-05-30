using nanoFramework.OpenThread.NCP;

namespace nanoFramework.OpenThread.Net.Lowpan
{
    public class LowpanCredential
    {      
        private byte[] masterKey;

        public byte[] MasterKey
        {
            get
            {
                return masterKey;
            }

            set
            {
                masterKey = value;
                //if (value != masterKey)
                //{
                //    if (wpanApi.DoMasterkey(value))
                //    {
                //        masterKey = value;
                //    }
                //}              
            }
        }

        public LowpanCredential(byte[] masterKey)
        {
           // this.wpanApi = wpanApi;
            this.masterKey = masterKey;
        }
    }
}
