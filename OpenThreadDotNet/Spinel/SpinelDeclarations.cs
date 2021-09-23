//*
//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//*
//*Primitive Types
//*
//*+----------+----------------------+---------------------------------+
//*   | Char | Name | Description |
//*+----------+----------------------+---------------------------------+
//*   | "." | DATATYPE_VOID | Empty data type.Used           |
// *   |          |                      | internally.                     |
// *   |   "b"    | DATATYPE_BOOL        | Boolean value. Encoded in       |
// *   |          |                      | 8-bits as either 0x00 or 0x01.  |
// *   |          |                      | All other values are illegal.   |
// *   |   "C"    | DATATYPE_UINT8       | Unsigned 8-bit integer.         |
// *   |   "c"    | DATATYPE_INT8        | Signed 8-bit integer.           |
// *   |   "S"    | DATATYPE_UINT16      | Unsigned 16-bit integer.        |
// *   |   "s"    | DATATYPE_INT16       | Signed 16-bit integer.          |
// *   |   "L"    | DATATYPE_UINT32      | Unsigned 32-bit integer.        |
// *   |   "l"    | DATATYPE_INT32       | Signed 32-bit integer.          |
// *   |   "i"    | DATATYPE_UINT_PACKED | Packed Unsigned Integer.See    |
// *   |          |                      | description below               |
// *   |   "6"    | DATATYPE_IPv6ADDR    | IPv6 Address. (Big-endian)      |
// *   | "E"      | DATATYPE_EUI64       | EUI - 64 Address. (Big - endian) |
//*    | "e"      | DATATYPE_EUI48       | EUI - 48 Address. (Big - endian) |
//*    | "D"      | DATATYPE_DATA        | Arbitrary data.See related     |
// *   |          |                      | section below for details.      |
// *   |   "d"    | DATATYPE_DATA_WLEN   | Arbitrary data with prepended   |
// *   |          |                      | length. See below for details   |
// *   |   "U"    | DATATYPE_UTF8        | Zero-terminated UTF8-encoded    |
// *   |          |                      | string.                         |
// *   | "t(...)" | DATATYPE_STRUCT      | Structured datatype with        |
// *   |          |                      | prepended length.               |
// *   | "A(...)" | DATATYPE_ARRAY       | Array of datatypes. Compound    |
// *   |          |                      | type.                           |
// *   +----------+----------------------+---------------------------------+

#if (NANOFRAMEWORK_1_0)
using System.Collections;

namespace nanoFramework.OpenThread.Spinel
{ 
#else
using System.Collections;

namespace dotNETCore.OpenThread.Spinel
{
#endif      
    public class SpinelCommands
    {
        // Singular class that contains all Spinel constants. """

        public const byte HEADER_ASYNC = 0x80;

        public const byte HEADER_DEFAULT = 0x81;

        public const byte HEADER_EVENT_HANDLER = 0x82;

        ////=========================================
        //// Spinel Commands: Host -> NCP
        ////=========================================

        public const uint CMD_NOOP = 0;

        public const int CMD_RESET = 1;

        public const int CMD_PROP_VALUE_GET = 2;

        public const int CMD_PROP_VALUE_SET = 3;

        public const int CMD_PROP_VALUE_INSERT = 4;

        public const int CMD_PROP_VALUE_REMOVE = 5;

        ////=========================================
        //// Spinel Command Responses: NCP -> Host
        ////=========================================

        public const uint RSP_PROP_VALUE_IS = 6;

        public const uint RSP_PROP_VALUE_INSERTED = 7;

        public const uint RSP_PROP_VALUE_REMOVED = 8;
    }

    public enum Capabilities
    {
        SPINEL_CAP_LOCK = 1,
        SPINEL_CAP_NET_SAVE = 2,
        SPINEL_CAP_HBO = 3,
        SPINEL_CAP_POWER_SAVE = 4,

        SPINEL_CAP_COUNTERS = 5,
        SPINEL_CAP_JAM_DETECT = 6,

        SPINEL_CAP_PEEK_POKE = 7,

        SPINEL_CAP_WRITABLE_RAW_STREAM = 8,
        SPINEL_CAP_GPIO = 9,
        SPINEL_CAP_TRNG = 10,
        SPINEL_CAP_CMD_MULTI = 11,
        SPINEL_CAP_UNSOL_UPDATE_FILTER = 12,
        SPINEL_CAP_MCU_POWER_STATE = 13,
        SPINEL_CAP_PCAP = 14,

        SPINEL_CAP_802_15_4__BEGIN = 16,
        SPINEL_CAP_802_15_4_2003 = (SPINEL_CAP_802_15_4__BEGIN + 0),
        SPINEL_CAP_802_15_4_2006 = (SPINEL_CAP_802_15_4__BEGIN + 1),
        SPINEL_CAP_802_15_4_2011 = (SPINEL_CAP_802_15_4__BEGIN + 2),
        SPINEL_CAP_802_15_4_PIB = (SPINEL_CAP_802_15_4__BEGIN + 5),
        SPINEL_CAP_802_15_4_2450MHZ_OQPSK = (SPINEL_CAP_802_15_4__BEGIN + 8),
        SPINEL_CAP_802_15_4_915MHZ_OQPSK = (SPINEL_CAP_802_15_4__BEGIN + 9),
        SPINEL_CAP_802_15_4_868MHZ_OQPSK = (SPINEL_CAP_802_15_4__BEGIN + 10),
        SPINEL_CAP_802_15_4_915MHZ_BPSK = (SPINEL_CAP_802_15_4__BEGIN + 11),
        SPINEL_CAP_802_15_4_868MHZ_BPSK = (SPINEL_CAP_802_15_4__BEGIN + 12),
        SPINEL_CAP_802_15_4_915MHZ_ASK = (SPINEL_CAP_802_15_4__BEGIN + 13),
        SPINEL_CAP_802_15_4_868MHZ_ASK = (SPINEL_CAP_802_15_4__BEGIN + 14),
        SPINEL_CAP_802_15_4__END = 32,

        SPINEL_CAP_CONFIG__BEGIN = 32,
        SPINEL_CAP_CONFIG_FTD = (SPINEL_CAP_CONFIG__BEGIN + 0),
        SPINEL_CAP_CONFIG_MTD = (SPINEL_CAP_CONFIG__BEGIN + 1),
        SPINEL_CAP_CONFIG_RADIO = (SPINEL_CAP_CONFIG__BEGIN + 2),
        SPINEL_CAP_CONFIG__END = 40,

        SPINEL_CAP_ROLE__BEGIN = 48,
        SPINEL_CAP_ROLE_ROUTER = (SPINEL_CAP_ROLE__BEGIN + 0),
        SPINEL_CAP_ROLE_SLEEPY = (SPINEL_CAP_ROLE__BEGIN + 1),
        SPINEL_CAP_ROLE__END = 52,

        SPINEL_CAP_NET__BEGIN = 52,
        SPINEL_CAP_NET_THREAD_1_0 = (SPINEL_CAP_NET__BEGIN + 0),
        SPINEL_CAP_NET_THREAD_1_1 = (SPINEL_CAP_NET__BEGIN + 1),
        SPINEL_CAP_NET__END = 64,

        SPINEL_CAP_OPENTHREAD__BEGIN = 512,
        SPINEL_CAP_MAC_WHITELIST = (SPINEL_CAP_OPENTHREAD__BEGIN + 0),
        SPINEL_CAP_MAC_RAW = (SPINEL_CAP_OPENTHREAD__BEGIN + 1),
        SPINEL_CAP_OOB_STEERING_DATA = (SPINEL_CAP_OPENTHREAD__BEGIN + 2),
        SPINEL_CAP_CHANNEL_MONITOR = (SPINEL_CAP_OPENTHREAD__BEGIN + 3),
        SPINEL_CAP_ERROR_RATE_TRACKING = (SPINEL_CAP_OPENTHREAD__BEGIN + 4),
        SPINEL_CAP_CHANNEL_MANAGER = (SPINEL_CAP_OPENTHREAD__BEGIN + 5),
        SPINEL_CAP_OPENTHREAD_LOG_METADATA = (SPINEL_CAP_OPENTHREAD__BEGIN + 6),
        SPINEL_CAP_TIME_SYNC = (SPINEL_CAP_OPENTHREAD__BEGIN + 7),
        SPINEL_CAP_CHILD_SUPERVISION = (SPINEL_CAP_OPENTHREAD__BEGIN + 8),
        SPINEL_CAP_POSIX_APP = (SPINEL_CAP_OPENTHREAD__BEGIN + 9),
        SPINEL_CAP_SLAAC = (SPINEL_CAP_OPENTHREAD__BEGIN + 10),
        SPINEL_CAP_OPENTHREAD__END = 640,

