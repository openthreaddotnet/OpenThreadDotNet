using System;
using System.Collections;
using System.Threading;

#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.Net;
using nanoFramework.OpenThread.Net.Lowpan;
using nanoFramework.OpenThread.Spinel;
using nanoFramework.OpenThread.Core;

namespace nanoFramework.OpenThread.NCP
{
#else
using dotNETCore.OpenThread.Net;
using dotNETCore.OpenThread.Net.Lowpan;
using dotNETCore.OpenThread.Spinel;
using dotNETCore.OpenThread.Core;

namespace dotNETCore.OpenThread.NCP
{
#endif

    public class LoWPAN : ILowpanInterface
    {
        private WpanApi wpanApi;

        private string name;
        private string ncpVersion;
        private uint[] protocolVersion;
        private SpinelProtocolType interfaceType;
        private string vendor;
        private Capabilities[] capabilities;
        private byte[] supportedChannels;        
        private bool threadStackState;
        private bool networkInterfaceState;
        private SpinelNetRole netRole = SpinelNetRole.SPINEL_NET_ROLE_DETACHED;
        private SpinelMcuPowerState powerstate = SpinelMcuPowerState.SPINEL_MCU_POWER_STATE_OFF;

        private HardwareAddress extendedAddress;
        private HardwareAddress hardwareAddress;
        
        private IPAddress[] ipAddresses;
        private IPAddress[] ipMulticastAddresses;
        private IPAddress ipLinkLocal;
        private IPAddress ipMeshLocal;
        private SpinelStatus lastStatus;

        private uint partitionId = 0;    
        private AutoResetEvent scanThread = new AutoResetEvent(false);
                     
        public event PacketReceivedEventHandler OnPacketReceived;
        public event LowpanPropertyChangedHandler OnLowpanPropertyChanged;

        public string Name
        {
            get { return ncpVersion.Split('/')[0]; }
        }

        public string NcpVersion
        {
            get { return ncpVersion; }
        }

        public string ProtocolVersion
        {
            get 
            {
                if (protocolVersion == null)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Format("Version {0}.{1}", protocolVersion[0], protocolVersion[1]);
//#if NETMF
//                    return "Version " + protocolVersion[0].ToString() +"." + protocolVersion[1].ToString();
//#else
//                    return string.Format("Version {0}.{1}", protocolVersion[0], protocolVersion[1]);
//#endif

                }
            }
        }

        public SpinelProtocolType InterfaceType
        {
            get { return interfaceType; }
        }

        public string Vendor
        {
            get { return vendor; }
        }

        public Capabilities[] Capabilities
        {
            get { return capabilities; }
        }

        public byte[] SupportedChannels
        {
            get { return supportedChannels; }
        }

 
        public HardwareAddress HardwareAddress
        {
            get { return hardwareAddress; }
        }

        public HardwareAddress ExtendedAddress
        {
            get { return extendedAddress; }
        }

        public bool ThreadStackState
        {
            get { return threadStackState; }
        }

        public bool NetworkInterfaceState
        {
            get { return networkInterfaceState; }
        }
       
        public SpinelMcuPowerState PowerState
        {
            get { return powerstate; }
            set
            {
                if (value != powerstate)
                {
                    if (wpanApi.SetPropMcuPowerState((SpinelMcuPowerState)value))
                    {                        
                        powerstate = value;
                    }
                }
            }
        }

        public SpinelNetRole NetRole
        {
            get { return netRole; }
            set
            {
                if (value != NetRole)
                {
                    if (wpanApi.SetNetRole((SpinelNetRole)value))
                    {
                        netRole = value;
                    }
                }
            }
        }

        public LowpanIdentity LowpanIdentity { get; set; }

        public LowpanCredential LowpanCredential { get; set; }

        public LowpanScanner LowpanScanner { get; set; }

        public IPAddress[] IPAddresses
        {
            get { return ipAddresses; }
        }

        public IPAddress[] IPMulticastAddresses
        {
            get { return ipMulticastAddresses; }
        }

        public IPAddress IPLinkLocal
        {
            get { return ipLinkLocal; }
        }

        public IPAddress IPMeshLocal
        {
            get { return ipMeshLocal; }
        }

        public SpinelStatus LastStatus
        {
            get { return lastStatus; }
        }

        public uint PartitionId
        {
            get { return partitionId; }
        }

        public bool Connected 
        {
            get { return netRole == SpinelNetRole.SPINEL_NET_ROLE_DETACHED ? false : true ; }
        }
    
        public bool Commissioned => throw new NotImplementedException();

        public LoWPAN()
        {          
        }

