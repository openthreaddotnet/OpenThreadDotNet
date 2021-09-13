﻿using System;
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
        private InterfaceType interfaceType;
        private string vendor;
        private Capabilities[] capabilities;
        private byte[] supportedChannels;
        private byte[] scanMask;
        private bool threadStackState;
        private bool networkInterfaceState;
        private NetworkRole netRole = NetworkRole.Detached;
        private PowerState powerstate = PowerState.Offline;

        private HardwareAddress extendedAddress;
        private HardwareAddress hardwareAddress;
        
        private IPAddress[] ipAddresses;
        private IPAddress[] ipMulticastAddresses;
        private IPAddress ipLinkLocal;
        private IPAddress ipMeshLocal;

        private uint partitionId = 0;

        private ArrayList scanMacResult = new ArrayList();
        private ArrayList scanEnergyResult = new ArrayList();

        private AutoResetEvent scanThread = new AutoResetEvent(false);

        public event LowpanLastStatusHandler OnLastStatusHandler;
        public event LowpanRoleChanged OnLowpanNetRoleChanged;           
        public event PacketReceivedEventHandler OnPacketReceived;
        public event LowpanIpChanged OnIpChanged;

        private IStream stream;

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

        public InterfaceType InterfaceType
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

        public byte[] ScanMask
        {
            get { return supportedChannels; }
            set
            {
                if (wpanApi.DoChannelsMask((byte[])value))
                {
                    scanMask = value;
                }
            }
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
       
        public PowerState PowerState
        {
            get { return powerstate; }
            set
            {
                if (value != powerstate)
                {
                    if (wpanApi.DoPowerState((byte)value))
                    {
                        powerstate = value;
                    }
                }
            }
        }

        public NetworkRole NetRole
        {
            get { return netRole; }
            set
            {
                if (value != NetRole)
                {
                    if (wpanApi.DoNetRole((byte)value))
                    {
                        netRole = value;
                    }
                }
            }
        }

        public LowpanIdentity LowpanIdentity { get; set; }

        public LowpanCredential LowpanCredential { get; set; }

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

        public LastStatus LastStatus => (LastStatus)wpanApi.DoLastStatus();

        public uint PartitionId
        {
            get { return partitionId; }
        }

        public bool Connected 
        {
            get { return netRole == NetworkRole.Detached ? false : true ; }
        }
    
        public bool Commissioned => throw new NotImplementedException();

        //public LoWPAN(string portName)
        //{
        //    stream = new SerialStream(portName);
        //}

        //public LoWPAN(IStream stream)
        //{
        //    this.stream = stream;
        //}
     
        public void Open(string portName)
        {
            wpanApi = new WpanApi();
            wpanApi.FrameDataReceived += new FrameReceivedEventHandler(FrameDataReceived);
            wpanApi.Open(portName);

            //     wpanApi.DoReset();

            Thread.Sleep(300);

            ReadInitialValues();
            NetworkInterface.SetupInterface(this);
        }

        private void ReadInitialValues()
        {
           
            LowpanIdentity = new LowpanIdentity(wpanApi.DoNetworkName(),wpanApi.DoPanId(),wpanApi.DoChannel(),wpanApi.DoXpanId());
            LowpanCredential = new LowpanCredential(wpanApi.DoMasterkey());

            ncpVersion = wpanApi.DoNCPVersion();
            protocolVersion = wpanApi.DoProtocolVersion();
            interfaceType = (InterfaceType)wpanApi.DoInterfaceType();
            vendor = wpanApi.DoVendor();
            capabilities = wpanApi.DoCaps();
            supportedChannels = wpanApi.DoChannels();
            scanMask = wpanApi.DoChannelsMask();

            networkInterfaceState = wpanApi.DoInterfaceConfig();
            threadStackState = wpanApi.DoThread();
            netRole = (NetworkRole)wpanApi.DoNetRole();
            extendedAddress = new HardwareAddress(wpanApi.DoExtendedAddress().bytes);
            hardwareAddress = new HardwareAddress(wpanApi.DoPhysicalAddress().bytes);
            ipAddresses = NetUtilities.SpinelIPtoSystemIP(wpanApi.DoIPAddresses());

            ipLinkLocal = new IPAddress(wpanApi.DoIPLinkLocal64().bytes);
            ipMeshLocal = new IPAddress(wpanApi.DoIPMeshLocal64().bytes);
        }

        public bool NetworkInterfaceUp()
        {
            if (wpanApi.DoInterfaceConfig(true))
            {
                networkInterfaceState = true;
            }

            return networkInterfaceState;
        }

        public bool NetworkInterfaceDown()
        {
            if (wpanApi.DoInterfaceConfig(false))
            {
                networkInterfaceState = false;
            }

            return networkInterfaceState;
        }

        public bool ThreadUp()
        {
            if (wpanApi.DoThread(true))
            {
                threadStackState = true;
            }

            return threadStackState;
        }

        public bool ThreadDown()
        {
            if (wpanApi.DoThread(false))
            {
                threadStackState = false;
            }

            return threadStackState;
        }
                              
        public LowpanChannelInfo[] ScanEnergy()
        {
            wpanApi.DoScan(2);
            scanThread.WaitOne(10000, false);

            if (scanEnergyResult.Count > 0)
            {
                LowpanChannelInfo[] scanEnergyArray = (LowpanChannelInfo[])scanEnergyResult.ToArray(typeof(LowpanChannelInfo));
                scanEnergyResult.Clear();
                return scanEnergyArray;
            }
            else
            {
                return null;
            }
        }
        
        public LowpanBeaconInfo[] ScanBeacon()
        {
            wpanApi.DoScan(1);
            scanThread.WaitOne(10000, false);


            if (scanMacResult.Count > 0)        
            {            
                LowpanBeaconInfo[] scanMacArray = (LowpanBeaconInfo[])scanMacResult.ToArray(typeof(LowpanBeaconInfo));            
                scanMacResult.Clear();
                return scanMacArray;
            }
            else
            {
                return null;
            }    
        }

        public void EnableLowPower()
        {
            throw new NotImplementedException();
        }

        public void Form(string networkName, byte channel, string masterkey = null, ushort panid = 0xFFFF)
        {           
            if (networkName == string.Empty) throw new ArgumentException("Networkname cannot be null or empty");

            if (channel < 11 || channel > 26) throw new ArgumentException("Channel number should be in between 11 and 26");

            var scanResult = ScanBeacon();

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

            if (!ThreadUp()) throw new InvalidOperationException("Thread start exception");

            this.NetRole = (NetworkRole)wpanApi.DoNetRole();
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
                wpanApi.DoProperty_NET_REQUIRE_JOIN_EXISTING(true);
            }
            else
            {
                wpanApi.DoProperty_NET_REQUIRE_JOIN_EXISTING(false);
            }

            if (!ThreadUp()) throw new InvalidOperationException("Thread stack start exception");
        }

        public bool ShutDownNetwork()
        {
            //if (threadStackState == true)
            //{
            //    if (!ThreadDown()) return false;
            //}

            bool statete=wpanApi.DoInterfaceConfig();

            if (statete == true)
            {
               bool opaaa= wpanApi.DoInterfaceConfig(false);
            }

            //if(networkInterfaceState== true)
            //{
            //    if (!NetworkInterfaceDown()) return false;                
            //}

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
            wpanApi.DoSendData(frame);
        }

        public void Send(byte[] frame)
        {
            wpanApi.DoSendData(frame, false);
        }      

        public LowpanBufferCounters GetBufferCounters()
        {
            LowpanBufferCounters lowpanCounters = new LowpanBufferCounters(wpanApi.DoCountersMessageBuffer());
            return lowpanCounters;
        }

        private void FrameDataReceived(FrameData frameData)
        {
            uint properyId = frameData.PropertyId;

            if (properyId == SpinelProperties.PROP_LAST_STATUS)
            {
                if (OnLastStatusHandler != null)
                {
                    LastStatus lastStatus = (LastStatus)(uint)frameData.Response;
                    OnLastStatusHandler(lastStatus);
                }
                
                return;
            }
            else if (properyId == SpinelProperties.SPINEL_PROP_STREAM_NET)
            {
                byte[] ipv6frame = (byte[])frameData.Response;

                if (OnPacketReceived != null)
                {
                    OnPacketReceived(this, ipv6frame);
                }

                return;
            }
            else if (properyId == SpinelProperties.SPINEL_PROP_MAC_SCAN_STATE)
            {
                byte scanState = (byte)(frameData.Response);

                if (scanState == 0)
                {
                    scanThread.Set();
                }
            }
            else if (properyId == SpinelProperties.SPINEL_PROP_MAC_SCAN_BEACON)
            {
                ArrayList scanInfo = (ArrayList)frameData.Response;

                LowpanBeaconInfo lowpanBeaconInfo = new LowpanBeaconInfo();

                lowpanBeaconInfo.Channel = (byte)scanInfo[0];
                lowpanBeaconInfo.Rssi = (sbyte)scanInfo[1];

                ArrayList tempObj = scanInfo[2] as ArrayList;
                EUI64 mac = (EUI64)tempObj[0];

                lowpanBeaconInfo.HardwareAddress = new HardwareAddress(mac.bytes);
                lowpanBeaconInfo.ShortAddress = (ushort)tempObj[1];
                lowpanBeaconInfo.PanId = (ushort)tempObj[2];
                lowpanBeaconInfo.LQI = (byte)tempObj[3];

                tempObj = scanInfo[3] as ArrayList;

                lowpanBeaconInfo.Protocol = (uint)tempObj[0];
                lowpanBeaconInfo.Flags = (byte)tempObj[1];
                lowpanBeaconInfo.NetworkName = (string)tempObj[2];
                lowpanBeaconInfo.XpanId = (byte[])tempObj[3];

                scanMacResult.Add(lowpanBeaconInfo);


                return;
            }
            else if (properyId == SpinelProperties.SPINEL_PROP_MAC_ENERGY_SCAN_RESULT)
            {
                ArrayList energyScan = (ArrayList)frameData.Response;

                LowpanChannelInfo lowpanChannelInfo = new LowpanChannelInfo();

                lowpanChannelInfo.Channel = (byte)energyScan[0];
                lowpanChannelInfo.Rssi = (sbyte)energyScan[1];
                scanEnergyResult.Add(lowpanChannelInfo);

                return;
            }

            if (frameData.TID == 0x80)
            {
                switch (properyId)
                {
                    case SpinelProperties.SPINEL_PROP_NET_ROLE :
                        NetworkRole newRole = (NetworkRole)(byte)(frameData.Response);
                        if (netRole != newRole)
                        {
                            netRole = newRole;
                            if (OnLowpanNetRoleChanged != null)
                            {
                                OnLowpanNetRoleChanged();
                            }
                        }
                        break;

                    case SpinelProperties.SPINEL_PROP_IPV6_LL_ADDR:

                        if (frameData.Response == null)
                        {
                            ipLinkLocal = null;
                            return;
                        }

                        IPv6Address ipaddrLL = (IPv6Address)frameData.Response;
                        ipLinkLocal = new IPAddress(ipaddrLL.bytes);

                        if (OnIpChanged != null)
                        {
                            OnIpChanged();
                        }

                        break;

                    case SpinelProperties.SPINEL_PROP_IPV6_ML_ADDR:

                        if (frameData.Response == null)
                        {
                            ipMeshLocal = null;
                            return;
                        }

                        IPv6Address ipaddrML = (IPv6Address)frameData.Response;
                        ipMeshLocal = new IPAddress(ipaddrML.bytes);
                        break;

                    case SpinelProperties.SPINEL_PROP_IPV6_ADDRESS_TABLE:
                        ipAddresses = NetUtilities.SpinelIPtoSystemIP((IPv6Address[])frameData.Response);
                        break;

                    case SpinelProperties.SPINEL_PROP_IPV6_MULTICAST_ADDRESS_TABLE:
                        ipMulticastAddresses = NetUtilities.SpinelIPtoSystemIP((IPv6Address[])frameData.Response);
                        break;

                    //case SpinelProperties.PROP_NET_SAVED:
                    //case SpinelProperties.PROP_NET_IF_UP:
                    //    break;
                    //case SpinelProperties.PROP_NET_STACK_UP:
                    //    break;

                    //case SpinelProperties.PROP_NET_NETWORK_NAME:
                    //case SpinelProperties.PROP_NET_XPANID:
                    //case SpinelProperties.PROP_NET_MASTER_KEY:
                    //case SpinelProperties.PROP_NET_KEY_SEQUENCE_COUNTER:
                    //case SpinelProperties.PROP_NET_PARTITION_ID:
                    //case SpinelProperties.PROP_NET_KEY_SWITCH_GUARDTIME:
                    //    break;

                    //case SpinelProperties.SPINEL_PROP_THREAD_LEADER_ADDR:
                    //case SpinelProperties.SPINEL_PROP_THREAD_PARENT:
                    //case SpinelProperties.SPINEL_PROP_THREAD_CHILD_TABLE:
                    //case SpinelProperties.SPINEL_PROP_THREAD_LEADER_RID:
                    //case SpinelProperties.SPINEL_PROP_THREAD_LEADER_WEIGHT:
                    //case SpinelProperties.SPINEL_PROP_THREAD_LOCAL_LEADER_WEIGHT:
                    //case SpinelProperties.SPINEL_PROP_THREAD_NETWORK_DATA:
                    //case SpinelProperties.SPINEL_PROP_THREAD_NETWORK_DATA_VERSION:
                    //case SpinelProperties.SPINEL_PROP_THREAD_STABLE_NETWORK_DATA:
                    //case SpinelProperties.SPINEL_PROP_THREAD_STABLE_NETWORK_DATA_VERSION:
                    //case SpinelProperties.SPINEL_PROP_THREAD_ASSISTING_PORTS:
                    //case SpinelProperties.SPINEL_PROP_THREAD_ALLOW_LOCAL_NET_DATA_CHANGE:
                    //case SpinelProperties.SPINEL_PROP_THREAD_MODE:
                    //    break;
                    //case SpinelProperties.SPINEL_PROP_THREAD_ON_MESH_NETS:
                    //    break;
                    //case SpinelProperties.SPINEL_PROP_THREAD_OFF_MESH_ROUTES:
                    //    break;



                    //case SpinelProperties.SPINEL_PROP_IPV6_ML_PREFIX:
                    //    break;

                    //case SpinelProperties.SPINEL_PROP_IPV6_ROUTE_TABLE:
                    //case SpinelProperties.SPINEL_PROP_IPV6_ICMP_PING_OFFLOAD:
                    //case SpinelProperties.SPINEL_PROP_IPV6_ICMP_PING_OFFLOAD_MODE:
                    //    break;



                    //case SpinelProperties.SPINEL_PROP_THREAD_CHILD_TIMEOUT:
                    //case SpinelProperties.SPINEL_PROP_THREAD_RLOC16:
                    //case SpinelProperties.SPINEL_PROP_THREAD_ROUTER_UPGRADE_THRESHOLD:
                    //case SpinelProperties.SPINEL_PROP_THREAD_CONTEXT_REUSE_DELAY:
                    //case SpinelProperties.SPINEL_PROP_THREAD_NETWORK_ID_TIMEOUT:
                    //case SpinelProperties.SPINEL_PROP_THREAD_ACTIVE_ROUTER_IDS:
                    //case SpinelProperties.SPINEL_PROP_THREAD_RLOC16_DEBUG_PASSTHRU:
                    //case SpinelProperties.SPINEL_PROP_THREAD_ROUTER_ROLE_ENABLED:
                    //case SpinelProperties.SPINEL_PROP_THREAD_ROUTER_DOWNGRADE_THRESHOLD:
                    //case SpinelProperties.SPINEL_PROP_THREAD_ROUTER_SELECTION_JITTER:
                    //case SpinelProperties.SPINEL_PROP_THREAD_PREFERRED_ROUTER_ID:
                    //case SpinelProperties.SPINEL_PROP_THREAD_CHILD_COUNT_MAX:
                    //    break;
                    //case SpinelProperties.SPINEL_PROP_THREAD_NEIGHBOR_TABLE:
                    //    break;
                    //case SpinelProperties.SPINEL_PROP_THREAD_LEADER_NETWORK_DATA:
                    //    break;

                    //case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_NEW_CHANNEL:
                    //case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_DELAY:
                    //case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_SUPPORTED_CHANNELS:
                    //case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_FAVORED_CHANNELS:
                    //case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_CHANNEL_SELECT:
                    //case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_AUTO_SELECT_ENABLED:
                    //case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_AUTO_SELECT_INTERVAL:
                    //case SpinelProperties.SPINEL_PROP_THREAD_NETWORK_TIME:
                    //case SpinelProperties.SPINEL_PROP_TIME_SYNC_PERIOD:
                    //case SpinelProperties.SPINEL_PROP_TIME_SYNC_XTAL_THRESHOLD:
                    //case SpinelProperties.SPINEL_PROP_CHILD_SUPERVISION_INTERVAL:
                    //case SpinelProperties.SPINEL_PROP_CHILD_SUPERVISION_CHECK_TIMEOUT:
                    //case SpinelProperties.SPINEL_PROP_RCP_VERSION:
                    //case SpinelProperties.SPINEL_PROP_SLAAC_ENABLED:
                    //    break;
                    //case SpinelProperties.SPINEL_PROP_PARENT_RESPONSE_INFO:
                    //    break;
                    //case SpinelProperties.PROP_LAST_STATUS:

                    //    LastStatus lastStatus = (LastStatus)Convert.ToInt32(frameData.Response);
                    //    OnLastStatusHandler(lastStatus);
                    //    break;
                    default:
                        break;

                }
            }
        }
    }
}
