#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.Spinel;
namespace nanoFramework.OpenThread.Net.Lowpan
{
#else
using dotNETCore.OpenThread.Spinel;
namespace dotNETCore.OpenThread.Net.Lowpan
{
#endif
    public interface ILowpanInterface
    {
        Capabilities[] Capabilities { get; }
        HardwareAddress ExtendedAddress { get; }
        SpinelProtocolType InterfaceType { get; }
        IPAddress[] IPAddresses { get; }
        IPAddress IPLinkLocal { get; }
        IPAddress IPMeshLocal { get; }
        IPAddress[] IPMulticastAddresses { get; }
        SpinelStatus LastStatus { get; }
        LowpanCredential LowpanCredential { get; set; }
        LowpanIdentity LowpanIdentity { get; set; }
        LowpanScanner LowpanScanner { get; set; }
        string Name { get; }
        string NcpVersion { get; }
        bool NetworkInterfaceState { get; }
        uint PartitionId { get; }
        HardwareAddress HardwareAddress { get; }
        string ProtocolVersion { get; }
        SpinelNetRole NetRole { get; set; }        
        SpinelMcuPowerState PowerState { get; }
        byte[] SupportedChannels { get; }
        bool ThreadStackState { get; }
        string Vendor { get; }
        bool Connected { get; }
        bool Commissioned { get; }
                
        event PacketReceivedEventHandler OnPacketReceived;
                
        void Form(string networkName, byte channel, string masterkey, ushort panid);
        void Attach(string networkName, byte channel, string masterkey, string xpanid, ushort panid, bool requireExistingPeers = false);
        void Join(string networkName, byte channel, string masterkey, string xpanid, ushort panid);
        bool ShutDownNetwork();

        void EnableLowPower();

        bool NetworkInterfaceDown();
        bool NetworkInterfaceUp();
        void OnHostWake();
        void Open(string portName);        
        void Reset();
        bool ThreadStackDown();
        bool ThreadStackUp();
        void SendAndWait(byte[] frame);
        void Send(byte[] frame);
    }
}