        public void Open(string portName)
        {
            wpanApi = new WpanApi();
            wpanApi.OnFrameDataReceived += new FrameReceivedEventHandler(FrameDataReceived);           
            wpanApi.OnPropertyChanged += new SpinelPropertyChangedHandler(PropertyChanged);
            wpanApi.Open(portName);
            
           // wpanApi.DoReset();

            Thread.Sleep(300);
            ReadInitialValues();

            ShutDownNetwork();
            NetworkInterfaceDown();

            NetworkInterface.SetupInterface(this);
        }

        private void WpanApi_OnPropertyChanged(uint PropertyId, object PropertyValue)
        {
            throw new NotImplementedException();
        }

        private void ReadInitialValues()
        {         
            LowpanIdentity = new LowpanIdentity(wpanApi);
            LowpanCredential = new LowpanCredential(wpanApi);
            LowpanScanner = new LowpanScanner(wpanApi);

            ncpVersion = wpanApi.GetPropNcpVersion();
            protocolVersion = wpanApi.GetPropProtocolVersion();
            interfaceType = (SpinelProtocolType)wpanApi.GetPropInterfaceType();
            vendor = wpanApi.GetPropVendorId().ToString();
            capabilities = wpanApi.GetPropCaps();
            supportedChannels = wpanApi.GetPhyChanSupported();            
            networkInterfaceState = wpanApi.GetNetIfUp();
            threadStackState = wpanApi.GetNetStackUp();
            netRole = (SpinelNetRole)wpanApi.GetNetRole();
            extendedAddress = new HardwareAddress(wpanApi.GetMac_15_4_Laddr().bytes);
            hardwareAddress = new HardwareAddress(wpanApi.GetPropHwaddr().bytes);
            ipAddresses = NetUtilities.SpinelIPtoSystemIP(wpanApi.GetIPv6AddressTable());

            ipLinkLocal = new IPAddress(wpanApi.GetIPv6LLAddr().bytes);
            ipMeshLocal = new IPAddress(wpanApi.GetIPv6MLAddr().bytes);
            lastStatus = (SpinelStatus)wpanApi.GetPropLastStatus();
        }

        public bool NetworkInterfaceUp()
        {
            if (wpanApi.SetNetIfUp(true))
            {
                networkInterfaceState = true;
            }

            return networkInterfaceState;
        }

        public bool NetworkInterfaceDown()
        {
            if (wpanApi.SetNetIfUp(false))
            {
                networkInterfaceState = false;
            }

            return networkInterfaceState;
        }

        public bool ThreadStackUp()
        {
            if (wpanApi.SetNetStackUp(true))
            {
                threadStackState = true;
            }

            return threadStackState;
        }

        public bool ThreadStackDown()
        {
            if (wpanApi.SetNetStackUp(false))
            {
                threadStackState = false;
            }

            return threadStackState;
        }
                                  
        public void EnableLowPower()
        {
            throw new NotImplementedException();
        }

        public void Form(string networkName, byte channel, string masterkey = null, ushort panid = 0xFFFF)
        {           
            if (networkName == string.Empty) throw new ArgumentException("Networkname cannot be null or empty");

            if (channel < 11 || channel > 26) throw new ArgumentException("Channel number should be in between 11 and 26");



            var scanResult = LowpanScanner.ScanBeacon();

            bool netExisted = false;

            if (scanResult != null)
            {
                foreach (var beacon in scanResult)
                {
                    if (beacon.NetworkName == networkName && beacon.Channel == channel)
                    {
                        netExisted = true;
                        break;
                    }
                }
            }
          
            if (netExisted) throw new ArgumentException("Networkname with provided identity already exists.");

            if(!ShutDownNetwork()) throw new ArgumentException("Shut Down network exception");

            LowpanIdentity.Channel = channel;
            LowpanIdentity.NetworkName = networkName;
          
            if (!NetworkInterfaceUp()) throw new InvalidOperationException("Interface up exception");

            if (panid != 0xFFFF)
            {
                this.LowpanIdentity.Panid = panid;
            }

            if (masterkey != string.Empty)
            {
                this.LowpanCredential.MasterKey = Utilities.HexToBytes(masterkey);
            }

            if (!ThreadStackUp()) throw new InvalidOperationException("Thread start exception");
        }
                 
        public void Join(string networkName, byte channel, string masterkey, string xpanid, ushort panid)
        {
            Attach(networkName, channel, masterkey, xpanid, panid, true);
        }

