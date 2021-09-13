#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
namespace nanoFramework.OpenThread.Spinel
{ 
#else
using dotNETCore.OpenThread.NCP;
namespace dotNETCore.OpenThread.Spinel
{
#endif   
    public class FrameData
    {
        public FrameData() { }

        public FrameData(uint propertyId, byte tid, byte[] propertyValue,  object response)
        {
            this.PropertyId = propertyId;
            this.PropertyValue = propertyValue;
            this.TID = tid;
            this.Response = response;
            this.UID = Utilities.GetUID(propertyId, tid);
        }

        public uint PropertyId { get; }

        public byte[] PropertyValue { get; }

        public byte TID { get; }

        public object Response { get; }

        public int UID { get; }
    }
}
