using System;
using System.Text;

#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.OpenThread.Core
{ 
#else

namespace dotNETCore.OpenThread.Core
{
#endif
    public class EUI48
    {
        public byte[] bytes = new byte[6];
    }

    public class EUI64
    {
        public byte[] bytes = new byte[8];
    }

    public class IPv6Address
    {
        public byte[] bytes = new byte[16];
    }

    public class OpenThreadExtAddress
    {
        public byte[] m8 = new byte[8];
    }


    public class OpenThreadRouterInfo
    {
        public OpenThreadExtAddress OpenThreadExtAddress; ///< IEEE 802.15.4 Extended Address
        ushort Rloc16; ///< RLOC16
        byte RouterId; ///< Router ID
        byte NextHop; ///< Next hop to router
        byte PathCost;  ///< Path cost to router
        byte LinkQualityIn; ///< Link Quality In
        byte LinkQualityOut; ///< Link Quality Out
        byte Age;    ///< Time last heard
        bool Allocated; ///< Router ID allocated or not
        bool LinkEstablished;  ///< Link established with Router ID or not
    }

    public class OpenThreadNetifAddress
    {
        OpenThreadIp6Address Address;                ///< The IPv6 unicast address.
        byte PrefixLength;           ///< The Prefix length (in bits).
        byte AddressOrigin;          ///< The IPv6 address origin.
        bool Preferred;          ///< TRUE if the address is preferred, FALSE otherwise.
        bool Valid;              ///< TRUE if the address is valid, FALSE otherwise.
        bool ScopeOverrideValid; ///< TRUE if the mScopeOverride value is valid, FALSE otherwise.
        uint ScopeOverride = 4;      ///< The IPv6 scope of this address.
        bool Rloc;               ///< TRUE if the address is an RLOC, FALSE otherwise.
    }


    public class OpenThreadChildInfo
    {
        public OpenThreadExtAddress OpenThreadExtAddress;
        uint Timeout;
        uint Age;
        ushort Rloc16;
        ushort ChildId;
        byte NetworkDataVersion;
        byte LinkQualityIn;
        sbyte AverageRssi;
        sbyte LastRssi;

        uint FrameErrorRate;       ///< Frame error rate (0xffff->100%). Requires error tracking feature.
        uint MessageErrorRate;     ///< (IPv6) msg error rate (0xffff->100%). Requires error tracking feature.
        uint QueuedMessageCnt;     ///< Number of queued messages for the child.
        byte Version;              ///< MLE version

        bool RxOnWhenIdle;     ///< rx-on-when-idle
        bool FullThreadDevice; ///< Full Thread Device
        bool FullNetworkData;  ///< Full Network Data
        bool IsStateRestoring; ///< Is in restoring state
        bool IsCslSynced;      ///< Is child CSL synchronized
    }

    public class OpenThreadIp6Prefix
    {
        OpenThreadIp6Address Prefix; ///< The IPv6 prefix.
        byte Length; ///< The IPv6 prefix length (in bits).
    }

    //public class OpenThreadIp6MLPrefix
    //{
    //    OpenThreadIp6Address Prefix; ///< The IPv6 prefix.
    //    byte Length; ///< The IPv6 prefix length (in bits).
    //}

    public class OpenThreadIp6Address
    {
        byte[] m8 = new byte[16];  ///< 8-bit fields
        ushort[] m16 = new ushort[8]; ///< 16-bit fields
        uint[] m32 = new uint[4]; ///< 32-bit fields
    }

    public class OpenThreadIp6InterfaceIdentifier
    {
        byte[] m8 = new byte[8];  ///< 8-bit fields
        ushort[] m16 = new ushort[4]; ///< 16-bit fields
        uint[] m32 = new uint[2]; ///< 32-bit fields
    }

    public class OpenThreadIp6NetworkPrefix
    {
        byte[] m8 = new byte[8]; ///< The Network Prefix.
    }

    public class OpenThreadIp6AddressComponents
    {
        OpenThreadIp6NetworkPrefix NetworkPrefix; ///< The Network Prefix (most significant 64 bits of the address)
        OpenThreadIp6InterfaceIdentifier Iid;           ///< The Interface Identifier (least significant 64 bits of the address)
    }

    public class OpenThreadBorderRouterConfig
    {
        OpenThreadIp6Prefix Prefix;           ///< The IPv6 prefix.
        short Preference = 2;   ///< A 2-bit signed int preference (`OT_ROUTE_PREFERENCE_*` values).
        bool Preferred;    ///< Whether prefix is preferred.
        bool Slaac;        ///< Whether prefix can be used for address auto-configuration (SLAAC).
        bool Dhcp;         ///< Whether border router is DHCPv6 Agent.
        bool Configure;    ///< Whether DHCPv6 Agent supplying other config data.
        bool DefaultRoute; ///< Whether border router is a default router for prefix.
        bool OnMesh;       ///< Whether this prefix is considered on-mesh.
        bool Stable;       ///< Whether this configuration is considered Stable Network Data.
        bool NdDns;        ///< Whether this border router can supply DNS information via ND.
        bool Dp;           ///< Whether prefix is a Thread Domain Prefix (added since Thread 1.2).
        uint mRloc16;           ///< The border router's RLOC16 (value ignored on config add).
    }

    public class OpenThreadExternalRouteConfig
    {
        OpenThreadIp6Prefix Prefix;           ///< The IPv6 prefix.
        ushort Rloc16;
        short Preference = 2;   ///< A 2-bit signed int preference (`OT_ROUTE_PREFERENCE_*` values).       
        bool mNat64 = true;               ///< Whether this is a NAT64 prefix.
        bool mStable = true;              ///< Whether this configuration is considered Stable Network Data.
        bool mNextHopIsThisDevice = true; ///< Whether the next hop is this device (value ignored on config add).
    }

    public class OpenThreadNeighborInfo
    {
        OpenThreadExtAddress ExtAddress;           ///< IEEE 802.15.4 Extended Address
        uint Age;                  ///< Time last heard
        ushort Rloc16;               ///< RLOC16
        uint LinkFrameCounter;     ///< Link Frame Counter
        uint MleFrameCounter;      ///< MLE Frame Counter
        byte LinkQualityIn;        ///< Link Quality In
        byte AverageRssi;          ///< Average RSSI
        byte LastRssi;             ///< Last observed RSSI
        ushort FrameErrorRate;       ///< Frame error rate (0xffff->100%). Requires error tracking feature.
        ushort MessageErrorRate;     ///< (IPv6) msg error rate (0xffff->100%). Requires error tracking feature.
        bool RxOnWhenIdle = true;     ///< rx-on-when-idle
        bool FullThreadDevice = true; ///< Full Thread Device
        bool FullNetworkData = true;  ///< Full Network Data
        bool IsChild = true;          ///< Is the neighbor a child
    }
}
