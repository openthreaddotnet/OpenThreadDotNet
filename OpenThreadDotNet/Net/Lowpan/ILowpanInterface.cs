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
        InterfaceType InterfaceType { get; }
        IPAddress[] IPAddresses { get; }
        IPAddress IPLinkLocal { get; }
        IPAddress IPMeshLocal { get; }
        IPAddress[] IPMulticastAddresses { get; }
        LastStatus LastStatus { get; }
        LowpanCredential LowpanCredential { get; set; }
        LowpanIdentity LowpanIdentity { get; set; }
        string Name { get; }
        string NcpVersion { get; }
        bool NetworkInterfaceState { get; }
        uint PartitionId { get; }
        HardwareAddress HardwareAddress { get; }
        string ProtocolVersion { get; }
        NetworkRole NetRole { get; set; }
        byte[] ScanMask { get; set; }
        PowerState PowerState { get; }
        byte[] SupportedChannels { get; }
        bool ThreadStackState { get; }
        string Vendor { get; }
        bool Connected { get; }
        bool Commissioned { get; }

        event LowpanLastStatusHandler OnLastStatusHandler;       
        event LowpanRoleChanged OnLowpanNetRoleChanged;
        event PacketReceivedEventHandler OnPacketReceived;
        event LowpanIpChanged OnIpChanged;
        
        void Form(string networkName, byte channel, string masterkey, ushort panid);
        void Attach(string networkName, byte channel, string masterkey, string xpanid, ushort panid, bool requireExistingPeers = false);
        void Join(string networkName, byte channel, string masterkey, string xpanid, ushort panid);
        bool ShutDownNetwork();

        void EnableLowPower();

        bool NetworkInterfaceDown();
        bool NetworkInterfaceUp();
        void OnHostWake();
        void Open();        
        void Reset();
        LowpanBeaconInfo[] ScanBeacon();
        LowpanChannelInfo[] ScanEnergy();
        bool ThreadDown();
        bool ThreadUp();
        void SendAndWait(byte[] frame);
        void Send(byte[] frame);
    }
}