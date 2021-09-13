using System;
using System.Text;

#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.OpenThread.Net.Lowpan
{
#else
namespace dotNETCore.OpenThread.Net.Lowpan
{
#endif
    public class LowpanBufferCounters
    {
        public ushort TotalBuffers { get; internal set; }
        public ushort FreeBuffers { get; internal set; }
        public ushort LowpanSendMessages { get; internal set; }
        public ushort LowpanSendBuffers { get; internal set; }
        public ushort LowpanReassemblyMessages { get; internal set; }
        public ushort LowpanReassemblyBuffers { get; internal set; }
        public ushort Ip6Messages { get; internal set; }
        public ushort Ip6Buffers { get; internal set; }
        public ushort MplMessages { get; internal set; }
        public ushort MplBuffers { get; internal set; }
        public ushort MleMessages { get; internal set; }
        public ushort MleBuffers { get; internal set; }
        public ushort ArpMessages { get; internal set; }
        public ushort ArpBuffers { get; internal set; }
        public ushort CoapMessages { get; internal set; }
        public ushort CoapBuffers { get; internal set; }

        public LowpanBufferCounters(ushort[] bufferCountersInfo)
        {
            TotalBuffers = bufferCountersInfo[0];
            FreeBuffers = bufferCountersInfo[1];
            LowpanSendMessages = bufferCountersInfo[2];
            LowpanSendBuffers = bufferCountersInfo[3];
            LowpanReassemblyMessages = bufferCountersInfo[4];
            LowpanReassemblyBuffers = bufferCountersInfo[5];
            Ip6Messages = bufferCountersInfo[6];
            Ip6Buffers = bufferCountersInfo[7];
            MplMessages = bufferCountersInfo[8];
            MplBuffers = bufferCountersInfo[9];
            MleMessages = bufferCountersInfo[10];
            MleBuffers = bufferCountersInfo[11];
            ArpMessages = bufferCountersInfo[12];
            ArpBuffers = bufferCountersInfo[13];
            CoapMessages = bufferCountersInfo[14];
            CoapBuffers = bufferCountersInfo[15];
        }
    }
}