        SPINEL_CAP_THREAD__BEGIN = 1024,
        SPINEL_CAP_THREAD_COMMISSIONER = (SPINEL_CAP_THREAD__BEGIN + 0),
        SPINEL_CAP_THREAD_TMF_PROXY = (SPINEL_CAP_THREAD__BEGIN + 1),
        SPINEL_CAP_THREAD_UDP_FORWARD = (SPINEL_CAP_THREAD__BEGIN + 2),
        SPINEL_CAP_THREAD_JOINER = (SPINEL_CAP_THREAD__BEGIN + 3),
        SPINEL_CAP_THREAD_BORDER_ROUTER = (SPINEL_CAP_THREAD__BEGIN + 4),
        SPINEL_CAP_THREAD_SERVICE = (SPINEL_CAP_THREAD__BEGIN + 5),
        SPINEL_CAP_THREAD__END = 1152,
    }

    public enum SpinelNetRole : byte
    {
        SPINEL_NET_ROLE_DETACHED = 0,
        SPINEL_NET_ROLE_CHILD = 1,
        SPINEL_NET_ROLE_ROUTER = 2,
        SPINEL_NET_ROLE_LEADER = 3
    }

    public enum SpinelIPv6ICMPPingOffloadMode : byte
    {
        SPINEL_IPV6_ICMP_PING_OFFLOAD_DISABLED = 0,
        SPINEL_IPV6_ICMP_PING_OFFLOAD_UNICAST_ONLY = 1,
        SPINEL_IPV6_ICMP_PING_OFFLOAD_MULTICAST_ONLY = 2,
        SPINEL_IPV6_ICMP_PING_OFFLOAD_ALL = 3,
    }

    public enum SpinelProtocolType : uint
    {
        SPINEL_PROTOCOL_TYPE_BOOTLOADER = 0,
        SPINEL_PROTOCOL_TYPE_ZIGBEE_IP = 2,
        SPINEL_PROTOCOL_TYPE_THREAD = 3
    }

    public enum SpinelHostPowerState : uint
    {
        SPINEL_HOST_POWER_STATE_OFFLINE = 0,
        SPINEL_HOST_POWER_STATE_DEEP_SLEEP = 1,
        SPINEL_HOST_POWER_STATE_RESERVED = 2,
        SPINEL_HOST_POWER_STATE_LOW_POWER = 3,
        SPINEL_HOST_POWER_STATE_ONLINE = 4
    }

    public enum SpinelMcuPowerState : uint
    {
        SPINEL_MCU_POWER_STATE_ON = 0,
        SPINEL_MCU_POWER_STATE_LOW_POWER = 1,
        SPINEL_MCU_POWER_STATE_OFF = 2
    }

    public enum SpinelScanState : byte
    {
        SPINEL_SCAN_STATE_IDLE = 0,
        SPINEL_SCAN_STATE_BEACON = 1,
        SPINEL_SCAN_STATE_ENERGY = 2,
        SPINEL_SCAN_STATE_DISCOVER = 3
    }

    public enum SpinelStatus
    {
        //Ok = 0,  ///< Operation has completed successfully.
        //Failure = 1,  ///< Operation has failed for some undefined reason.
        //Unimplemented = 2,  ///< Given operation has not been implemented.
        //Invalid_Argument = 3,  ///< An argument to the operation is invalid.
        //Invalid_State = 4,  ///< This operation is invalid for the current device state.
        //Invalid_Command = 5,  ///< This command is not recognized.
        //Invalid_Interface = 6,  ///< This interface is not supported.
        //Internal_error = 7,  ///< An internal runtime error has occured.
        //Security_error = 8,  ///< A security/authentication error has occured.
        //Parse_error = 9,  ///< A error has occured while parsing the command.
        //In_progress = 10, ///< This operation is in progress.
        //Nomem = 11, ///< Operation prevented due to memory pressure.
        //Busy = 12, ///< The device is currently performing a mutually exclusive operation
        //Prop_not_found = 13, ///< The given property is not recognized.
        //Dropped = 14, ///< A/The packet was dropped.
        //Empty = 15, ///< The result of the operation is empty.
        //Cmd_too_big = 16, ///< The command was too large to fit in the internal buffer.
        //No_ack = 17, ///< The packet was not acknowledged.
        //Cca_failure = 18, ///< The packet was not sent due to a CCA failure.
        //Already = 19, ///< The operation is already in progress.
        //Item_not_found = 20, ///< The given item could not be found.
        //Invalid_command_for_prop = 21, ///< The given command cannot be performed on this property.
        //STATUS_RESET_POWER_ON = 112,
        //STATUS_RESET_EXTERNAL = 113,
        //Status_Reset_Software = 114,
        //STATUS_RESET_FAULT = 115,
        //STATUS_RESET_CRASH = 116,
        //STATUS_RESET_ASSERT = 117,
        //STATUS_RESET_OTHER = 118,
        //STATUS_RESET_UNKNOWN = 119,
        //STATUS_RESET_WATCHDOG = 120

        SPINEL_STATUS_OK = 0,  ///< Operation has completed successfully.
        SPINEL_STATUS_FAILURE = 1,  ///< Operation has failed for some undefined reason.
        SPINEL_STATUS_UNIMPLEMENTED = 2,  ///< Given operation has not been implemented.
        SPINEL_STATUS_INVALID_ARGUMENT = 3,  ///< An argument to the operation is invalid.
        SPINEL_STATUS_INVALID_STATE = 4,  ///< This operation is invalid for the current device state.
        SPINEL_STATUS_INVALID_COMMAND = 5,  ///< This command is not recognized.
        SPINEL_STATUS_INVALID_INTERFACE = 6,  ///< This interface is not supported.
        SPINEL_STATUS_INTERNAL_ERROR = 7,  ///< An internal runtime error has occurred.
        SPINEL_STATUS_SECURITY_ERROR = 8,  ///< A security/authentication error has occurred.
        SPINEL_STATUS_PARSE_ERROR = 9,  ///< A error has occurred while parsing the command.
        SPINEL_STATUS_IN_PROGRESS = 10, ///< This operation is in progress.
        SPINEL_STATUS_NOMEM = 11, ///< Operation prevented due to memory pressure.
        SPINEL_STATUS_BUSY = 12, ///< The device is currently performing a mutually exclusive operation
        SPINEL_STATUS_PROP_NOT_FOUND = 13, ///< The given property is not recognized.
        SPINEL_STATUS_DROPPED = 14, ///< A/The packet was dropped.
        SPINEL_STATUS_EMPTY = 15, ///< The result of the operation is empty.
        SPINEL_STATUS_CMD_TOO_BIG = 16, ///< The command was too large to fit in the internal buffer.
        SPINEL_STATUS_NO_ACK = 17, ///< The packet was not acknowledged.
        SPINEL_STATUS_CCA_FAILURE = 18, ///< The packet was not sent due to a CCA failure.
        SPINEL_STATUS_ALREADY = 19, ///< The operation is already in progress.
        SPINEL_STATUS_ITEM_NOT_FOUND = 20, ///< The given item could not be found.
        SPINEL_STATUS_INVALID_COMMAND_FOR_PROP = 21, ///< The given command cannot be performed on this property.
        SPINEL_STATUS_UNKNOWN_NEIGHBOR = 22, ///< The neighbor is unknown.
        SPINEL_STATUS_NOT_CAPABLE = 23, ///< The target is not capable of handling requested operation.
        SPINEL_STATUS_RESPONSE_TIMEOUT = 24, ///< No response received from remote node

        SPINEL_STATUS_JOIN__BEGIN = 104,

        /// Generic failure to associate with other peers.
        /**
         *  This status error should not be used by implementors if
         *  enough information is available to determine that one of the
         *  later join failure status codes would be more accurate.
         *
         *  \sa SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING
         *  \sa SPINEL_PROP_MESHCOP_JOINER_COMMISSIONING
         */
        SPINEL_STATUS_JOIN_FAILURE = SPINEL_STATUS_JOIN__BEGIN + 0,

        /// The node found other peers but was unable to decode their packets.
        /**
         *  Typically this error code indicates that the network
         *  key has been set incorrectly.
         *
         *  \sa SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING
         *  \sa SPINEL_PROP_MESHCOP_JOINER_COMMISSIONING
         */
        SPINEL_STATUS_JOIN_SECURITY = SPINEL_STATUS_JOIN__BEGIN + 1,

        /// The node was unable to find any other peers on the network.
        /**
         *  \sa SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING
         *  \sa SPINEL_PROP_MESHCOP_JOINER_COMMISSIONING
         */
        SPINEL_STATUS_JOIN_NO_PEERS = SPINEL_STATUS_JOIN__BEGIN + 2,

        /// The only potential peer nodes found are incompatible.
        /**
         *  \sa SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING
         */
        SPINEL_STATUS_JOIN_INCOMPATIBLE = SPINEL_STATUS_JOIN__BEGIN + 3,

