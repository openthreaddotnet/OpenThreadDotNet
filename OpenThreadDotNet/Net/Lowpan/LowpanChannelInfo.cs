
#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.OpenThread.Net.Lowpan
{
#else
namespace dotNETCore.OpenThread.Net.Lowpan
{
#endif
    public class LowpanChannelInfo
    {
        public byte Channel { get; set; }
        public sbyte Rssi { get; set; }
    }
}