        public void Attach(string networkName, byte channel, string masterkey, string xpanid, ushort panid, bool requireExistingPeers=false)
        {
            if (networkName == string.Empty) throw new ArgumentException("Networkname cannot be null or empty");

            if (channel < 11 || channel > 26) throw new ArgumentException("Channel number should be in between 11 and 26");

            if (masterkey == string.Empty) throw new ArgumentException("Masterkey cannot be null or empty");

            if (xpanid == string.Empty) throw new ArgumentException("Xpanid cannot be null or empty");

            if (panid == 0xFFFF) throw new ArgumentException("Panid value cannot be 0xFFFF");

            ShutDownNetwork();

            this.LowpanCredential.MasterKey = Utilities.HexToBytes(masterkey);

            this.LowpanIdentity.Channel = channel;
            this.LowpanIdentity.NetworkName = networkName;
            this.LowpanIdentity.Panid = panid;
            this.LowpanIdentity.Xpanid = Utilities.HexToBytes(xpanid);

            if (!NetworkInterfaceUp()) throw new InvalidOperationException("Interface up exception");

            if (requireExistingPeers)
            {
                wpanApi.SetNetRequireJoinExisting(true);
            }
            else
            {
                wpanApi.SetNetRequireJoinExisting(false);
            }

            if (!ThreadStackUp()) throw new InvalidOperationException("Thread stack start exception");
        }

        public bool ShutDownNetwork()
        {        
            wpanApi.SetNetIfUp(false);

            return true;
        }

        public void OnHostWake()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            wpanApi.DoReset();
        }
 
        public void SendAndWait(byte[] frame)
        {
            wpanApi.SetPropStreamNet(frame);
        }

        public void Send(byte[] frame)
        {
            wpanApi.SetPropStreamNet(frame, false);
        }      

        public LowpanBufferCounters GetBufferCounters()
        {
            LowpanBufferCounters lowpanCounters = new LowpanBufferCounters(wpanApi.GetMsgBufferCounters());
            return lowpanCounters;
        }

        private void PropertyChanged(uint PropertyId, object PropertyValue)
        {            
            switch ((SpinelProperties)PropertyId)
            {
                case SpinelProperties.SPINEL_PROP_LAST_STATUS:
                
                    lastStatus = (SpinelStatus)(uint)PropertyValue;

                    if (OnLowpanPropertyChanged != null)
                    {
                        OnLowpanPropertyChanged((SpinelProperties)PropertyId);
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_STREAM_NET:
                case SpinelProperties.SPINEL_PROP_STREAM_NET_INSECURE:


                    byte[] ipv6frame = (byte[])PropertyValue;

                    if (OnPacketReceived != null)
                    {
                        OnPacketReceived(ipv6frame);
                    }

                    break;
              
                case SpinelProperties.SPINEL_PROP_NET_ROLE:
                    
                    SpinelNetRole newRole = (SpinelNetRole)(byte)(PropertyValue);

                    if (netRole != newRole)
                    {
                        netRole = newRole;

                        if (OnLowpanPropertyChanged != null)
                        {
                            OnLowpanPropertyChanged((SpinelProperties)PropertyId);
                        }
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_IPV6_LL_ADDR:

                    if (PropertyValue == null)
                    {
                        ipLinkLocal = null;
                        return;
                    }

                    IPv6Address ipaddrLL = (IPv6Address)PropertyValue;
                    ipLinkLocal = new IPAddress(ipaddrLL.bytes);

                    if (OnLowpanPropertyChanged != null)
                    {
                        OnLowpanPropertyChanged((SpinelProperties)PropertyId);
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_IPV6_ML_ADDR:

                    if (PropertyValue == null)
                    {
                        ipMeshLocal = null;
                        return;
                    }

                    IPv6Address ipaddrML = (IPv6Address)PropertyValue;
                    ipMeshLocal = new IPAddress(ipaddrML.bytes);


                    if (OnLowpanPropertyChanged != null)
                    {
                        OnLowpanPropertyChanged((SpinelProperties)PropertyId);
                    }

                    break;


                case SpinelProperties.SPINEL_PROP_IPV6_ADDRESS_TABLE:
                    
                    ipAddresses = NetUtilities.SpinelIPtoSystemIP((IPv6Address[])PropertyValue);

                    if (OnLowpanPropertyChanged != null)
                    {
                        OnLowpanPropertyChanged((SpinelProperties)PropertyId);
                    }

                    break;

                case SpinelProperties.SPINEL_PROP_IPV6_MULTICAST_ADDRESS_TABLE:
                   
                    ipMulticastAddresses = NetUtilities.SpinelIPtoSystemIP((IPv6Address[])PropertyValue);

                    if (OnLowpanPropertyChanged != null)
                    {
                        OnLowpanPropertyChanged((SpinelProperties)PropertyId);
                    }

                    break;
            }
        }
            
        private void FrameDataReceived(FrameData frameData)
        {
          
            
        }
    }
}