        /// No response in expecting time.
        /**
         *  \sa SPINEL_PROP_MESHCOP_JOINER_COMMISSIONING
         */
        SPINEL_STATUS_JOIN_RSP_TIMEOUT = SPINEL_STATUS_JOIN__BEGIN + 4,

        /// The node succeeds in commissioning and get the network credentials.
        /**
         *  \sa SPINEL_PROP_MESHCOP_JOINER_COMMISSIONING
         */
        SPINEL_STATUS_JOIN_SUCCESS = SPINEL_STATUS_JOIN__BEGIN + 5,

        SPINEL_STATUS_JOIN__END = 112,

        SPINEL_STATUS_RESET__BEGIN = 112,
        SPINEL_STATUS_RESET_POWER_ON = SPINEL_STATUS_RESET__BEGIN + 0,
        SPINEL_STATUS_RESET_EXTERNAL = SPINEL_STATUS_RESET__BEGIN + 1,
        SPINEL_STATUS_RESET_SOFTWARE = SPINEL_STATUS_RESET__BEGIN + 2,
        SPINEL_STATUS_RESET_FAULT = SPINEL_STATUS_RESET__BEGIN + 3,
        SPINEL_STATUS_RESET_CRASH = SPINEL_STATUS_RESET__BEGIN + 4,
        SPINEL_STATUS_RESET_ASSERT = SPINEL_STATUS_RESET__BEGIN + 5,
        SPINEL_STATUS_RESET_OTHER = SPINEL_STATUS_RESET__BEGIN + 6,
        SPINEL_STATUS_RESET_UNKNOWN = SPINEL_STATUS_RESET__BEGIN + 7,
        SPINEL_STATUS_RESET_WATCHDOG = SPINEL_STATUS_RESET__BEGIN + 8,
        SPINEL_STATUS_RESET__END = 128,

        SPINEL_STATUS_VENDOR__BEGIN = 15360,
        SPINEL_STATUS_VENDOR__END = 16384,

        SPINEL_STATUS_STACK_NATIVE__BEGIN = 16384,
        SPINEL_STATUS_STACK_NATIVE__END = 81920,

        SPINEL_STATUS_EXPERIMENTAL__BEGIN = 2000000,
        SPINEL_STATUS_EXPERIMENTAL__END = 2097152,
    }

    public static class SpinelTools
    {
        public static Hashtable NetRoleToStr = new Hashtable()
        {
            {SpinelNetRole.SPINEL_NET_ROLE_DETACHED, "DETACHED"},
            {SpinelNetRole.SPINEL_NET_ROLE_CHILD, "CHILD"},
            {SpinelNetRole.SPINEL_NET_ROLE_ROUTER, "ROUTER"},
            {SpinelNetRole.SPINEL_NET_ROLE_LEADER, "LEADER"},
        };

        public static string SpinelNetRoleToString(SpinelNetRole NetRole)
        {
            return (string)NetRoleToStr[NetRole];
            //foreach(DictionaryEntry row in NetRoleToStr)
            //{
            //    if ((SpinelNetRole)row.Key == NetRole) return (string)row.Value;
            //}

            //   return "unknown";
        }

        public static SpinelNetRole StringToSpinelNetRole(string NetRole)
        {
            SpinelNetRole returnNetRole = SpinelNetRole.SPINEL_NET_ROLE_DETACHED;

            foreach (DictionaryEntry row in NetRoleToStr)
            {
                if ((string)row.Value == NetRole)
                {
                    returnNetRole = (SpinelNetRole)row.Key;
                    break;
                }
            }

            return returnNetRole;
        }
    }

    public enum SpinelProperties : int
    {


        ////=========================================
        //// Spinel Properties
        ////=========================================
        SPINEL_PROP_LAST_STATUS = 0, ///** Format: `i` - Read-only

        SPINEL_PROP_PROTOCOL_VERSION = 1, ///** Format: `ii` - Read-only

        SPINEL_PROP_NCP_VERSION = 2,// /** Format: `U` - Read-only

        SPINEL_PROP_INTERFACE_TYPE = 3, // // Format: 'i' - Read-only
        SPINEL_PROP_VENDOR_ID = 4, // Format: 'i` - Read-only
        SPINEL_PROP_CAPS = 5,  // < capability list Format: 'A(i)` - Read-only
        SPINEL_PROP_INTERFACE_COUNT = 6, // < Interface count [C]
        SPINEL_PROP_POWER_STATE = 7,  // < PowerState [C] (deprecated, use `MCU_POWER_STATE` instead).
        SPINEL_PROP_HWADDR = 8, // NCP Hardware Address Format: 'E` - Read-only
        SPINEL_PROP_LOCK = 9, // < //< PropLock [b] (not supported)
        SPINEL_PROP_HBO_MEM_MAX = 10, ///< Max offload mem [S] (not supported)
        SPINEL_PROP_HBO_BLOCK_MAX = 11, ///< Max offload block [S] (not supported)
        SPINEL_PROP_HOST_POWER_STATE = 12,  // < PowerState [C]
        SPINEL_PROP_MCU_POWER_STATE = 13,  // < PowerState [C]

        SPINEL_PROP_PHY__BEGIN = 0x20,
        SPINEL_PROP_PHY_ENABLED = SPINEL_PROP_PHY__BEGIN + 0, // < [b]
        SPINEL_PROP_PHY_CHAN = SPINEL_PROP_PHY__BEGIN + 1, // < [C]
        SPINEL_PROP_PHY_CHAN_SUPPORTED = SPINEL_PROP_PHY__BEGIN + 2,  // < [A(C)]
        SPINEL_PROP_PHY_FREQ = SPINEL_PROP_PHY__BEGIN + 3,  // < kHz [L]
        SPINEL_PROP_PHY_CCA_THRESHOLD = SPINEL_PROP_PHY__BEGIN + 4,  // < dBm [c]
        SPINEL_PROP_PHY_TX_POWER = SPINEL_PROP_PHY__BEGIN + 5,  // < [c]
        SPINEL_PROP_PHY_RSSI = SPINEL_PROP_PHY__BEGIN + 6,  // < dBm [c]
        SPINEL_PROP_PHY_RX_SENSITIVITY = SPINEL_PROP_PHY__BEGIN + 7,  // < dBm [c]
        SPINEL_PROP_PHY_PCAP_ENABLED = SPINEL_PROP_PHY__BEGIN + 8,  ///< [b]
        SPINEL_PROP_PHY_CHAN_PREFERRED = SPINEL_PROP_PHY__BEGIN + 9,  ///< [A(C)]
        SPINEL_PROP_PHY_FEM_LNA_GAIN = SPINEL_PROP_PHY__BEGIN + 10, ///< dBm [c]
        SPINEL_PROP_PHY_CHAN_MAX_POWER = SPINEL_PROP_PHY__BEGIN + 11, ///Signal the max power for a channel Format: `Cc` First byte is the channel then the max transmit power, write-only.
        SPINEL_PROP_PHY_REGION_CODE = SPINEL_PROP_PHY__BEGIN + 12, /// Region code Format: `S` The ascii representation of the ISO 3166 alpha-2 code.
        SPINEL_PROP_PHY__END = 0x30,

        SPINEL_PROP_MAC__BEGIN = 0x30,
        SPINEL_PROP_MAC_SCAN_STATE = SPINEL_PROP_MAC__BEGIN + 0,//< [C]
        SPINEL_PROP_MAC_SCAN_MASK = SPINEL_PROP_MAC__BEGIN + 1,//< [A(C)]
        SPINEL_PROP_MAC_SCAN_PERIOD = SPINEL_PROP_MAC__BEGIN + 2,//< ms-per-channel [S]
                                                                 //< chan,rssi,(laddr,saddr,panid,lqi),(proto,xtra) [Cct(ESSC)t(i)]
        SPINEL_PROP_MAC_SCAN_BEACON = SPINEL_PROP_MAC__BEGIN + 3,
        SPINEL_PROP_MAC_15_4_LADDR = SPINEL_PROP_MAC__BEGIN + 4,//< [E]
        SPINEL_PROP_MAC_15_4_SADDR = SPINEL_PROP_MAC__BEGIN + 5,//< [S]
        SPINEL_PROP_MAC_15_4_PANID = SPINEL_PROP_MAC__BEGIN + 6,//< [S]
        SPINEL_PROP_MAC_RAW_STREAM_ENABLED = SPINEL_PROP_MAC__BEGIN + 7,//< [C]
        SPINEL_PROP_MAC_FILTER_MODE = SPINEL_PROP_MAC__BEGIN + 8,//< [C]
        SPINEL_PROP_MAC_ENERGY_SCAN_RESULT = SPINEL_PROP_MAC__BEGIN + 9,// `C`: Channel `c`: RSSI (in dBm)


        PROP_MAC__END = 0x40,

        SPINEL_PROP_NET__BEGIN = 0x40,
        SPINEL_PROP_NET_SAVED = SPINEL_PROP_NET__BEGIN + 0,//< [b]
        SPINEL_PROP_NET_IF_UP = SPINEL_PROP_NET__BEGIN + 1,//< [b]
        SPINEL_PROP_NET_STACK_UP = SPINEL_PROP_NET__BEGIN + 2,//< [C]
        SPINEL_PROP_NET_ROLE = SPINEL_PROP_NET__BEGIN + 3,//< [C]
        SPINEL_PROP_NET_NETWORK_NAME = SPINEL_PROP_NET__BEGIN + 4,//< [U]
        SPINEL_PROP_NET_XPANID = SPINEL_PROP_NET__BEGIN + 5,//< [D]
        SPINEL_PROP_NET_NETWORK_KEY = SPINEL_PROP_NET__BEGIN + 6,//< [D]
        SPINEL_PROP_NET_KEY_SEQUENCE_COUNTER = SPINEL_PROP_NET__BEGIN + 7,//< [L]
        SPINEL_PROP_NET_PARTITION_ID = SPINEL_PROP_NET__BEGIN + 8,//< [L]
        SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING = SPINEL_PROP_NET__BEGIN + 9,//< [b]        
        SPINEL_PROP_NET_KEY_SWITCH_GUARDTIME = SPINEL_PROP_NET__BEGIN + 10,//< [L]
        SPINEL_PROP_NET_PSKC = SPINEL_PROP_NET__BEGIN + 11,//< [D]
        PROP_NET__END = 0x50,

        SPINEL_PROP_THREAD__BEGIN = 0x50,
        SPINEL_PROP_THREAD_LEADER_ADDR = SPINEL_PROP_THREAD__BEGIN + 0,//< [6]
        SPINEL_PROP_THREAD_PARENT = SPINEL_PROP_THREAD__BEGIN + 1,//< LADDR, SADDR [ES]
        SPINEL_PROP_THREAD_CHILD_TABLE = SPINEL_PROP_THREAD__BEGIN + 2,//< [A(t(ES))] /** Format: [A(t(ESLLCCcCc)] - Read only
        SPINEL_PROP_THREAD_LEADER_RID = SPINEL_PROP_THREAD__BEGIN + 3,//< [C]
        SPINEL_PROP_THREAD_LEADER_WEIGHT = SPINEL_PROP_THREAD__BEGIN + 4,//< [C]
        SPINEL_PROP_THREAD_LOCAL_LEADER_WEIGHT = SPINEL_PROP_THREAD__BEGIN + 5,//< [C]
        SPINEL_PROP_THREAD_NETWORK_DATA = SPINEL_PROP_THREAD__BEGIN + 6,//< [D]
        SPINEL_PROP_THREAD_NETWORK_DATA_VERSION = SPINEL_PROP_THREAD__BEGIN + 7,//< [S]
        SPINEL_PROP_THREAD_STABLE_NETWORK_DATA = SPINEL_PROP_THREAD__BEGIN + 8,//< [D]
        SPINEL_PROP_THREAD_STABLE_NETWORK_DATA_VERSION = SPINEL_PROP_THREAD__BEGIN + 9,//< [S]
                                                                                       //< array(ipv6prefix,prefixlen,stable,flags) [A(t(6CbC))]
        SPINEL_PROP_THREAD_ON_MESH_NETS = SPINEL_PROP_THREAD__BEGIN + 10,
        //< array(ipv6prefix,prefixlen,stable,flags) [A(t(6CbC))]
        SPINEL_PROP_THREAD_OFF_MESH_ROUTES = SPINEL_PROP_THREAD__BEGIN + 11,
        SPINEL_PROP_THREAD_ASSISTING_PORTS = SPINEL_PROP_THREAD__BEGIN + 12,//< array(portn) [A(S)]
        SPINEL_PROP_THREAD_ALLOW_LOCAL_NET_DATA_CHANGE = SPINEL_PROP_THREAD__BEGIN + 13,//< [b]
        SPINEL_PROP_THREAD_MODE = SPINEL_PROP_THREAD__BEGIN + 14,

        SPINEL_PROP_IPV6__BEGIN = 0x60,

        /// Link-Local IPv6 Address
        /** Format: `6` - Read only
         *
         */
        SPINEL_PROP_IPV6_LL_ADDR = SPINEL_PROP_IPV6__BEGIN + 0, ///< [6]

                                                                /// Mesh Local IPv6 Address
        /** Format: `6` - Read only
         *
         */
        SPINEL_PROP_IPV6_ML_ADDR = SPINEL_PROP_IPV6__BEGIN + 1,

        /// Mesh Local Prefix
        /** Format: `6C` - Read-write
         *
         * Provides Mesh Local Prefix
         *
         *   `6`: Mesh local prefix
         *   `C` : Prefix length (64 bit for Thread).
         *
         */
        SPINEL_PROP_IPV6_ML_PREFIX = SPINEL_PROP_IPV6__BEGIN + 2,

        /// IPv6 (Unicast) Address Table
        /** Format: `A(t(6CLLC))`
         *
         * This property provides all unicast addresses.
         *
         * Array of structures containing:
         *
         *  `6`: IPv6 Address
         *  `C`: Network Prefix Length
         *  `L`: Valid Lifetime
         *  `L`: Preferred Lifetime
         *
         */
        SPINEL_PROP_IPV6_ADDRESS_TABLE = SPINEL_PROP_IPV6__BEGIN + 3,

        /// IPv6 Route Table - Deprecated
        SPINEL_PROP_IPV6_ROUTE_TABLE = SPINEL_PROP_IPV6__BEGIN + 4,

        /// IPv6 ICMP Ping Offload
        /** Format: `b`
         *
         * Allow the NCP to directly respond to ICMP ping requests. If this is
         * turned on, ping request ICMP packets will not be passed to the host.
         *
         * Default value is `false`.
         */
        SPINEL_PROP_IPV6_ICMP_PING_OFFLOAD = SPINEL_PROP_IPV6__BEGIN + 5,

        /// IPv6 Multicast Address Table
        /** Format: `A(t(6))`
         *
         * This property provides all multicast addresses.
         *
         */
        SPINEL_PROP_IPV6_MULTICAST_ADDRESS_TABLE = SPINEL_PROP_IPV6__BEGIN + 6,

        /// IPv6 ICMP Ping Offload
        /** Format: `C`
         *
         * Allow the NCP to directly respond to ICMP ping requests. If this is
         * turned on, ping request ICMP packets will not be passed to the host.
         *
         * This property allows enabling responses sent to unicast only, multicast
         * only, or both. The valid value are defined by enumeration
         * `spinel_ipv6_icmp_ping_offload_mode_t`.
         *
         *   SPINEL_IPV6_ICMP_PING_OFFLOAD_DISABLED       = 0
         *   SPINEL_IPV6_ICMP_PING_OFFLOAD_UNICAST_ONLY   = 1
         *   SPINEL_IPV6_ICMP_PING_OFFLOAD_MULTICAST_ONLY = 2
         *   SPINEL_IPV6_ICMP_PING_OFFLOAD_ALL            = 3
         *
         * Default value is `NET_IPV6_ICMP_PING_OFFLOAD_DISABLED`.
         *
         */
        SPINEL_PROP_IPV6_ICMP_PING_OFFLOAD_MODE = SPINEL_PROP_IPV6__BEGIN + 7, ///< [b]


        SPINEL_PROP_STREAM__BEGIN = 0x70,
        SPINEL_PROP_STREAM_DEBUG = SPINEL_PROP_STREAM__BEGIN + 0, //# <  Format: `U` (stream, read only)
        SPINEL_PROP_STREAM_RAW = SPINEL_PROP_STREAM__BEGIN + 1, // # < Format: `dD` (stream, read only)
        SPINEL_PROP_STREAM_NET = SPINEL_PROP_STREAM__BEGIN + 2, // # < Format: `dD` (stream, read only)
        SPINEL_PROP_STREAM_NET_INSECURE = SPINEL_PROP_STREAM__BEGIN + 3,//  # Format: `dD` (stream, read only)
        SPINEL_PROP_STREAM_LOG = SPINEL_PROP_STREAM__BEGIN + 4,//  # Format: `UD` (stream, read only)
        PROP_STREAM__END = 0x80,

        //  PROP_THREAD_EXT__BEGIN = 0x1500,
        //  PROP_THREAD_CHILD_TIMEOUT = PROP_THREAD_EXT__BEGIN + 0,//  // < [L]
        //  PROP_THREAD_RLOC16 = PROP_THREAD_EXT__BEGIN + 1,//  // < [S]
        //  PROP_THREAD_ROUTER_UPGRADE_THRESHOLD = PROP_THREAD_EXT__BEGIN + 2,  // < [C]
        //  PROP_THREAD_CONTEXT_REUSE_DELAY = PROP_THREAD_EXT__BEGIN + 3,  // < [L]
        //  PROP_THREAD_NETWORK_ID_TIMEOUT = PROP_THREAD_EXT__BEGIN + 4,  // < [b]
        //  PROP_THREAD_ACTIVE_ROUTER_IDS = PROP_THREAD_EXT__BEGIN + 5,  // < [A(b)]
        //  PROP_THREAD_RLOC16_DEBUG_PASSTHRU = PROP_THREAD_EXT__BEGIN + 6,  // < [b]
        //  PROP_THREAD_ROUTER_ROLE_ENABLED = PROP_THREAD_EXT__BEGIN + 7,  // < [b]
        //  PROP_THREAD_ROUTER_DOWNGRADE_THRESHOLD = PROP_THREAD_EXT__BEGIN + 8,  // < [C]
        //  PROP_THREAD_ROUTER_SELECTION_JITTER = PROP_THREAD_EXT__BEGIN + 9,  // < [C]
        //  PROP_THREAD_PREFERRED_ROUTER_ID = PROP_THREAD_EXT__BEGIN + 10,  // < [C]
        //  PROP_THREAD_NEIGHBOR_TABLE = PROP_THREAD_EXT__BEGIN + 11,  // < [A(t(ESLCcCbLL))]
        //  PROP_THREAD_CHILD_COUNT_MAX = PROP_THREAD_EXT__BEGIN + 12,  // < [C]

        SPINEL_PROP_THREAD_EXT__BEGIN = 0x1500,

        /// Thread Child Timeout
        /** Format: `L`
         *  Unit: Seconds
         *
         *  Used when operating in the Child role.
         */
        SPINEL_PROP_THREAD_CHILD_TIMEOUT = SPINEL_PROP_THREAD_EXT__BEGIN + 0,

        /// Thread RLOC16
        /** Format: `S`
         *
         */
        SPINEL_PROP_THREAD_RLOC16 = SPINEL_PROP_THREAD_EXT__BEGIN + 1,

        /// Thread Router Upgrade Threshold
        /** Format: `C`
         *
         */
        SPINEL_PROP_THREAD_ROUTER_UPGRADE_THRESHOLD = SPINEL_PROP_THREAD_EXT__BEGIN + 2,

        /// Thread Context Reuse Delay
        /** Format: `L`
         *
         */
        SPINEL_PROP_THREAD_CONTEXT_REUSE_DELAY = SPINEL_PROP_THREAD_EXT__BEGIN + 3,

        /// Thread Network ID Timeout
        /** Format: `C`
         *
         */
        SPINEL_PROP_THREAD_NETWORK_ID_TIMEOUT = SPINEL_PROP_THREAD_EXT__BEGIN + 4,

        /// List of active thread router ids
        /** Format: `A(C)`
         *
         * Note that some implementations may not support CMD_GET_VALUE
         * router ids, but may support CMD_REMOVE_VALUE when the node is
         * a leader.
         *
         */
        SPINEL_PROP_THREAD_ACTIVE_ROUTER_IDS = SPINEL_PROP_THREAD_EXT__BEGIN + 5,

        /// Forward IPv6 packets that use RLOC16 addresses to HOST.
        /** Format: `b`
         *
         * Allow host to directly observe all IPv6 packets received by the NCP,
         * including ones sent to the RLOC16 address.
         *
         * Default is false.
         *
         */
        SPINEL_PROP_THREAD_RLOC16_DEBUG_PASSTHRU = SPINEL_PROP_THREAD_EXT__BEGIN + 6,

        /// Router Role Enabled
        /** Format `b`
         *
         * Allows host to indicate whether or not the router role is enabled.
         * If current role is a router, setting this property to `false` starts
         * a re-attach process as an end-device.
         *
         */
        SPINEL_PROP_THREAD_ROUTER_ROLE_ENABLED = SPINEL_PROP_THREAD_EXT__BEGIN + 7,

        /// Thread Router Downgrade Threshold
        /** Format: `C`
         *
         */
        SPINEL_PROP_THREAD_ROUTER_DOWNGRADE_THRESHOLD = SPINEL_PROP_THREAD_EXT__BEGIN + 8,

        /// Thread Router Selection Jitter
        /** Format: `C`
         *
         */
        SPINEL_PROP_THREAD_ROUTER_SELECTION_JITTER = SPINEL_PROP_THREAD_EXT__BEGIN + 9,

        /// Thread Preferred Router Id
        /** Format: `C` - Write only
         *
         * Specifies the preferred Router Id. Upon becoming a router/leader the node
         * attempts to use this Router Id. If the preferred Router Id is not set or
         * if it can not be used, a randomly generated router id is picked. This
         * property can be set only when the device role is either detached or
         * disabled.
         *
         */
        SPINEL_PROP_THREAD_PREFERRED_ROUTER_ID = SPINEL_PROP_THREAD_EXT__BEGIN + 10,

        /// Thread Neighbor Table
        /** Format: `A(t(ESLCcCbLLc))` - Read only
         *
         * Data per item is:
         *
         *  `E`: Extended address
         *  `S`: RLOC16
         *  `L`: Age (in seconds)
         *  `C`: Link Quality In
         *  `c`: Average RSS (in dBm)
         *  `C`: Mode (bit-flags)
         *  `b`: `true` if neighbor is a child, `false` otherwise.
         *  `L`: Link Frame Counter
         *  `L`: MLE Frame Counter
         *  `c`: The last RSSI (in dBm)
         *
         */
        SPINEL_PROP_THREAD_NEIGHBOR_TABLE = SPINEL_PROP_THREAD_EXT__BEGIN + 11,

        /// Thread Max Child Count
        /** Format: `C`
         *
         * Specifies the maximum number of children currently allowed.
         * This parameter can only be set when Thread protocol operation
         * has been stopped.
         *
         */
        SPINEL_PROP_THREAD_CHILD_COUNT_MAX = SPINEL_PROP_THREAD_EXT__BEGIN + 12,

        /// Leader Network Data
        /** Format: `D` - Read only
         *
         */
        SPINEL_PROP_THREAD_LEADER_NETWORK_DATA = SPINEL_PROP_THREAD_EXT__BEGIN + 13,

        /// Stable Leader Network Data
        /** Format: `D` - Read only
         *
         */
        SPINEL_PROP_THREAD_STABLE_LEADER_NETWORK_DATA = SPINEL_PROP_THREAD_EXT__BEGIN + 14,

        /// Thread Joiner Data
        /** Format `A(T(ULE))`
         *  PSKd, joiner timeout, eui64 (optional)
         *
         * This property is being deprecated by SPINEL_PROP_MESHCOP_COMMISSIONER_JOINERS.
         *
         */
        SPINEL_PROP_THREAD_JOINERS = SPINEL_PROP_THREAD_EXT__BEGIN + 15,

        /// Thread Commissioner Enable
        /** Format `b`
         *
         * Default value is `false`.
         *
         * This property is being deprecated by SPINEL_PROP_MESHCOP_COMMISSIONER_STATE.
         *
         */
        SPINEL_PROP_THREAD_COMMISSIONER_ENABLED = SPINEL_PROP_THREAD_EXT__BEGIN + 16,

        /// Thread TMF proxy enable
        /** Format `b`
         * Required capability: `SPINEL_CAP_THREAD_TMF_PROXY`
         *
         * This property is deprecated.
         *
         */
        SPINEL_PROP_THREAD_TMF_PROXY_ENABLED = SPINEL_PROP_THREAD_EXT__BEGIN + 17,

        /// Thread TMF proxy stream
        /** Format `dSS`
         * Required capability: `SPINEL_CAP_THREAD_TMF_PROXY`
         *
         * This property is deprecated. Please see `SPINEL_PROP_THREAD_UDP_FORWARD_STREAM`.
         *
         */
        SPINEL_PROP_THREAD_TMF_PROXY_STREAM = SPINEL_PROP_THREAD_EXT__BEGIN + 18,

        /// Thread "joiner" flag used during discovery scan operation
        /** Format `b`
         *
         * This property defines the Joiner Flag value in the Discovery Request TLV.
         *
         * Default value is `false`.
         *
         */
        SPINEL_PROP_THREAD_DISCOVERY_SCAN_JOINER_FLAG = SPINEL_PROP_THREAD_EXT__BEGIN + 19,

        /// Enable EUI64 filtering for discovery scan operation.
        /** Format `b`
         *
         * Default value is `false`
         *
         */
        SPINEL_PROP_THREAD_DISCOVERY_SCAN_ENABLE_FILTERING = SPINEL_PROP_THREAD_EXT__BEGIN + 20,

        /// PANID used for Discovery scan operation (used for PANID filtering).
        /** Format: `S`
         *
         * Default value is 0xffff (Broadcast PAN) to disable PANID filtering
         *
         */
        SPINEL_PROP_THREAD_DISCOVERY_SCAN_PANID = SPINEL_PROP_THREAD_EXT__BEGIN + 21,

        /// Thread (out of band) steering data for MLE Discovery Response.
        /** Format `E` - Write only
         *
         * Required capability: SPINEL_CAP_OOB_STEERING_DATA.
         *
         * Writing to this property allows to set/update the MLE
         * Discovery Response steering data out of band.
         *
         *  - All zeros to clear the steering data (indicating that
         *    there is no steering data).
         *  - All 0xFFs to set steering data/bloom filter to
         *    accept/allow all.
         *  - A specific EUI64 which is then added to current steering
         *    data/bloom filter.
         *
         */
        SPINEL_PROP_THREAD_STEERING_DATA = SPINEL_PROP_THREAD_EXT__BEGIN + 22,

        /// Thread Router Table.
        /** Format: `A(t(ESCCCCCCb)` - Read only
         *
         * Data per item is:
         *
         *  `E`: IEEE 802.15.4 Extended Address
         *  `S`: RLOC16
         *  `C`: Router ID
         *  `C`: Next hop to router
         *  `C`: Path cost to router
         *  `C`: Link Quality In
         *  `C`: Link Quality Out
         *  `C`: Age (seconds since last heard)
         *  `b`: Link established with Router ID or not.
         *
         */
        SPINEL_PROP_THREAD_ROUTER_TABLE = SPINEL_PROP_THREAD_EXT__BEGIN + 23,

        /// Thread Active Operational Dataset
        /** Format: `A(t(iD))` - Read-Write
         *
         * This property provides access to current Thread Active Operational Dataset. A Thread device maintains the
         * Operational Dataset that it has stored locally and the one currently in use by the partition to which it is
         * attached. This property corresponds to the locally stored Dataset on the device.
         *
         * Operational Dataset consists of a set of supported properties (e.g., channel, master key, network name, PAN id,
         * etc). Note that not all supported properties may be present (have a value) in a Dataset.
         *
         * The Dataset value is encoded as an array of structs containing pairs of property key (as `i`) followed by the
         * property value (as `D`). The property value must follow the format associated with the corresponding property.
         *
         * On write, any unknown/unsupported property keys must be ignored.
         *
         * The following properties can be included in a Dataset list:
         *
         *   SPINEL_PROP_DATASET_ACTIVE_TIMESTAMP
         *   SPINEL_PROP_PHY_CHAN
         *   SPINEL_PROP_PHY_CHAN_SUPPORTED (Channel Mask Page 0)
         *   SPINEL_PROP_NET_MASTER_KEY
         *   SPINEL_PROP_NET_NETWORK_NAME
         *   SPINEL_PROP_NET_XPANID
         *   SPINEL_PROP_MAC_15_4_PANID
         *   SPINEL_PROP_IPV6_ML_PREFIX
         *   SPINEL_PROP_NET_PSKC
         *   SPINEL_PROP_DATASET_SECURITY_POLICY
         *
         */
        SPINEL_PROP_THREAD_ACTIVE_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 24,

        /// Thread Pending Operational Dataset
        /** Format: `A(t(iD))` - Read-Write
         *
         * This property provide access to current locally stored Pending Operational Dataset.
         *
         * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_ACTIVE_DATASET.
         *
         * In addition supported properties in SPINEL_PROP_THREAD_ACTIVE_DATASET, the following properties can also
         * be included in the Pending Dataset:
         *
         *   SPINEL_PROP_DATASET_PENDING_TIMESTAMP
         *   SPINEL_PROP_DATASET_DELAY_TIMER
         *
         */
        SPINEL_PROP_THREAD_PENDING_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 25,

        /// Send MGMT_SET Thread Active Operational Dataset
        /** Format: `A(t(iD))` - Write only
         *
         * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_ACTIVE_DATASET.
         *
         * This is write-only property. When written, it triggers a MGMT_ACTIVE_SET meshcop command to be sent to leader
         * with the given Dataset. The spinel frame response should be a `LAST_STATUS` with the status of the transmission
         * of MGMT_ACTIVE_SET command.
         *
         * In addition to supported properties in SPINEL_PROP_THREAD_ACTIVE_DATASET, the following property can be
         * included in the Dataset (to allow for custom raw TLVs):
         *
         *    SPINEL_PROP_DATASET_RAW_TLVS
         *
         */
        SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 26,

        /// Send MGMT_SET Thread Pending Operational Dataset
        /** Format: `A(t(iD))` - Write only
         *
         * This property is similar to SPINEL_PROP_THREAD_PENDING_DATASET and follows the same format and rules.
         *
         * In addition to supported properties in SPINEL_PROP_THREAD_PENDING_DATASET, the following property can be
         * included the Dataset (to allow for custom raw TLVs to be provided).
         *
         *    SPINEL_PROP_DATASET_RAW_TLVS
         *
         */
        SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 27,

        /// Operational Dataset Active Timestamp
        /** Format: `X` - No direct read or write
         *
         * It can only be included in one of the Dataset related properties below:
         *
         *   SPINEL_PROP_THREAD_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
         *
         */
        SPINEL_PROP_DATASET_ACTIVE_TIMESTAMP = SPINEL_PROP_THREAD_EXT__BEGIN + 28,

        /// Operational Dataset Pending Timestamp
        /** Format: `X` - No direct read or write
         *
         * It can only be included in one of the Pending Dataset properties:
         *
         *   SPINEL_PROP_THREAD_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
         *
         */
        SPINEL_PROP_DATASET_PENDING_TIMESTAMP = SPINEL_PROP_THREAD_EXT__BEGIN + 29,

        /// Operational Dataset Delay Timer
        /** Format: `L` - No direct read or write
         *
         * Delay timer (in ms) specifies the time renaming until Thread devices overwrite the value in the Active
         * Operational Dataset with the corresponding values in the Pending Operational Dataset.
         *
         * It can only be included in one of the Pending Dataset properties:
         *
         *   SPINEL_PROP_THREAD_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
         *
         */
        SPINEL_PROP_DATASET_DELAY_TIMER = SPINEL_PROP_THREAD_EXT__BEGIN + 30,

        /// Operational Dataset Security Policy
        /** Format: `SC` - No direct read or write
         *
         * It can only be included in one of the Dataset related properties below:
         *
         *   SPINEL_PROP_THREAD_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
         *
         * Content is
         *   `S` : Key Rotation Time (in units of hour)
         *   `C` : Security Policy Flags (as specified in Thread 1.1 Section 8.10.1.15)
         *
         */
        SPINEL_PROP_DATASET_SECURITY_POLICY = SPINEL_PROP_THREAD_EXT__BEGIN + 31,

        /// Operational Dataset Additional Raw TLVs
        /** Format: `D` - No direct read or write
         *
         * This property defines extra raw TLVs that can be added to an Operational DataSet.
         *
         * It can only be included in one of the following Dataset properties:
         *
         *   SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
         *
         */
        SPINEL_PROP_DATASET_RAW_TLVS = SPINEL_PROP_THREAD_EXT__BEGIN + 32,

        /// Child table addresses
        /** Format: `A(t(ESA(6)))` - Read only
         *
         * This property provides the list of all addresses associated with every child
         * including any registered IPv6 addresses.
         *
         * Data per item is:
         *
         *  `E`: Extended address of the child
         *  `S`: RLOC16 of the child
         *  `A(6)`: List of IPv6 addresses registered by the child (if any)
         *
         */
        SPINEL_PROP_THREAD_CHILD_TABLE_ADDRESSES = SPINEL_PROP_THREAD_EXT__BEGIN + 33,

        /// Neighbor Table Frame and Message Error Rates
        /** Format: `A(t(ESSScc))`
         *  Required capability: `CAP_ERROR_RATE_TRACKING`
         *
         * This property provides link quality related info including
         * frame and (IPv6) message error rates for all neighbors.
         *
         * With regards to message error rate, note that a larger (IPv6)
         * message can be fragmented and sent as multiple MAC frames. The
         * message transmission is considered a failure, if any of its
         * fragments fail after all MAC retry attempts.
         *
         * Data per item is:
         *
         *  `E`: Extended address of the neighbor
         *  `S`: RLOC16 of the neighbor
         *  `S`: Frame error rate (0 -> 0%, 0xffff -> 100%)
         *  `S`: Message error rate (0 -> 0%, 0xffff -> 100%)
         *  `c`: Average RSSI (in dBm)
         *  `c`: Last RSSI (in dBm)
         *
         */
        SPINEL_PROP_THREAD_NEIGHBOR_TABLE_ERROR_RATES = SPINEL_PROP_THREAD_EXT__BEGIN + 34,

        /// EID (Endpoint Identifier) IPv6 Address Cache Table
        /** Format `A(t(6SC))`
         *
         * This property provides Thread EID address cache table.
         *
         * Data per item is:
         *
         *  `6` : Target IPv6 address
         *  `S` : RLOC16 of target
         *  `C` : Age (order of use, 0 indicates most recently used entry)
         *
         */
        SPINEL_PROP_THREAD_ADDRESS_CACHE_TABLE = SPINEL_PROP_THREAD_EXT__BEGIN + 35,

        /// Thread UDP forward stream
        /** Format `dS6S`
         * Required capability: `SPINEL_CAP_THREAD_UDP_FORWARD`
         *
         * This property helps exchange UDP packets with host.
         *
         *  `d`: UDP payload
         *  `S`: Remote UDP port
         *  `6`: Remote IPv6 address
         *  `S`: Local UDP port
         *
         */
        SPINEL_PROP_THREAD_UDP_FORWARD_STREAM = SPINEL_PROP_THREAD_EXT__BEGIN + 36,

        /// Send MGMT_GET Thread Active Operational Dataset
        /** Format: `A(t(iD))` - Write only
         *
         * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET. This
         * property further allows the sender to not include a value associated with properties in formating of `t(iD)`,
         * i.e., it should accept either a `t(iD)` or a `t(i)` encoding (in both cases indicating that the associated
         * Dataset property should be requested as part of MGMT_GET command).
         *
         * This is write-only property. When written, it triggers a MGMT_ACTIVE_GET meshcop command to be sent to leader
         * requesting the Dataset related properties from the format. The spinel frame response should be a `LAST_STATUS`
         * with the status of the transmission of MGMT_ACTIVE_GET command.
         *
         * In addition to supported properties in SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET, the following property can be
         * optionally included in the Dataset:
         *
         *    SPINEL_PROP_DATASET_DEST_ADDRESS
         *
         */
        SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 37,

        /// Send MGMT_GET Thread Pending Operational Dataset
        /** Format: `A(t(iD))` - Write only
         *
         * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET.
         *
         * This is write-only property. When written, it triggers a MGMT_PENDING_GET meshcop command to be sent to leader
         * with the given Dataset. The spinel frame response should be a `LAST_STATUS` with the status of the transmission
         * of MGMT_PENDING_GET command.
         *
         */
        SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 38,

        /// Operational Dataset (MGMT_GET) Destination IPv6 Address
        /** Format: `6` - No direct read or write
         *
         * This property specifies the IPv6 destination when sending MGMT_GET command for either Active or Pending Dataset
         * if not provided, Leader ALOC address is used as default.
         *
         * It can only be included in one of the MGMT_GET Dataset properties:
         *
         *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
         *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
         *
         */
        SPINEL_PROP_DATASET_DEST_ADDRESS = SPINEL_PROP_THREAD_EXT__BEGIN + 39,
        PROP_THREAD_EXT__END = 0x1600,

        PROP_MESHCOP_EXT__BEGIN = 0x1600,
        PROP_MESHCOP_JOINER_ENABLE = PROP_MESHCOP_EXT__BEGIN + 0,  // < [b]
        PROP_MESHCOP_JOINER_CREDENTIAL = PROP_MESHCOP_EXT__BEGIN + 1,  // < [D]
        PROP_MESHCOP_JOINER_URL = PROP_MESHCOP_EXT__BEGIN + 2,  // < [U]
        PROP_MESHCOP_BORDER_AGENT_ENABLE = PROP_MESHCOP_EXT__BEGIN + 3,  // < [b]
        PROP_MESHCOP_EXT__END = 0x1700,

        //  PROP_IPV6__BEGIN = 0x60,
        //  PROP_IPV6_LL_ADDR = PROP_IPV6__BEGIN + 0, // // < [6]
        //  PROP_IPV6_ML_ADDR = PROP_IPV6__BEGIN + 1, // // < [6C]
        //  PROP_IPV6_ML_PREFIX = PROP_IPV6__BEGIN + 2, // // < [6C]                                                                 
        ////// < array(ipv6addr,prefixlen,valid,preferred,flags) [A(t(6CLLC))]
        //  PROP_IPV6_ADDRESS_TABLE = PROP_IPV6__BEGIN + 3,
        ////// < array(ipv6prefix,prefixlen,iface,flags) [A(t(6CCC))]
        //  PROP_IPV6_ROUTE_TABLE = PROP_IPV6__BEGIN + 4,
        //  PROP_IPv6_ICMP_PING_OFFLOAD = PROP_IPV6__BEGIN + 5,//  // < [b]

        SPINEL_PROP_CNTR__BEGIN = 1280,

        //// Counter reset behavior
        //// Format: `C`
        SPINEL_PROP_CNTR_RESET = SPINEL_PROP_CNTR__BEGIN + 0,

        //// The total number of transmissions.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_TOTAL = PROP_CNTR__BEGIN + 1

        //// The number of transmissions with ack request.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_ACK_REQ = PROP_CNTR__BEGIN + 2

        //// The number of transmissions that were acked.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_ACKED = PROP_CNTR__BEGIN + 3

        //// The number of transmissions without ack request.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_NO_ACK_REQ = PROP_CNTR__BEGIN + 4

        //// The number of transmitted data.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_DATA = PROP_CNTR__BEGIN + 5

        //// The number of transmitted data poll.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_DATA_POLL = PROP_CNTR__BEGIN + 6

        //// The number of transmitted beacon.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_BEACON = PROP_CNTR__BEGIN + 7

        //// The number of transmitted beacon request.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_BEACON_REQ = PROP_CNTR__BEGIN + 8

        //// The number of transmitted other types of frames.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_OTHER = PROP_CNTR__BEGIN + 9

        //// The number of retransmission times.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_PKT_RETRY = PROP_CNTR__BEGIN + 10

        //// The number of CCA failure times.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_TX_ERR_CCA = PROP_CNTR__BEGIN + 11

        //// The total number of received packets.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_TOTAL = PROP_CNTR__BEGIN + 100

        //// The number of received data.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_DATA = PROP_CNTR__BEGIN + 101

        //// The number of received data poll.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_DATA_POLL = PROP_CNTR__BEGIN + 102

        //// The number of received beacon.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_BEACON = PROP_CNTR__BEGIN + 103

        //// The number of received beacon request.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_BEACON_REQ = PROP_CNTR__BEGIN + 104

        //// The number of received other types of frames.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_OTHER = PROP_CNTR__BEGIN + 105

        //// The number of received packets filtered by whitelist.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_FILT_WL = PROP_CNTR__BEGIN + 106

        //// The number of received packets filtered by destination check.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_PKT_FILT_DA = PROP_CNTR__BEGIN + 107

        //// The number of received packets that are empty.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_ERR_EMPTY = PROP_CNTR__BEGIN + 108

        //// The number of received packets from an unknown neighbor.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_ERR_UKWN_NBR = PROP_CNTR__BEGIN + 109

        //// The number of received packets whose source address is invalid.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_ERR_NVLD_SADDR = PROP_CNTR__BEGIN + 110

        //// The number of received packets with a security error.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_ERR_SECURITY = PROP_CNTR__BEGIN + 111

        //// The number of received packets with a checksum error.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_ERR_BAD_FCS = PROP_CNTR__BEGIN + 112

        //// The number of received packets with other errors.
        //// Format: `L` (Read-only) */
        //PROP_CNTR_RX_ERR_OTHER = PROP_CNTR__BEGIN + 113

        // The message buffer counter info
        // Format: `SSSSSSSSSSSSSSSS` (Read-only)
        //     `S`, (TotalBuffers)           The number of buffers in the pool.
        //     `S`, (FreeBuffers)            The number of free message buffers.
        //     `S`, (6loSendMessages)        The number of messages in the 6lo send queue.
        //     `S`, (6loSendBuffers)         The number of buffers in the 6lo send queue.
        //     `S`, (6loReassemblyMessages)  The number of messages in the 6LoWPAN reassembly queue.
        //     `S`, (6loReassemblyBuffers)   The number of buffers in the 6LoWPAN reassembly queue.
        //     `S`, (Ip6Messages)            The number of messages in the IPv6 send queue.
        //     `S`, (Ip6Buffers)             The number of buffers in the IPv6 send queue.
        //     `S`, (MplMessages)            The number of messages in the MPL send queue.
        //     `S`, (MplBuffers)             The number of buffers in the MPL send queue.
        //     `S`, (MleMessages)            The number of messages in the MLE send queue.
        //     `S`, (MleBuffers)             The number of buffers in the MLE send queue.
        //     `S`, (ArpMessages)            The number of messages in the ARP send queue.
        //     `S`, (ArpBuffers)             The number of buffers in the ARP send queue.
        //     `S`, (CoapClientMessages)     The number of messages in the CoAP client send queue.
        //     `S`  (CoapClientBuffers)      The number of buffers in the CoAP client send queue.
        SPINEL_PROP_MSG_BUFFER_COUNTERS = SPINEL_PROP_CNTR__BEGIN + 400,
        SPINEL_PROP_CNTR__END = 0x800,

        SPINEL_PROP_OPENTHREAD__BEGIN = 0x1900,

        /// Channel Manager - Channel Change New Channel
        /** Format: `C` (read-write)
         *
         * Required capability: SPINEL_CAP_CHANNEL_MANAGER
         *
         * Setting this property triggers the Channel Manager to start
         * a channel change process. The network switches to the given
         * channel after the specified delay (see `CHANNEL_MANAGER_DELAY`).
         *
         * A subsequent write to this property will cancel an ongoing
         * (previously requested) channel change.
         *
         */
        SPINEL_PROP_CHANNEL_MANAGER_NEW_CHANNEL = SPINEL_PROP_OPENTHREAD__BEGIN + 0,

        /// Channel Manager - Channel Change Delay
        /** Format 'S'
         *  Units: seconds
         *
         * Required capability: SPINEL_CAP_CHANNEL_MANAGER
         *
         * This property specifies the delay (in seconds) to be used for
         * a channel change request.
         *
         * The delay should preferably be longer than maximum data poll
         * interval used by all sleepy-end-devices within the Thread
         * network.
         *
         */
        SPINEL_PROP_CHANNEL_MANAGER_DELAY = SPINEL_PROP_OPENTHREAD__BEGIN + 1,

        /// Channel Manager Supported Channels
        /** Format 'A(C)'
         *
         * Required capability: SPINEL_CAP_CHANNEL_MANAGER
         *
         * This property specifies the list of supported channels.
         *
         */
        SPINEL_PROP_CHANNEL_MANAGER_SUPPORTED_CHANNELS = SPINEL_PROP_OPENTHREAD__BEGIN + 2,

        /// Channel Manager Favored Channels
        /** Format 'A(C)'
         *
         * Required capability: SPINEL_CAP_CHANNEL_MANAGER
         *
         * This property specifies the list of favored channels (when `ChannelManager` is asked to select channel)
         *
         */
        SPINEL_PROP_CHANNEL_MANAGER_FAVORED_CHANNELS = SPINEL_PROP_OPENTHREAD__BEGIN + 3,

        /// Channel Manager Channel Select Trigger
        /** Format 'b'
         *
         * Required capability: SPINEL_CAP_CHANNEL_MANAGER
         *
         * Writing to this property triggers a request on `ChannelManager` to select a new channel.
         *
         * Once a Channel Select is triggered, the Channel Manager will perform the following 3 steps:
         *
         * 1) `ChannelManager` decides if the channel change would be helpful. This check can be skipped if in the input
         *    boolean to this property is set to `true` (skipping the quality check).
         *    This step uses the collected link quality metrics on the device such as CCA failure rate, frame and message
         *    error rates per neighbor, etc. to determine if the current channel quality is at the level that justifies
         *    a channel change.
         *
         * 2) If first step passes, then `ChannelManager` selects a potentially better channel. It uses the collected
         *    channel quality data by `ChannelMonitor` module. The supported and favored channels are used at this step.
         *
         * 3) If the newly selected channel is different from the current channel, `ChannelManager` requests/starts the
         *    channel change process.
         *
         * Reading this property always yields `false`.
         *
         */
        SPINEL_PROP_CHANNEL_MANAGER_CHANNEL_SELECT = SPINEL_PROP_OPENTHREAD__BEGIN + 4,

        /// Channel Manager Auto Channel Selection Enabled
        /** Format 'b'
         *
         * Required capability: SPINEL_CAP_CHANNEL_MANAGER
         *
         * This property indicates if auto-channel-selection functionality is enabled/disabled on `ChannelManager`.
         *
         * When enabled, `ChannelManager` will periodically checks and attempts to select a new channel. The period interval
         * is specified by `SPINEL_PROP_CHANNEL_MANAGER_AUTO_SELECT_INTERVAL`.
         *
         */
        SPINEL_PROP_CHANNEL_MANAGER_AUTO_SELECT_ENABLED = SPINEL_PROP_OPENTHREAD__BEGIN + 5,

        /// Channel Manager Auto Channel Selection Interval
        /** Format 'L'
         *  units: seconds
         *
         * Required capability: SPINEL_CAP_CHANNEL_MANAGER
         *
         * This property specifies the auto-channel-selection check interval (in seconds).
         *
         */
        SPINEL_PROP_CHANNEL_MANAGER_AUTO_SELECT_INTERVAL = SPINEL_PROP_OPENTHREAD__BEGIN + 6,

        /// Thread network time.
        /** Format: `Xc` - Read only
         *
         * Data per item is:
         *
         *  `X`: The Thread network time, in microseconds.
         *  `c`: Time synchronization status.
         *
         */
        SPINEL_PROP_THREAD_NETWORK_TIME = SPINEL_PROP_OPENTHREAD__BEGIN + 7,

        /// Thread time synchronization period
        /** Format: `S` - Read-Write
         *
         * Data per item is:
         *
         *  `S`: Time synchronization period, in seconds.
         *
         */
        SPINEL_PROP_TIME_SYNC_PERIOD = SPINEL_PROP_OPENTHREAD__BEGIN + 8,

        /// Thread Time synchronization XTAL accuracy threshold for Router
        /** Format: `S` - Read-Write
         *
         * Data per item is:
         *
         *  `S`: The XTAL accuracy threshold for Router, in PPM.
         *
         */
        SPINEL_PROP_TIME_SYNC_XTAL_THRESHOLD = SPINEL_PROP_OPENTHREAD__BEGIN + 9,

        /// Child Supervision Interval
        /** Format: `S` - Read-Write
         *  Units: Seconds
         *
         * Required capability: `SPINEL_CAP_CHILD_SUPERVISION`
         *
         * The child supervision interval (in seconds). Zero indicates that child supervision is disabled.
         *
         * When enabled, Child supervision feature ensures that at least one message is sent to every sleepy child within
         * the given supervision interval. If there is no other message, a supervision message (a data message with empty
         * payload) is enqueued and sent to the child.
         *
         * This property is available for FTD build only.
         *
         */
        SPINEL_PROP_CHILD_SUPERVISION_INTERVAL = SPINEL_PROP_OPENTHREAD__BEGIN + 10,

        /// Child Supervision Check Timeout
        /** Format: `S` - Read-Write
         *  Units: Seconds
         *
         * Required capability: `SPINEL_CAP_CHILD_SUPERVISION`
         *
         * The child supervision check timeout interval (in seconds). Zero indicates supervision check on the child is
         * disabled.
         *
         * Supervision check is only applicable on a sleepy child. When enabled, if the child does not hear from its parent
         * within the specified check timeout, it initiates a re-attach process by starting an MLE Child Update
         * Request/Response exchange with the parent.
         *
         * This property is available for FTD and MTD builds.
         *
         */
        SPINEL_PROP_CHILD_SUPERVISION_CHECK_TIMEOUT = SPINEL_PROP_OPENTHREAD__BEGIN + 11,

        // RCP (NCP in radio only mode) version
        /** Format `U` - Read only
         *
         * Required capability: SPINEL_CAP_POSIX_APP
         *
         * This property gives the version string of RCP (NCP in radio mode) which is being controlled by the POSIX
         * application. It is available only in "POSIX Application" configuration (i.e., `OPENTHREAD_ENABLE_POSIX_APP` is
         * enabled).
         *
         */
        SPINEL_PROP_RCP_VERSION = SPINEL_PROP_OPENTHREAD__BEGIN + 12,

        /// Thread Parent Response info
        /** Format: `ESccCCCb` - Asynchronous event only
         *
         *  `E`: Extended address
         *  `S`: RLOC16
         *  `c`: Instant RSSI
         *  'c': Parent Priority
         *  `C`: Link Quality3
         *  `C`: Link Quality2
         *  `C`: Link Quality1
         *  'b': Is the node receiving parent response frame attached
         *
         * This property sends Parent Response frame information to the Host.
         * This property is available for FTD build only.
         *
         */
        SPINEL_PROP_PARENT_RESPONSE_INFO = SPINEL_PROP_OPENTHREAD__BEGIN + 13,

        /// SLAAC enabled
        /** Format `b` - Read-Write
         *  Required capability: `SPINEL_CAP_SLAAC`
         *
         * This property allows the host to enable/disable SLAAC module on NCP at run-time. When SLAAC module is enabled,
         * SLAAC addresses (based on on-mesh prefixes in Network Data) are added to the interface. When SLAAC module is
         * disabled any previously added SLAAC address is removed.
         *
         */
        SPINEL_PROP_SLAAC_ENABLED = SPINEL_PROP_OPENTHREAD__BEGIN + 14,
    }
}
