using System;
using System.Collections;
using System.Threading;


#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.Spinel;
using nanoFramework.OpenThread.Core;

namespace nanoFramework.OpenThread.NCP
{

#else

using dotNETCore.OpenThread.Spinel;
using dotNETCore.OpenThread.Core;

namespace dotNETCore.OpenThread.NCP
{
#endif

    public class WpanApi
    {
        private const byte SpinelHeaderFlag = 0x80;
        private IStream stream;
        private Hdlc hdlcInterface;
        private SpinelEncoder mEncoder = new SpinelEncoder();
        private Queue waitingQueue = new Queue();
        private bool isSyncFrameExpecting = false;
        private AutoResetEvent receivedPacketWaitHandle = new AutoResetEvent(false);

        static object rxLocker = new object();
        static object txLocker = new object();

        public event FrameReceivedEventHandler OnFrameDataReceived;        
        public event SpinelPropertyChangedHandler OnPropertyChanged;

        /// <summary>
        ///
        /// </summary>
        public WpanApi()
        {
        }

        public void Open(string portName)
        {
            this.stream = new SerialStream(portName);
            this.hdlcInterface = new Hdlc(this.stream);
            this.stream.SerialDataReceived += new SerialDataReceivedEventHandler(StreamDataReceived);
            stream.Open();
        }

        public void DoReset()
        {
            Transact(SpinelCommands.CMD_RESET);
        }

        //**********************************************************************
        //
        // Spinel NET Core Properties
        //
        //**********************************************************************

        /// <summary>
        /// Last Operation Status
        /// </summary>
        /// <returns>uint see `SPINEL_STATUS_*` for list of values</returns>
        protected internal uint GetPropLastStatus()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_LAST_STATUS);
            try
            {
                return (uint)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel Last Operation Status format violation");
            }
        }

        /// <summary>
        /// Protocol Version
        /// </summary>
        /// <returns></returns>
        protected internal uint[] GetPropProtocolVersion()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PROTOCOL_VERSION);
            try
            {
                return (uint[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel Net format violation");
            }
        }

        /// <summary>
        /// Protocol Version
        /// </summary>
        /// <returns></returns>
        protected internal string GetPropNcpVersion()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NCP_VERSION);

            try
            {
                return frameData.Response.ToString();
            }
            catch
            {
                throw new SpinelProtocolExceptions("Protocol ncp version format violation");
            }
        }

        /// <summary>
        ///  NCP Network Protocol Type
        /// </summary>
        /// <returns></returns>
        protected internal SpinelProtocolType GetPropInterfaceType()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_INTERFACE_TYPE);

            try
            {
                return (SpinelProtocolType)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Interface type format violation");
            }
        }


        /// <summary>
        ///  NCP Vendor ID
        /// </summary>
        /// <returns></returns>
        protected internal uint GetPropVendorId()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_VENDOR_ID);

            try
            {
                return (uint)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Vendor id format violation");
            }
        }

        /// <summary>
        ///  NCP Capability List
        /// </summary>
        /// <returns></returns>
        protected internal Capabilities[] GetPropCaps()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_CAPS);

            try
            {
                Capabilities[] caps = (Capabilities[])frameData.Response;
                return caps;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Caps format violation");
            }
        }

        /// <summary>
        ///  NCP Interface Count
        /// </summary>
        /// <returns></returns>
        protected internal byte GetPropInterfaceCount()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_INTERFACE_COUNT);

            try
            {
                return (byte)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Interface Count format violation");
            }
        }

        /// <summary>
        ///  NCP Power State
        /// </summary>
        /// <returns></returns>
        [Obsolete("Prop Power State - Deprecated")]
        protected internal byte GetPropPowerState()
        {
            //     SPINEL_PROP_POWER_STATE  ///< PowerState [C] (deprecated, use `MCU_POWER_STATE` instead).
            throw new NotImplementedException();
        }

        /// <summary>
        ///  NCP Hardware Address
        /// </summary>
        /// <returns></returns>
        protected internal EUI64 GetPropHwaddr()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_HWADDR);

            try
            {
                return (EUI64)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("IP addesss format violation");
            }
        }

     
        protected internal byte GetPropLock()
        {
            // SPINEL_PROP_LOCK  //////< PropLock [b] (not supported)
            throw new NotImplementedException();
        }

        protected internal byte GetPropHboMemMax()
        {
            //     SPINEL_PROP_HBO_MEM_MAX   ///< Max offload mem [S] (not supported)
            throw new NotImplementedException();
        }

        protected internal byte GetPropHboBlockMax()
        {
            //     SPINEL_PROP_HBO_BLOCK_MAX  ///< Max offload block [S] (not supported)
            throw new NotImplementedException();
        }

        protected internal byte GetPropHostPowerState()
        {
            //     SPINEL_PROP_HOST_POWER_STATE  /** Format: 'C` the valid values (`SPINEL_HOST_POWER_STATE_*`):
            throw new NotImplementedException();
        }

        protected internal byte SetPropHostPowerState()
        {
            //    SPINEL_PROP_HOST_POWER_STATE
            throw new NotImplementedException();
        }

        protected internal SpinelMcuPowerState GetPropMcuPowerState()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MCU_POWER_STATE);

            try
            {
                return (SpinelMcuPowerState)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Power state format violation");
            }
        }

        protected internal bool SetPropMcuPowerState(SpinelMcuPowerState PowerState)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MCU_POWER_STATE, (byte) PowerState, "C");

            if (frameData != null && (byte)(frameData.Response) == (byte)PowerState)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region "Core Extended Properties Not Ported C Code"
        //    SPINEL_PROP_BASE_EXT__BEGIN = 0x1000,

        ///// GPIO Configuration
        ///** Format: `A(CCU)`
        // *  Type: Read-Only (Optionally Read-write using `CMD_PROP_VALUE_INSERT`)
        // *
        // * An array of structures which contain the following fields:
        // *
        // * *   `C`: GPIO Number
        // * *   `C`: GPIO Configuration Flags
        // * *   `U`: Human-readable GPIO name
        // *
        // * GPIOs which do not have a corresponding entry are not supported.
        // *
        // * The configuration parameter contains the configuration flags for the
        // * GPIO:
        // *
        // *       0   1   2   3   4   5   6   7
        // *     +---+---+---+---+---+---+---+---+
        // *     |DIR|PUP|PDN|TRIGGER|  RESERVED |
        // *     +---+---+---+---+---+---+---+---+
        // *             |O/D|
        // *             +---+
        // *
        // * *   `DIR`: Pin direction. Clear (0) for input, set (1) for output.
        // * *   `PUP`: Pull-up enabled flag.
        // * *   `PDN`/`O/D`: Flag meaning depends on pin direction:
        // *     *   Input: Pull-down enabled.
        // *     *   Output: Output is an open-drain.
        // * *   `TRIGGER`: Enumeration describing how pin changes generate
        // *     asynchronous notification commands (TBD) from the NCP to the host.
        // *     *   0: Feature disabled for this pin
        // *     *   1: Trigger on falling edge
        // *     *   2: Trigger on rising edge
        // *     *   3: Trigger on level change
        // * *   `RESERVED`: Bits reserved for future use. Always cleared to zero
        // *     and ignored when read.
        // *
        // * As an optional feature, the configuration of individual pins may be
        // * modified using the `CMD_PROP_VALUE_INSERT` command. Only the GPIO
        // * number and flags fields MUST be present, the GPIO name (if present)
        // * would be ignored. This command can only be used to modify the
        // * configuration of GPIOs which are already exposed---it cannot be used
        // * by the host to add additional GPIOs.
        // */
        //SPINEL_PROP_GPIO_CONFIG = SPINEL_PROP_BASE_EXT__BEGIN + 0,

        ///// GPIO State Bitmask
        ///** Format: `D`
        // *  Type: Read-Write
        // *
        // * Contains a bit field identifying the state of the GPIOs. The length of
        // * the data associated with these properties depends on the number of
        // * GPIOs. If you have 10 GPIOs, you'd have two bytes. GPIOs are numbered
        // * from most significant bit to least significant bit, so 0x80 is GPIO 0,
        // * 0x40 is GPIO 1, etc.
        // *
        // * For GPIOs configured as inputs:
        // *
        // * *   `CMD_PROP_VAUE_GET`: The value of the associated bit describes the
        // *     logic level read from the pin.
        // * *   `CMD_PROP_VALUE_SET`: The value of the associated bit is ignored
        // *     for these pins.
        // *
        // * For GPIOs configured as outputs:
        // *
        // * *   `CMD_PROP_VAUE_GET`: The value of the associated bit is
        // *     implementation specific.
        // * *   `CMD_PROP_VALUE_SET`: The value of the associated bit determines
        // *     the new logic level of the output. If this pin is configured as an
        // *     open-drain, setting the associated bit to 1 will cause the pin to
        // *     enter a Hi-Z state.
        // *
        // * For GPIOs which are not specified in `PROP_GPIO_CONFIG`:
        // *
        // * *   `CMD_PROP_VAUE_GET`: The value of the associated bit is
        // *     implementation specific.
        // * *   `CMD_PROP_VALUE_SET`: The value of the associated bit MUST be
        // *     ignored by the NCP.
        // *
        // * When writing, unspecified bits are assumed to be zero.
        // */
        //SPINEL_PROP_GPIO_STATE = SPINEL_PROP_BASE_EXT__BEGIN + 2,

        ///// GPIO State Set-Only Bitmask
        ///** Format: `D`
        // *  Type: Write-Only
        // *
        // * Allows for the state of various output GPIOs to be set without affecting
        // * other GPIO states. Contains a bit field identifying the output GPIOs that
        // * should have their state set to 1.
        // *
        // * When writing, unspecified bits are assumed to be zero. The value of
        // * any bits for GPIOs which are not specified in `PROP_GPIO_CONFIG` MUST
        // * be ignored.
        // */
        //SPINEL_PROP_GPIO_STATE_SET = SPINEL_PROP_BASE_EXT__BEGIN + 3,

        ///// GPIO State Clear-Only Bitmask
        ///** Format: `D`
        // *  Type: Write-Only
        // *
        // * Allows for the state of various output GPIOs to be cleared without affecting
        // * other GPIO states. Contains a bit field identifying the output GPIOs that
        // * should have their state cleared to 0.
        // *
        // * When writing, unspecified bits are assumed to be zero. The value of
        // * any bits for GPIOs which are not specified in `PROP_GPIO_CONFIG` MUST
        // * be ignored.
        // */
        //SPINEL_PROP_GPIO_STATE_CLEAR = SPINEL_PROP_BASE_EXT__BEGIN + 4,

        ///// 32-bit random number from TRNG, ready-to-use.
        //SPINEL_PROP_TRNG_32 = SPINEL_PROP_BASE_EXT__BEGIN + 5,

        ///// 16 random bytes from TRNG, ready-to-use.
        //SPINEL_PROP_TRNG_128 = SPINEL_PROP_BASE_EXT__BEGIN + 6,

        ///// Raw samples from TRNG entropy source representing 32 bits of entropy.
        //SPINEL_PROP_TRNG_RAW_32 = SPINEL_PROP_BASE_EXT__BEGIN + 7,

        ///// NCP Unsolicited update filter
        ///** Format: `A(I)`
        // *  Type: Read-Write (optional Insert-Remove)
        // *  Required capability: `CAP_UNSOL_UPDATE_FILTER`
        // *
        // * Contains a list of properties which are excluded from generating
        // * unsolicited value updates. This property is empty after reset.
        // * In other words, the host may opt-out of unsolicited property updates
        // * for a specific property by adding that property id to this list.
        // * Hosts SHOULD NOT add properties to this list which are not
        // * present in `PROP_UNSOL_UPDATE_LIST`. If such properties are added,
        // * the NCP ignores the unsupported properties.
        // *
        // */
        //SPINEL_PROP_UNSOL_UPDATE_FILTER = SPINEL_PROP_BASE_EXT__BEGIN + 8,

        ///// List of properties capable of generating unsolicited value update.
        ///** Format: `A(I)`
        // *  Type: Read-Only
        // *  Required capability: `CAP_UNSOL_UPDATE_FILTER`
        // *
        // * Contains a list of properties which are capable of generating
        // * unsolicited value updates. This list can be used when populating
        // * `PROP_UNSOL_UPDATE_FILTER` to disable all unsolicited property
        // * updates.
        // *
        // * This property is intended to effectively behave as a constant
        // * for a given NCP firmware.
        // */
        //SPINEL_PROP_UNSOL_UPDATE_LIST = SPINEL_PROP_BASE_EXT__BEGIN + 9,

        //SPINEL_PROP_BASE_EXT__END = 0x1100,
        #endregion

        //**********************************************************************
        //
        // Spinel PHY Properties
        //
        //**********************************************************************

        /// <summary>
        /// Get if the PHY is enabled
        /// </summary>
        /// <returns>.</returns>
        protected internal bool GetPhyEnabled()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_ENABLED);
            try
            {
                return (bool)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel Net format violation");
            }
        }

        /// <summary>
        /// Set to True if the PHY is enabled, set to False otherwise.
        /// </summary>
        /// <returns>.</returns>
        protected internal bool SetPhyEnabled(bool PhyEnabled)
        {
            FrameData frameData;

            if (PhyEnabled)
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_ENABLED, 1, "b");
            }
            else
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_ENABLED, 0, "b");
            }

            if (frameData != null && (bool)(frameData.Response) == PhyEnabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal byte GetPhyChan()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_CHAN);

            try
            {
                return (byte)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Channel number format violation");
            }
        }

        protected internal bool SetPhyChan(byte channel)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_CHAN, channel, "C");

            if (frameData != null && ((byte)frameData.Response == channel))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal byte[] GetPhyChanSupported()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_CHAN_SUPPORTED);

            try
            {
                return (byte[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Phy Chan format violation");
            }
        }

        protected internal uint GetPhyFreq()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_FREQ);

            try
            {
                return (uint)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Phy Freq format violation");
            }
        }

        protected internal sbyte GetPhyCcaThreshold()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_CCA_THRESHOLD);

            try
            {
                return (sbyte)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Phy Cca Threshold format violation");
            }
        }

        protected internal bool SetPhyCcaThreshold(sbyte CcaThreshold)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_CCA_THRESHOLD, CcaThreshold, "c");

            if (frameData != null && ((sbyte)frameData.Response == CcaThreshold))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal sbyte GetPhyTxPower()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_TX_POWER);

            try
            {
                return (sbyte)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("TX POWER format violation");
            }
        }

        protected internal bool SetPhyTxPower(sbyte TxPower)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_TX_POWER, TxPower, "c");

            if (frameData != null && ((sbyte)frameData.Response == TxPower))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal sbyte GetPhyRssi()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_RSSI);

            try
            {
                return (sbyte)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Phy format violation");
            }
        }

        protected internal sbyte GetPhyRxSensitivity()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_RX_SENSITIVITY);

            try
            {
                return (sbyte)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Phy format violation");
            }
        }

      
        protected internal bool GetPcapEnabled()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_PCAP_ENABLED);
            try
            {
                return (bool)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Phy format violation");
            }
        }

        protected internal bool SetPcapEnabled(bool PcapEnabled)
        {
            FrameData frameData;

            if (PcapEnabled)
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_PCAP_ENABLED, 1, "b");
            }
            else
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_PCAP_ENABLED, 0, "b");
            }

            if (frameData != null && (bool)(frameData.Response) == PcapEnabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal bool SetChanPreferred(byte[] ChanPreferred)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_CHAN_PREFERRED, ChanPreferred, "C");

            if (frameData != null && Utilities.ByteArrayCompare((byte[])frameData.Response, ChanPreferred))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal sbyte GetPhyFemLnaGain()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_FEM_LNA_GAIN);

            try
            {
                return (sbyte)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Phy format violation");
            }
        }

        protected internal bool SetPhyFemLnaGain(sbyte LnaGain)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_FEM_LNA_GAIN, LnaGain, "c");

            if (frameData != null && ((sbyte)frameData.Response == LnaGain))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal bool SetPhyChanMaxPower(byte Channel, sbyte MaxPower)
        {
            byte[] ChanMaxPower = new byte[2];

            ChanMaxPower[0] = Channel;
            ChanMaxPower[1] = (byte) MaxPower;

            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_CHAN_MAX_POWER, ChanMaxPower, "Cc");

            if (frameData != null && Utilities.ByteArrayCompare((byte[])frameData.Response, ChanMaxPower))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal ushort GetPhyRegionCode()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_PHY_REGION_CODE);

            try
            {
                return (ushort)(frameData.Response);
            }
            catch
            {
                throw new SpinelProtocolExceptions("Pan id format violation");
            }
        }

        protected internal bool SetPhyRegionCode(ushort RegionCode)
        {           
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_PHY_REGION_CODE, RegionCode, "S");

            if (frameData != null && (ushort)(frameData.Response) == RegionCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region "Phy Extended Properties Not Ported C Code"
        //    SPINEL_PROP_PHY_EXT__BEGIN = 0x1200,

        ///// Signal Jamming Detection Enable
        ///** Format: `b`
        // *
        // * Indicates if jamming detection is enabled or disabled. Set to true
        // * to enable jamming detection.
        // */
        //SPINEL_PROP_JAM_DETECT_ENABLE = SPINEL_PROP_PHY_EXT__BEGIN + 0,

        ///// Signal Jamming Detected Indicator
        ///** Format: `b` (Read-Only)
        // *
        // * Set to true if radio jamming is detected. Set to false otherwise.
        // *
        // * When jamming detection is enabled, changes to the value of this
        // * property are emitted asynchronously via `CMD_PROP_VALUE_IS`.
        // */
        //SPINEL_PROP_JAM_DETECTED = SPINEL_PROP_PHY_EXT__BEGIN + 1,

        ///// Jamming detection RSSI threshold
        ///** Format: `c`
        // *  Units: dBm
        // *
        // * This parameter describes the threshold RSSI level (measured in
        // * dBm) above which the jamming detection will consider the
        // * channel blocked.
        // */
        //SPINEL_PROP_JAM_DETECT_RSSI_THRESHOLD = SPINEL_PROP_PHY_EXT__BEGIN + 2,

        ///// Jamming detection window size
        ///** Format: `C`
        // *  Units: Seconds (1-63)
        // *
        // * This parameter describes the window period for signal jamming
        // * detection.
        // */
        //SPINEL_PROP_JAM_DETECT_WINDOW = SPINEL_PROP_PHY_EXT__BEGIN + 3,

        ///// Jamming detection busy period
        ///** Format: `C`
        // *  Units: Seconds (1-63)
        // *
        // * This parameter describes the number of aggregate seconds within
        // * the detection window where the RSSI must be above
        // * `PROP_JAM_DETECT_RSSI_THRESHOLD` to trigger detection.
        // *
        // * The behavior of the jamming detection feature when `PROP_JAM_DETECT_BUSY`
        // * is larger than `PROP_JAM_DETECT_WINDOW` is undefined.
        // */
        //SPINEL_PROP_JAM_DETECT_BUSY = SPINEL_PROP_PHY_EXT__BEGIN + 4,

        ///// Jamming detection history bitmap (for debugging)
        ///** Format: `X` (read-only)
        // *
        // * This value provides information about current state of jamming detection
        // * module for monitoring/debugging purpose. It returns a 64-bit value where
        // * each bit corresponds to one second interval starting with bit 0 for the
        // * most recent interval and bit 63 for the oldest intervals (63 sec earlier).
        // * The bit is set to 1 if the jamming detection module observed/detected
        // * high signal level during the corresponding one second interval.
        // *
        // */
        //SPINEL_PROP_JAM_DETECT_HISTORY_BITMAP = SPINEL_PROP_PHY_EXT__BEGIN + 5,

        ///// Channel monitoring sample interval
        ///** Format: `L` (read-only)
        // *  Units: Milliseconds
        // *
        // * Required capability: SPINEL_CAP_CHANNEL_MONITOR
        // *
        // * If channel monitoring is enabled and active, every sample interval, a
        // * zero-duration Energy Scan is performed, collecting a single RSSI sample
        // * per channel. The RSSI samples are compared with a pre-specified RSSI
        // * threshold.
        // *
        // */
        //SPINEL_PROP_CHANNEL_MONITOR_SAMPLE_INTERVAL = SPINEL_PROP_PHY_EXT__BEGIN + 6,

        ///// Channel monitoring RSSI threshold
        ///** Format: `c` (read-only)
        // *  Units: dBm
        // *
        // * Required capability: SPINEL_CAP_CHANNEL_MONITOR
        // *
        // * This value specifies the threshold used by channel monitoring module.
        // * Channel monitoring maintains the average rate of RSSI samples that
        // * are above the threshold within (approximately) a pre-specified number
        // * of samples (sample window).
        // *
        // */
        //SPINEL_PROP_CHANNEL_MONITOR_RSSI_THRESHOLD = SPINEL_PROP_PHY_EXT__BEGIN + 7,

        ///// Channel monitoring sample window
        ///** Format: `L` (read-only)
        // *  Units: Number of samples
        // *
        // * Required capability: SPINEL_CAP_CHANNEL_MONITOR
        // *
        // * The averaging sample window length (in units of number of channel
        // * samples) used by channel monitoring module. Channel monitoring will
        // * sample all channels every sample interval. It maintains the average rate
        // * of RSSI samples that are above the RSSI threshold within (approximately)
        // * the sample window.
        // *
        // */
        //SPINEL_PROP_CHANNEL_MONITOR_SAMPLE_WINDOW = SPINEL_PROP_PHY_EXT__BEGIN + 8,

        ///// Channel monitoring sample count
        ///** Format: `L` (read-only)
        // *  Units: Number of samples
        // *
        // * Required capability: SPINEL_CAP_CHANNEL_MONITOR
        // *
        // * Total number of RSSI samples (per channel) taken by the channel
        // * monitoring module since its start (since Thread network interface
        // * was enabled).
        // *
        // */
        //SPINEL_PROP_CHANNEL_MONITOR_SAMPLE_COUNT = SPINEL_PROP_PHY_EXT__BEGIN + 9,

        ///// Channel monitoring channel occupancy
        ///** Format: `A(t(CU))` (read-only)
        // *
        // * Required capability: SPINEL_CAP_CHANNEL_MONITOR
        // *
        // * Data per item is:
        // *
        // *  `C`: Channel
        // *  `U`: Channel occupancy indicator
        // *
        // * The channel occupancy value represents the average rate/percentage of
        // * RSSI samples that were above RSSI threshold ("bad" RSSI samples) within
        // * (approximately) sample window latest RSSI samples.
        // *
        // * Max value of `0xffff` indicates all RSSI samples were above RSSI
        // * threshold (i.e. 100% of samples were "bad").
        // *
        // */
        //SPINEL_PROP_CHANNEL_MONITOR_CHANNEL_OCCUPANCY = SPINEL_PROP_PHY_EXT__BEGIN + 10,

        ///// Radio caps
        ///** Format: `i` (read-only)
        // *
        // * Data per item is:
        // *
        // *  `i`: Radio Capabilities.
        // *
        // */
        //SPINEL_PROP_RADIO_CAPS = SPINEL_PROP_PHY_EXT__BEGIN + 11,

        ///// All coex metrics related counters.
        ///** Format: t(LLLLLLLL)t(LLLLLLLLL)bL  (Read-only)
        // *
        // * Required capability: SPINEL_CAP_RADIO_COEX
        // *
        // * The contents include two structures and two common variables, first structure corresponds to
        // * all transmit related coex counters, second structure provides the receive related counters.
        // *
        // * The transmit structure includes:
        // *   'L': NumTxRequest                       (The number of tx requests).
        // *   'L': NumTxGrantImmediate                (The number of tx requests while grant was active).
        // *   'L': NumTxGrantWait                     (The number of tx requests while grant was inactive).
        // *   'L': NumTxGrantWaitActivated            (The number of tx requests while grant was inactive that were
        // *                                            ultimately granted).
        // *   'L': NumTxGrantWaitTimeout              (The number of tx requests while grant was inactive that timed out).
        // *   'L': NumTxGrantDeactivatedDuringRequest (The number of tx requests that were in progress when grant was
        // *                                            deactivated).
        // *   'L': NumTxDelayedGrant                  (The number of tx requests that were not granted within 50us).
        // *   'L': AvgTxRequestToGrantTime            (The average time in usec from tx request to grant).
        // *
        // * The receive structure includes:
        // *   'L': NumRxRequest                       (The number of rx requests).
        // *   'L': NumRxGrantImmediate                (The number of rx requests while grant was active).
        // *   'L': NumRxGrantWait                     (The number of rx requests while grant was inactive).
        // *   'L': NumRxGrantWaitActivated            (The number of rx requests while grant was inactive that were
        // *                                            ultimately granted).
        // *   'L': NumRxGrantWaitTimeout              (The number of rx requests while grant was inactive that timed out).
        // *   'L': NumRxGrantDeactivatedDuringRequest (The number of rx requests that were in progress when grant was
        // *                                            deactivated).
        // *   'L': NumRxDelayedGrant                  (The number of rx requests that were not granted within 50us).
        // *   'L': AvgRxRequestToGrantTime            (The average time in usec from rx request to grant).
        // *   'L': NumRxGrantNone                     (The number of rx requests that completed without receiving grant).
        // *
        // * Two common variables:
        // *   'b': Stopped        (Stats collection stopped due to saturation).
        // *   'L': NumGrantGlitch (The number of of grant glitches).
        // */
        //SPINEL_PROP_RADIO_COEX_METRICS = SPINEL_PROP_PHY_EXT__BEGIN + 12,

        ///// Radio Coex Enable
        ///** Format: `b`
        // *
        // * Required capability: SPINEL_CAP_RADIO_COEX
        // *
        // * Indicates if radio coex is enabled or disabled. Set to true to enable radio coex.
        // */
        //SPINEL_PROP_RADIO_COEX_ENABLE = SPINEL_PROP_PHY_EXT__BEGIN + 13,

        //SPINEL_PROP_PHY_EXT__END = 0x1300,

        #endregion

        //**********************************************************************
        //
        // Spinel MAC Properties
        //
        //**********************************************************************

        protected internal void SetMacScanState(SpinelScanState ScanState)
        {
            PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_SCAN_STATE, (byte)ScanState, "C");
        }

        protected internal byte[] GetMacScanMask()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MAC_SCAN_MASK);

            try
            {
                return (byte[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Channels mask format violation");
            }
        }

        protected internal bool SetMacScanMask(byte[] channels)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_SCAN_MASK, channels, "D");

            if (frameData != null && Utilities.ByteArrayCompare((byte[])frameData.Response, channels))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal ushort GetMacScanPeriod()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MAC_SCAN_PERIOD);

            try
            {
                return (ushort)(frameData.Response);
            }
            catch
            {
                throw new SpinelProtocolExceptions("Pan id format violation");
            }
        }

        protected internal bool SetMacScanPeriod(ushort ScanPeriod)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_SCAN_PERIOD, ScanPeriod, "S");

            if (frameData != null && (ushort)(frameData.Response) == ScanPeriod)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //SPINEL_PROP_MAC_SCAN_BEACON = SPINEL_PROP_MAC__BEGIN + 3, In async packet handler

        protected internal EUI64 GetMac_15_4_Laddr()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MAC_15_4_LADDR);

            try
            {
                return (EUI64)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Addesss format violation");
            }
        }

        protected internal bool SetMac_15_4_Laddr(EUI64 LongAddr)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_15_4_LADDR, LongAddr.bytes, "D");

            if (frameData != null && Utilities.ByteArrayCompare((byte[])frameData.Response, LongAddr.bytes))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal EUI64 GetMac_15_4_Saddr()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MAC_15_4_SADDR);

            try
            {
                return (EUI64)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Addesss format violation");
            }
        }

        protected internal bool SetMac_15_4_Saddr(ushort ShortAddr)
        {
           
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_15_4_SADDR, ShortAddr, "S");

            if (frameData != null && (ushort)(frameData.Response) == ShortAddr)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal ushort GetMac_15_4_PanId()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MAC_15_4_PANID);

            try
            {
                return (ushort)(frameData.Response);
            }
            catch
            {
                throw new SpinelProtocolExceptions("Pan id format violation");
            }
        }

        protected internal bool SetMac_15_4_PanId(ushort panId)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_15_4_PANID, panId, "S");

            if (frameData != null && (ushort)(frameData.Response) == panId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

     
        protected internal bool GetMacRawStreamEnabled()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MAC_RAW_STREAM_ENABLED);
            try
            {
                return (bool)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Mac format violation");
            }
        }

     
        protected internal bool SetMacRawStreamEnabled(bool StreamEnabled)
        {
            FrameData frameData;

            if (StreamEnabled)
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_RAW_STREAM_ENABLED, 1, "b");
            }
            else
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_MAC_RAW_STREAM_ENABLED, 0, "b");
            }

            if (frameData != null && (bool)(frameData.Response) == StreamEnabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal byte GetMacPromiscuousMode()
        {
            //SPINEL_PROP_THREAD_MODE  Format `b` - Read-write
            throw new NotImplementedException();
        }

        protected internal bool SetMacPromiscuousMode(byte PromiscuousMode)
        {
            //SPINEL_PROP_THREAD_MODE  Format `b` - Read-write
            throw new NotImplementedException();
        }

        //  SPINEL_PROP_MAC_ENERGY_SCAN_RESULT = SPINEL_PROP_MAC__BEGIN + 9,  /** Format: `Cc` - Asynchronous event only In async packet handler

        protected internal byte GetMacDataPollPeriod()
        {
            //SPINEL_PROP_THREAD_MODE  Format `b` - Read-write
            throw new NotImplementedException();
        }

        protected internal bool SetMacDataPollPeriod(uint PollPeriod)
        {
            
            //SPINEL_PROP_THREAD_MODE  Format `b` - Read-write
            throw new NotImplementedException();
        }

        #region "Mac Extended Properties Not Ported C Code"
    //    SPINEL_PROP_MAC_EXT__BEGIN = 0x1300,

    ///// MAC Allowlist
    ///** Format: `A(t(Ec))`
    // * Required capability: `CAP_MAC_ALLOWLIST`
    // *
    // * Structure Parameters:
    // *
    // *  `E`: EUI64 address of node
    // *  `c`: Optional RSSI-override value. The value 127 indicates
    // *       that the RSSI-override feature is not enabled for this
    // *       address. If this value is omitted when setting or
    // *       inserting, it is assumed to be 127. This parameter is
    // *       ignored when removing.
    // */
    //SPINEL_PROP_MAC_ALLOWLIST = SPINEL_PROP_MAC_EXT__BEGIN + 0,

    ///// MAC Allowlist Enabled Flag
    ///** Format: `b`
    // * Required capability: `CAP_MAC_ALLOWLIST`
    // *
    // */
    //SPINEL_PROP_MAC_ALLOWLIST_ENABLED = SPINEL_PROP_MAC_EXT__BEGIN + 1,

    ///// MAC Extended Address
    ///** Format: `E`
    // *
    // *  Specified by Thread. Randomly-chosen, but non-volatile EUI-64.
    // */
    //SPINEL_PROP_MAC_EXTENDED_ADDR = SPINEL_PROP_MAC_EXT__BEGIN + 2,

    ///// MAC Source Match Enabled Flag
    ///** Format: `b`
    // * Required Capability: SPINEL_CAP_MAC_RAW or SPINEL_CAP_CONFIG_RADIO
    // *
    // * Set to true to enable radio source matching or false to disable it.
    // * The source match functionality is used by radios when generating
    // * ACKs. The short and extended address lists are used for setting
    // * the Frame Pending bit in the ACKs.
    // *
    // */
    //SPINEL_PROP_MAC_SRC_MATCH_ENABLED = SPINEL_PROP_MAC_EXT__BEGIN + 3,

    ///// MAC Source Match Short Address List
    ///** Format: `A(S)`
    // * Required Capability: SPINEL_CAP_MAC_RAW or SPINEL_CAP_CONFIG_RADIO
    // *
    // */
    //SPINEL_PROP_MAC_SRC_MATCH_SHORT_ADDRESSES = SPINEL_PROP_MAC_EXT__BEGIN + 4,

    ///// MAC Source Match Extended Address List
    ///** Format: `A(E)`
    // *  Required Capability: SPINEL_CAP_MAC_RAW or SPINEL_CAP_CONFIG_RADIO
    // *
    // */
    //SPINEL_PROP_MAC_SRC_MATCH_EXTENDED_ADDRESSES = SPINEL_PROP_MAC_EXT__BEGIN + 5,

    ///// MAC Denylist
    ///** Format: `A(t(E))`
    // * Required capability: `CAP_MAC_ALLOWLIST`
    // *
    // * Structure Parameters:
    // *
    // *  `E`: EUI64 address of node
    // *
    // */
    //SPINEL_PROP_MAC_DENYLIST = SPINEL_PROP_MAC_EXT__BEGIN + 6,

    ///// MAC Denylist Enabled Flag
    ///** Format: `b`
    // *  Required capability: `CAP_MAC_ALLOWLIST`
    // */
    //SPINEL_PROP_MAC_DENYLIST_ENABLED = SPINEL_PROP_MAC_EXT__BEGIN + 7,

    ///// MAC Received Signal Strength Filter
    ///** Format: `A(t(Ec))`
    // * Required capability: `CAP_MAC_ALLOWLIST`
    // *
    // * Structure Parameters:
    // *
    // * * `E`: Optional EUI64 address of node. Set default RSS if not included.
    // * * `c`: Fixed RSS. 127 means not set.
    // */
    //SPINEL_PROP_MAC_FIXED_RSS = SPINEL_PROP_MAC_EXT__BEGIN + 8,

    ///// The CCA failure rate
    ///** Format: `S`
    // *
    // * This property provides the current CCA (Clear Channel Assessment) failure rate.
    // *
    // * Maximum value `0xffff` corresponding to 100% failure rate.
    // *
    // */
    //SPINEL_PROP_MAC_CCA_FAILURE_RATE = SPINEL_PROP_MAC_EXT__BEGIN + 9,

    ///// MAC Max direct retry number
    ///** Format: `C`
    // *
    // * The maximum (user-specified) number of direct frame transmission retries.
    // *
    // */
    //SPINEL_PROP_MAC_MAX_RETRY_NUMBER_DIRECT = SPINEL_PROP_MAC_EXT__BEGIN + 10,

    ///// MAC Max indirect retry number
    ///** Format: `C`
    // * Required capability: `SPINEL_CAP_CONFIG_FTD`
    // *
    // * The maximum (user-specified) number of indirect frame transmission retries.
    // *
    // */
    //SPINEL_PROP_MAC_MAX_RETRY_NUMBER_INDIRECT = SPINEL_PROP_MAC_EXT__BEGIN + 11,

    //SPINEL_PROP_MAC_EXT__END = 0x1400,
        #endregion


        //**********************************************************************
        //
        // Spinel NET Properties
        //
        //**********************************************************************

        /// <summary>
        /// Network Is Saved (Is Commissioned)
        /// </summary>
        /// <returns>true if there is a network state stored/saved.</returns>
        protected internal bool GetNetSaved()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_SAVED);
            try
            {
                return (bool)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel Net format violation");
            }
        }

        /// <summary>
        /// Network Interface Status
        /// </summary>
        /// <returns>Returns true if interface up and false if interface down</returns>
        protected internal bool GetNetIfUp()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_IF_UP);
            try
            {
                return (bool)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel Net format violation");
            }
        }

        /// <summary>
        /// Network interface up/down status. Write true to bring interface up and false to bring interface down.     
        /// </summary>
        /// <param name="NetworkInterfaceStatus"></param>
        /// <returns></returns>
        protected internal bool SetNetIfUp(bool NetworkInterfaceStatus)
        {
            FrameData frameData;

            if (NetworkInterfaceStatus)
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_IF_UP, 1, "b");
            }
            else
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_IF_UP, 0, "b");
            }

            if (frameData != null && (bool)(frameData.Response) == NetworkInterfaceStatus)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal bool GetNetStackUp()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_STACK_UP);
            try
            {
                return (bool)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel stack up format violation");
            }
        }

        protected internal bool SetNetStackUp(bool ThreadStackStatus)
        {
            FrameData frameData;

            if (ThreadStackStatus)
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_STACK_UP, 1, "b");
            }
            else
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_STACK_UP, 0, "b");
            }

            if (frameData != null && (bool)(frameData.Response) == ThreadStackStatus)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal SpinelNetRole GetNetRole()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_ROLE);

            try
            {
                return (SpinelNetRole)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Role id format violation");
            }
        }

        protected internal bool SetNetRole(SpinelNetRole ThreadDeviceRole)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_ROLE, ((byte)ThreadDeviceRole), "C");

            if (frameData != null && (SpinelNetRole)(frameData.Response) == ThreadDeviceRole)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal string GetNetNetworkName()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_NETWORK_NAME);

            try
            {
                return frameData.Response.ToString();
            }
            catch
            {
                throw new SpinelProtocolExceptions("Network name format violation");
            }
        }

        protected internal bool SetNetNetworkName(string ThreadNetworkName)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_NETWORK_NAME, ThreadNetworkName, "U");

            if (frameData != null && frameData.Response.ToString() == ThreadNetworkName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal byte[] GetNetXpanId()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_XPANID);

            try
            {
                return (byte[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("XPan id format violation");
            }
        }

        protected internal bool SetNetXpanId(byte[] ThreadNetworkExtendedPANId)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_XPANID, ThreadNetworkExtendedPANId, "D");

            if (frameData != null && Utilities.ByteArrayCompare((byte[])frameData.Response, ThreadNetworkExtendedPANId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal byte[] GetNetNetworkKey()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_NETWORK_KEY);

            try
            {
                return (byte[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel network key format violation.");
            }
        }

        protected internal bool SetNetNetworkKey(byte[] ThreadNetworkKey)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_NETWORK_KEY, ThreadNetworkKey, "D");

            if (frameData != null && Utilities.ByteArrayCompare((byte[])frameData.Response, ThreadNetworkKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal uint GetNetKeySequenceCounter()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_KEY_SEQUENCE_COUNTER);

            try
            {
                return (uint)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel Key Sequence Counter format violation.");
            }
        }

        protected internal bool SetNetKeySequenceCounter(uint ThreadNetworkKeySequenceCounter)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_KEY_SEQUENCE_COUNTER, ThreadNetworkKeySequenceCounter, "L");

            if (frameData != null && ((uint)frameData.Response == ThreadNetworkKeySequenceCounter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal uint GetNetPartitionId()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_PARTITION_ID);

            try
            {
                return (uint)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel Partition Id format violation.");
            }
        }

        protected internal bool SetNetPartitionId(uint ThreadNetworkPartitionId)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_PARTITION_ID, ThreadNetworkPartitionId, "L");

            if (frameData != null && ((uint)frameData.Response == ThreadNetworkPartitionId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal bool GetNetRequireJoinExisting()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING);
            try
            {
                return (bool)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Net format violation");
            }
        }

        protected internal bool SetNetRequireJoinExisting(bool RequireJoinExisting)
        {
            FrameData frameData;

            if (RequireJoinExisting)
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING, 1, "b");
            }
            else
            {
                frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_REQUIRE_JOIN_EXISTING, 0, "b");
            }

            if (frameData != null && (bool)(frameData.Response) == RequireJoinExisting)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected internal uint GetNetKeySwitchGuardtime()
        {
            //     SPINEL_PROP_NET_KEY_SWITCH_GUARDTIME
            throw new NotImplementedException();
        }

        protected internal bool SetNetKeySwitchGuardtime(uint ThreadNetworkKeySwitchGuardTime)
        {
            //    SPINEL_PROP_NET_KEY_SWITCH_GUARDTIME
            throw new NotImplementedException();
        }

        protected internal byte[] GetNetNetworkPSKC()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_NET_PSKC);

            try
            {
                return (byte[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Spinel network pskc format violation.");
            }
        }

        protected internal bool SetNetNetworkPSKC(byte[] ThreadNetworkPSKc)
        {
            FrameData frameData = PropertySetValue(SpinelProperties.SPINEL_PROP_NET_PSKC, ThreadNetworkPSKc, "D");

            if (frameData != null && Utilities.ByteArrayCompare((byte[])frameData.Response, ThreadNetworkPSKc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //**********************************************************************
        //
        //      Spinel Thread Properties
        //
        //**********************************************************************

        protected internal IPv6Address[] GetThreadLeaderAddr()
        {
            //   SPINEL_PROP_THREAD_LEADER_ADDR
            throw new NotImplementedException();
        }

        protected internal OpenThreadRouterInfo GetThreadLeaderParent()
        {
            //   SPINEL_PROP_THREAD_PARENT  /** Format: `ESLccCC` - Read only
            throw new NotImplementedException();
        }

        protected internal OpenThreadChildInfo[] GetThreadChildTable()
        {
            //SPINEL_PROP_THREAD_CHILD_TABLE Format: [A(t(ESLLCCcCc)] - Read only         
            throw new NotImplementedException();
        }

        protected internal byte GetThreadLeaderRid()
        {
            //SPINEL_PROP_THREAD_LEADER_RID Format `C` - Read only  
            throw new NotImplementedException();
        }

        protected internal byte GetThreadLeaderWeight()
        {
            // SPINEL_PROP_THREAD_LEADER_WEIGHT Format `C` - Read only  
            throw new NotImplementedException();
        }

        protected internal byte GetThreadLocalLeaderWeight()
        {
            // SPINEL_PROP_THREAD_LOCAL_LEADER_WEIGHT Format `C` - Read only  
            throw new NotImplementedException();
        }

        protected internal byte[] GetThreadNetworkData()
        {
            // SPINEL_PROP_THREAD_NETWORK_DATA   /** Format `D` - Read only
            throw new NotImplementedException();
        }

        protected internal byte GetThreadNetworkDataVersion()
        {
            // SPINEL_PROP_THREAD_NETWORK_DATA_VERSION Format `C` - Read only  
            throw new NotImplementedException();
        }

        protected internal byte[] GetThreadStableNetworkData()
        {
            // SPINEL_PROP_THREAD_STABLE_NETWORK_DATA   /** Format `D` - Read only
            throw new NotImplementedException();
        }

        protected internal byte GetThreadStableNetworkDataVersion()
        {
            // SPINEL_PROP_THREAD_STABLE_NETWORK_DATA_VERSION Format `C` - Read only  
            throw new NotImplementedException();
        }

        protected internal OpenThreadBorderRouterConfig[] GetThreadOnMeshNets()
        {
            // SPINEL_PROP_THREAD_ON_MESH_NETS Format: `A(t(6CbCbSC))`
            throw new NotImplementedException();
        }

        protected internal OpenThreadExternalRouteConfig[] GetThreadOffMeshRoutes()
        {
            // SPINEL_PROP_THREAD_OFF_MESH_ROUTES Format: `A(t(6CbCbSC))`
            throw new NotImplementedException();
        }

        protected internal ushort[] GetThreadAssistingPorts()
        {
            //  //SPINEL_PROP_THREAD_ASSISTING_PORTS Format `A(S)`
            throw new NotImplementedException();
        }

        protected internal bool SetThreadAssistingPorts(ushort[] AssistingPort)
        {
            //  //SPINEL_PROP_THREAD_ASSISTING_PORTS Format `A(S)`
            throw new NotImplementedException();
        }

        protected internal bool GetThreadAllowLocalNetDataChange()
        {
            //SPINEL_PROP_THREAD_ALLOW_LOCAL_NET_DATA_CHANGE Format `b` - Read-write
            throw new NotImplementedException();
        }

        protected internal bool SetThreadAllowLocalNetDataChange(bool AllowLocalNetworkDataChange)
        {
            //SPINEL_PROP_THREAD_ALLOW_LOCAL_NET_DATA_CHANGE Format `b` - Read-write
            throw new NotImplementedException();
        }

        protected internal byte GetThreadThreadMode()
        {
            //SPINEL_PROP_THREAD_MODE  Format `b` - Read-write
            throw new NotImplementedException();
        }

        protected internal bool SetThreadThreadMode(byte ThreadMode)
        {
            //SPINEL_PROP_THREAD_MODE  Format `b` - Read-write
            throw new NotImplementedException();
        }


        //**********************************************************************
        //
        //      Spinel Thread Extended Properties
        //
        //**********************************************************************



        protected internal OpenThreadNeighborInfo[] GetThreadNeighborTable()
        {
            //SPINEL_PROP_THREAD_NEIGHBOR_TABLE  Format: `A(t(ESLCcCbLLc))` - Read only
            throw new NotImplementedException();
        }

        protected internal OpenThreadRouterInfo[] GetThreadRouterTable()
        {
            //SPINEL_PROP_THREAD_ROUTER_TABLE   Format: `A(t(ESCCCCCCb)` - Read only
            throw new NotImplementedException();
        }

        protected internal OpenThreadChildInfo[] GetThreadChildTableAddresses()
        {
            //SPINEL_PROP_THREAD_CHILD_TABLE_ADDRESSES   Format: `A(t(ESA(6)))` - Read only
            throw new NotImplementedException();
        }

        #region "Thread Extended Properties Not Ported C Code"
        //


        //    SPINEL_PROP_THREAD_EXT__BEGIN = 0x1500,

        ///// Thread Child Timeout
        ///** Format: `L`
        // *  Unit: Seconds
        // *
        // *  Used when operating in the Child role.
        // */
        //SPINEL_PROP_THREAD_CHILD_TIMEOUT = SPINEL_PROP_THREAD_EXT__BEGIN + 0,

        ///// Thread RLOC16
        ///** Format: `S`
        // *
        // */
        //SPINEL_PROP_THREAD_RLOC16 = SPINEL_PROP_THREAD_EXT__BEGIN + 1,

        ///// Thread Router Upgrade Threshold
        ///** Format: `C`
        // *
        // */
        //SPINEL_PROP_THREAD_ROUTER_UPGRADE_THRESHOLD = SPINEL_PROP_THREAD_EXT__BEGIN + 2,

        ///// Thread Context Reuse Delay
        ///** Format: `L`
        // *
        // */
        //SPINEL_PROP_THREAD_CONTEXT_REUSE_DELAY = SPINEL_PROP_THREAD_EXT__BEGIN + 3,

        ///// Thread Network ID Timeout
        ///** Format: `C`
        // *
        // */
        //SPINEL_PROP_THREAD_NETWORK_ID_TIMEOUT = SPINEL_PROP_THREAD_EXT__BEGIN + 4,

        ///// List of active thread router ids
        ///** Format: `A(C)`
        // *
        // * Note that some implementations may not support CMD_GET_VALUE
        // * router ids, but may support CMD_REMOVE_VALUE when the node is
        // * a leader.
        // *
        // */
        //SPINEL_PROP_THREAD_ACTIVE_ROUTER_IDS = SPINEL_PROP_THREAD_EXT__BEGIN + 5,

        ///// Forward IPv6 packets that use RLOC16 addresses to HOST.
        ///** Format: `b`
        // *
        // * Allow host to directly observe all IPv6 packets received by the NCP,
        // * including ones sent to the RLOC16 address.
        // *
        // * Default is false.
        // *
        // */
        //SPINEL_PROP_THREAD_RLOC16_DEBUG_PASSTHRU = SPINEL_PROP_THREAD_EXT__BEGIN + 6,

        ///// Router Role Enabled
        ///** Format `b`
        // *
        // * Allows host to indicate whether or not the router role is enabled.
        // * If current role is a router, setting this property to `false` starts
        // * a re-attach process as an end-device.
        // *
        // */
        //SPINEL_PROP_THREAD_ROUTER_ROLE_ENABLED = SPINEL_PROP_THREAD_EXT__BEGIN + 7,

        ///// Thread Router Downgrade Threshold
        ///** Format: `C`
        // *
        // */
        //SPINEL_PROP_THREAD_ROUTER_DOWNGRADE_THRESHOLD = SPINEL_PROP_THREAD_EXT__BEGIN + 8,

        ///// Thread Router Selection Jitter
        ///** Format: `C`
        // *
        // */
        //SPINEL_PROP_THREAD_ROUTER_SELECTION_JITTER = SPINEL_PROP_THREAD_EXT__BEGIN + 9,

        ///// Thread Preferred Router Id
        ///** Format: `C` - Write only
        // *
        // * Specifies the preferred Router Id. Upon becoming a router/leader the node
        // * attempts to use this Router Id. If the preferred Router Id is not set or
        // * if it can not be used, a randomly generated router id is picked. This
        // * property can be set only when the device role is either detached or
        // * disabled.
        // *
        // */
        //SPINEL_PROP_THREAD_PREFERRED_ROUTER_ID = SPINEL_PROP_THREAD_EXT__BEGIN + 10,



        ///// Thread Max Child Count
        ///** Format: `C`
        // *
        // * Specifies the maximum number of children currently allowed.
        // * This parameter can only be set when Thread protocol operation
        // * has been stopped.
        // *
        // */
        //SPINEL_PROP_THREAD_CHILD_COUNT_MAX = SPINEL_PROP_THREAD_EXT__BEGIN + 12,

        ///// Leader Network Data
        ///** Format: `D` - Read only
        // *
        // */
        //SPINEL_PROP_THREAD_LEADER_NETWORK_DATA = SPINEL_PROP_THREAD_EXT__BEGIN + 13,

        ///// Stable Leader Network Data
        ///** Format: `D` - Read only
        // *
        // */
        //SPINEL_PROP_THREAD_STABLE_LEADER_NETWORK_DATA = SPINEL_PROP_THREAD_EXT__BEGIN + 14,

        ///// Thread Joiner Data
        ///** Format `A(T(ULE))`
        // *  PSKd, joiner timeout, eui64 (optional)
        // *
        // * This property is being deprecated by SPINEL_PROP_MESHCOP_COMMISSIONER_JOINERS.
        // *
        // */
        //SPINEL_PROP_THREAD_JOINERS = SPINEL_PROP_THREAD_EXT__BEGIN + 15,

        ///// Thread Commissioner Enable
        ///** Format `b`
        // *
        // * Default value is `false`.
        // *
        // * This property is being deprecated by SPINEL_PROP_MESHCOP_COMMISSIONER_STATE.
        // *
        // */
        //SPINEL_PROP_THREAD_COMMISSIONER_ENABLED = SPINEL_PROP_THREAD_EXT__BEGIN + 16,

        ///// Thread TMF proxy enable
        ///** Format `b`
        // * Required capability: `SPINEL_CAP_THREAD_TMF_PROXY`
        // *
        // * This property is deprecated.
        // *
        // */
        //SPINEL_PROP_THREAD_TMF_PROXY_ENABLED = SPINEL_PROP_THREAD_EXT__BEGIN + 17,

        ///// Thread TMF proxy stream
        ///** Format `dSS`
        // * Required capability: `SPINEL_CAP_THREAD_TMF_PROXY`
        // *
        // * This property is deprecated. Please see `SPINEL_PROP_THREAD_UDP_FORWARD_STREAM`.
        // *
        // */
        //SPINEL_PROP_THREAD_TMF_PROXY_STREAM = SPINEL_PROP_THREAD_EXT__BEGIN + 18,

        ///// Thread "joiner" flag used during discovery scan operation
        ///** Format `b`
        // *
        // * This property defines the Joiner Flag value in the Discovery Request TLV.
        // *
        // * Default value is `false`.
        // *
        // */
        //SPINEL_PROP_THREAD_DISCOVERY_SCAN_JOINER_FLAG = SPINEL_PROP_THREAD_EXT__BEGIN + 19,

        ///// Enable EUI64 filtering for discovery scan operation.
        ///** Format `b`
        // *
        // * Default value is `false`
        // *
        // */
        //SPINEL_PROP_THREAD_DISCOVERY_SCAN_ENABLE_FILTERING = SPINEL_PROP_THREAD_EXT__BEGIN + 20,

        ///// PANID used for Discovery scan operation (used for PANID filtering).
        ///** Format: `S`
        // *
        // * Default value is 0xffff (Broadcast PAN) to disable PANID filtering
        // *
        // */
        //SPINEL_PROP_THREAD_DISCOVERY_SCAN_PANID = SPINEL_PROP_THREAD_EXT__BEGIN + 21,

        ///// Thread (out of band) steering data for MLE Discovery Response.
        ///** Format `E` - Write only
        // *
        // * Required capability: SPINEL_CAP_OOB_STEERING_DATA.
        // *
        // * Writing to this property allows to set/update the MLE
        // * Discovery Response steering data out of band.
        // *
        // *  - All zeros to clear the steering data (indicating that
        // *    there is no steering data).
        // *  - All 0xFFs to set steering data/bloom filter to
        // *    accept/allow all.
        // *  - A specific EUI64 which is then added to current steering
        // *    data/bloom filter.
        // *
        // */
        //SPINEL_PROP_THREAD_STEERING_DATA = SPINEL_PROP_THREAD_EXT__BEGIN + 22,



        ///// Thread Active Operational Dataset
        ///** Format: `A(t(iD))` - Read-Write
        // *
        // * This property provides access to current Thread Active Operational Dataset. A Thread device maintains the
        // * Operational Dataset that it has stored locally and the one currently in use by the partition to which it is
        // * attached. This property corresponds to the locally stored Dataset on the device.
        // *
        // * Operational Dataset consists of a set of supported properties (e.g., channel, network key, network name, PAN id,
        // * etc). Note that not all supported properties may be present (have a value) in a Dataset.
        // *
        // * The Dataset value is encoded as an array of structs containing pairs of property key (as `i`) followed by the
        // * property value (as `D`). The property value must follow the format associated with the corresponding property.
        // *
        // * On write, any unknown/unsupported property keys must be ignored.
        // *
        // * The following properties can be included in a Dataset list:
        // *
        // *   SPINEL_PROP_DATASET_ACTIVE_TIMESTAMP
        // *   SPINEL_PROP_PHY_CHAN
        // *   SPINEL_PROP_PHY_CHAN_SUPPORTED (Channel Mask Page 0)
        // *   SPINEL_PROP_NET_NETWORK_KEY
        // *   SPINEL_PROP_NET_NETWORK_NAME
        // *   SPINEL_PROP_NET_XPANID
        // *   SPINEL_PROP_MAC_15_4_PANID
        // *   SPINEL_PROP_IPV6_ML_PREFIX
        // *   SPINEL_PROP_NET_PSKC
        // *   SPINEL_PROP_DATASET_SECURITY_POLICY
        // *
        // */
        //SPINEL_PROP_THREAD_ACTIVE_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 24,

        ///// Thread Pending Operational Dataset
        ///** Format: `A(t(iD))` - Read-Write
        // *
        // * This property provide access to current locally stored Pending Operational Dataset.
        // *
        // * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_ACTIVE_DATASET.
        // *
        // * In addition supported properties in SPINEL_PROP_THREAD_ACTIVE_DATASET, the following properties can also
        // * be included in the Pending Dataset:
        // *
        // *   SPINEL_PROP_DATASET_PENDING_TIMESTAMP
        // *   SPINEL_PROP_DATASET_DELAY_TIMER
        // *
        // */
        //SPINEL_PROP_THREAD_PENDING_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 25,

        ///// Send MGMT_SET Thread Active Operational Dataset
        ///** Format: `A(t(iD))` - Write only
        // *
        // * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_ACTIVE_DATASET.
        // *
        // * This is write-only property. When written, it triggers a MGMT_ACTIVE_SET meshcop command to be sent to leader
        // * with the given Dataset. The spinel frame response should be a `LAST_STATUS` with the status of the transmission
        // * of MGMT_ACTIVE_SET command.
        // *
        // * In addition to supported properties in SPINEL_PROP_THREAD_ACTIVE_DATASET, the following property can be
        // * included in the Dataset (to allow for custom raw TLVs):
        // *
        // *    SPINEL_PROP_DATASET_RAW_TLVS
        // *
        // */
        //SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 26,

        ///// Send MGMT_SET Thread Pending Operational Dataset
        ///** Format: `A(t(iD))` - Write only
        // *
        // * This property is similar to SPINEL_PROP_THREAD_PENDING_DATASET and follows the same format and rules.
        // *
        // * In addition to supported properties in SPINEL_PROP_THREAD_PENDING_DATASET, the following property can be
        // * included the Dataset (to allow for custom raw TLVs to be provided).
        // *
        // *    SPINEL_PROP_DATASET_RAW_TLVS
        // *
        // */
        //SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 27,

        ///// Operational Dataset Active Timestamp
        ///** Format: `X` - No direct read or write
        // *
        // * It can only be included in one of the Dataset related properties below:
        // *
        // *   SPINEL_PROP_THREAD_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
        // *
        // */
        //SPINEL_PROP_DATASET_ACTIVE_TIMESTAMP = SPINEL_PROP_THREAD_EXT__BEGIN + 28,

        ///// Operational Dataset Pending Timestamp
        ///** Format: `X` - No direct read or write
        // *
        // * It can only be included in one of the Pending Dataset properties:
        // *
        // *   SPINEL_PROP_THREAD_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
        // *
        // */
        //SPINEL_PROP_DATASET_PENDING_TIMESTAMP = SPINEL_PROP_THREAD_EXT__BEGIN + 29,

        ///// Operational Dataset Delay Timer
        ///** Format: `L` - No direct read or write
        // *
        // * Delay timer (in ms) specifies the time renaming until Thread devices overwrite the value in the Active
        // * Operational Dataset with the corresponding values in the Pending Operational Dataset.
        // *
        // * It can only be included in one of the Pending Dataset properties:
        // *
        // *   SPINEL_PROP_THREAD_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
        // *
        // */
        //SPINEL_PROP_DATASET_DELAY_TIMER = SPINEL_PROP_THREAD_EXT__BEGIN + 30,

        ///// Operational Dataset Security Policy
        ///** Format: `SD` - No direct read or write
        // *
        // * It can only be included in one of the Dataset related properties below:
        // *
        // *   SPINEL_PROP_THREAD_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
        // *
        // * Content is
        // *   `S` : Key Rotation Time (in units of hour)
        // *   `C` : Security Policy Flags (as specified in Thread 1.1 Section 8.10.1.15)
        // *   `C` : Optional Security Policy Flags extension (as specified in Thread 1.2 Section 8.10.1.15).
        // *         0xf8 is used if this field is missing.
        // *
        // */
        //SPINEL_PROP_DATASET_SECURITY_POLICY = SPINEL_PROP_THREAD_EXT__BEGIN + 31,

        ///// Operational Dataset Additional Raw TLVs
        ///** Format: `D` - No direct read or write
        // *
        // * This property defines extra raw TLVs that can be added to an Operational DataSet.
        // *
        // * It can only be included in one of the following Dataset properties:
        // *
        // *   SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_SET_PENDING_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
        // *
        // */
        //SPINEL_PROP_DATASET_RAW_TLVS = SPINEL_PROP_THREAD_EXT__BEGIN + 32,

        ///// Neighbor Table Frame and Message Error Rates
        ///** Format: `A(t(ESSScc))`
        // *  Required capability: `CAP_ERROR_RATE_TRACKING`
        // *
        // * This property provides link quality related info including
        // * frame and (IPv6) message error rates for all neighbors.
        // *
        // * With regards to message error rate, note that a larger (IPv6)
        // * message can be fragmented and sent as multiple MAC frames. The
        // * message transmission is considered a failure, if any of its
        // * fragments fail after all MAC retry attempts.
        // *
        // * Data per item is:
        // *
        // *  `E`: Extended address of the neighbor
        // *  `S`: RLOC16 of the neighbor
        // *  `S`: Frame error rate (0 -> 0%, 0xffff -> 100%)
        // *  `S`: Message error rate (0 -> 0%, 0xffff -> 100%)
        // *  `c`: Average RSSI (in dBm)
        // *  `c`: Last RSSI (in dBm)
        // *
        // */
        //SPINEL_PROP_THREAD_NEIGHBOR_TABLE_ERROR_RATES = SPINEL_PROP_THREAD_EXT__BEGIN + 34,

        ///// EID (Endpoint Identifier) IPv6 Address Cache Table
        ///** Format `A(t(6SCCt(bL6)t(bSS)))
        // *
        // * This property provides Thread EID address cache table.
        // *
        // * Data per item is:
        // *
        // *  `6` : Target IPv6 address
        // *  `S` : RLOC16 of target
        // *  `C` : Age (order of use, 0 indicates most recently used entry)
        // *  `C` : Entry state (values are defined by enumeration `SPINEL_ADDRESS_CACHE_ENTRY_STATE_*`).
        // *
        // *  `t` : Info when state is `SPINEL_ADDRESS_CACHE_ENTRY_STATE_CACHED`
        // *    `b` : Indicates whether last transaction time and ML-EID are valid.
        // *    `L` : Last transaction time
        // *    `6` : Mesh-local EIDudp
        // *
        // *  `t` : Info when state is other than `SPINEL_ADDRESS_CACHE_ENTRY_STATE_CACHED`
        // *    `b` : Indicates whether the entry can be evicted.
        // *    `S` : Timeout in seconds
        // *    `S` : Retry delay (applicable if in query-retry state).
        // *
        // */
        //SPINEL_PROP_THREAD_ADDRESS_CACHE_TABLE = SPINEL_PROP_THREAD_EXT__BEGIN + 35,

        ///// Thread UDP forward stream
        ///** Format `dS6S`
        // * Required capability: `SPINEL_CAP_THREAD_UDP_FORWARD`
        // *
        // * This property helps exchange UDP packets with host.
        // *
        // *  `d`: UDP payload
        // *  `S`: Remote UDP port
        // *  `6`: Remote IPv6 address
        // *  `S`: Local UDP port
        // *
        // */
        //SPINEL_PROP_THREAD_UDP_FORWARD_STREAM = SPINEL_PROP_THREAD_EXT__BEGIN + 36,

        ///// Send MGMT_GET Thread Active Operational Dataset
        ///** Format: `A(t(iD))` - Write only
        // *
        // * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET. This
        // * property further allows the sender to not include a value associated with properties in formating of `t(iD)`,
        // * i.e., it should accept either a `t(iD)` or a `t(i)` encoding (in both cases indicating that the associated
        // * Dataset property should be requested as part of MGMT_GET command).
        // *
        // * This is write-only property. When written, it triggers a MGMT_ACTIVE_GET meshcop command to be sent to leader
        // * requesting the Dataset related properties from the format. The spinel frame response should be a `LAST_STATUS`
        // * with the status of the transmission of MGMT_ACTIVE_GET command.
        // *
        // * In addition to supported properties in SPINEL_PROP_THREAD_MGMT_SET_ACTIVE_DATASET, the following property can be
        // * optionally included in the Dataset:
        // *
        // *    SPINEL_PROP_DATASET_DEST_ADDRESS
        // *
        // */
        //SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 37,

        ///// Send MGMT_GET Thread Pending Operational Dataset
        ///** Format: `A(t(iD))` - Write only
        // *
        // * The formatting of this property follows the same rules as in SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET.
        // *
        // * This is write-only property. When written, it triggers a MGMT_PENDING_GET meshcop command to be sent to leader
        // * with the given Dataset. The spinel frame response should be a `LAST_STATUS` with the status of the transmission
        // * of MGMT_PENDING_GET command.
        // *
        // */
        //SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 38,

        ///// Operational Dataset (MGMT_GET) Destination IPv6 Address
        ///** Format: `6` - No direct read or write
        // *
        // * This property specifies the IPv6 destination when sending MGMT_GET command for either Active or Pending Dataset
        // * if not provided, Leader ALOC address is used as default.
        // *
        // * It can only be included in one of the MGMT_GET Dataset properties:
        // *
        // *   SPINEL_PROP_THREAD_MGMT_GET_ACTIVE_DATASET
        // *   SPINEL_PROP_THREAD_MGMT_GET_PENDING_DATASET
        // *
        // */
        //SPINEL_PROP_DATASET_DEST_ADDRESS = SPINEL_PROP_THREAD_EXT__BEGIN + 39,

        ///// Thread New Operational Dataset
        ///** Format: `A(t(iD))` - Read only - FTD build only
        // *
        // * This property allows host to request NCP to create and return a new Operation Dataset to use when forming a new
        // * network.
        // *
        // * Operational Dataset consists of a set of supported properties (e.g., channel, network key, network name, PAN id,
        // * etc). Note that not all supported properties may be present (have a value) in a Dataset.
        // *
        // * The Dataset value is encoded as an array of structs containing pairs of property key (as `i`) followed by the
        // * property value (as `D`). The property value must follow the format associated with the corresponding property.
        // *
        // * The following properties can be included in a Dataset list:
        // *
        // *   SPINEL_PROP_DATASET_ACTIVE_TIMESTAMP
        // *   SPINEL_PROP_PHY_CHAN
        // *   SPINEL_PROP_PHY_CHAN_SUPPORTED (Channel Mask Page 0)
        // *   SPINEL_PROP_NET_NETWORK_KEY
        // *   SPINEL_PROP_NET_NETWORK_NAME
        // *   SPINEL_PROP_NET_XPANID
        // *   SPINEL_PROP_MAC_15_4_PANID
        // *   SPINEL_PROP_IPV6_ML_PREFIX
        // *   SPINEL_PROP_NET_PSKC
        // *   SPINEL_PROP_DATASET_SECURITY_POLICY
        // *
        // */
        //SPINEL_PROP_THREAD_NEW_DATASET = SPINEL_PROP_THREAD_EXT__BEGIN + 40,

        ///// MAC CSL Period
        ///** Format: `S`
        // * Required capability: `SPINEL_CAP_THREAD_CSL_RECEIVER`
        // *
        // * The CSL period in units of 10 symbols. Value of 0 indicates that CSL should be disabled.
        // */
        //SPINEL_PROP_THREAD_CSL_PERIOD = SPINEL_PROP_THREAD_EXT__BEGIN + 41,

        ///// MAC CSL Timeout
        ///** Format: `L`
        // * Required capability: `SPINEL_CAP_THREAD_CSL_RECEIVER`
        // *
        // * The CSL timeout in seconds.
        // */
        //SPINEL_PROP_THREAD_CSL_TIMEOUT = SPINEL_PROP_THREAD_EXT__BEGIN + 42,

        ///// MAC CSL Channel
        ///** Format: `C`
        // * Required capability: `SPINEL_CAP_THREAD_CSL_RECEIVER`
        // *
        // * The CSL channel as described in chapter 4.6.5.1.2 of the Thread v1.2.0 Specification.
        // * Value of 0 means that CSL reception (if enabled) occurs on the Thread Network channel.
        // * Value from range [11,26] is an alternative channel on which a CSL reception occurs.
        // */
        //SPINEL_PROP_THREAD_CSL_CHANNEL = SPINEL_PROP_THREAD_EXT__BEGIN + 43,

        ///// Thread Domain Name
        ///** Format `U` - Read-write
        // * Required capability: `SPINEL_CAP_NET_THREAD_1_2`
        // *
        // * This property is available since Thread 1.2.0.
        // * Write to this property succeeds only when Thread protocols are disabled.
        // *
        // */
        //SPINEL_PROP_THREAD_DOMAIN_NAME = SPINEL_PROP_THREAD_EXT__BEGIN + 44,

        ///// Link metrics query
        ///** Format: `6CC` - Write-Only
        // *
        // * Required capability: `SPINEL_CAP_THREAD_LINK_METRICS`
        // *
        // * `6` : IPv6 destination address
        // * `C` : Series id (0 for Single Probe)
        // * `C` : List of requested metric ids encoded as bit fields in single byte
        // *
        // *   +---------------+----+
        // *   |    Metric     | Id |
        // *   +---------------+----+
        // *   | Received PDUs |  0 |
        // *   | LQI           |  1 |
        // *   | Link margin   |  2 |
        // *   | RSSI          |  3 |
        // *   +---------------+----+
        // *
        // * If the query succeeds, the NCP will send a result to the Host using
        // * @ref SPINEL_PROP_THREAD_LINK_METRICS_QUERY_RESULT.
        // *
        // */
        //SPINEL_PROP_THREAD_LINK_METRICS_QUERY = SPINEL_PROP_THREAD_EXT__BEGIN + 45,

        ///// Link metrics query result
        ///** Format: `6Ct(A(t(CD)))` - Unsolicited notifications only
        // *
        // * Required capability: `SPINEL_CAP_THREAD_LINK_METRICS`
        // *
        // * `6` : IPv6 destination address
        // * `C` : Status
        // * `t(A(t(CD)))` : Array of structs encoded as following:
        // *   `C` : Metric id
        // *   `D` : Metric value
        // *
        // *   +---------------+----+----------------+
        // *   |    Metric     | Id |  Value format  |
        // *   +---------------+----+----------------+
        // *   | Received PDUs |  0 | `L` (uint32_t) |
        // *   | LQI           |  1 | `C` (uint8_t)  |
        // *   | Link margin   |  2 | `C` (uint8_t)  |
        // *   | RSSI          |  3 | `c` (int8_t)   |
        // *   +---------------+----+----------------+
        // *
        // */
        //SPINEL_PROP_THREAD_LINK_METRICS_QUERY_RESULT = SPINEL_PROP_THREAD_EXT__BEGIN + 46,

        ///// Link metrics probe
        ///** Format `6CC` - Write only
        // * Required capability: `SPINEL_CAP_THREAD_LINK_METRICS`
        // *
        // * Send a MLE Link Probe message to the peer.
        // *
        // * `6` : IPv6 destination address
        // * `C` : The Series ID for which this Probe message targets at
        // * `C` : The length of the Probe message, valid range: [0, 64]
        // *
        // */
        //SPINEL_PROP_THREAD_LINK_METRICS_PROBE = SPINEL_PROP_THREAD_EXT__BEGIN + 47,

        ///// Link metrics Enhanced-ACK Based Probing management
        ///** Format: 6Cd - Write only
        // *
        // * Required capability: `SPINEL_CAP_THREAD_LINK_METRICS`
        // *
        // * `6` : IPv6 destination address
        // * `C` : Indicate whether to register or clear the probing. `0` - clear, `1` - register
        // * `C` : List of requested metric ids encoded as bit fields in single byte
        // *
        // *   +---------------+----+
        // *   |    Metric     | Id |
        // *   +---------------+----+
        // *   | LQI           |  1 |
        // *   | Link margin   |  2 |
        // *   | RSSI          |  3 |
        // *   +---------------+----+
        // *
        // * Result of configuration is reported asynchronously to the Host using the
        // * @ref SPINEL_PROP_THREAD_LINK_METRICS_MGMT_RESPONSE.
        // *
        // * Whenever Enh-ACK IE report is received it is passed to the Host using the
        // * @ref SPINEL_PROP_THREAD_LINK_METRICS_MGMT_ENH_ACK_IE property.
        // *
        // */
        //SPINEL_PROP_THREAD_LINK_METRICS_MGMT_ENH_ACK = SPINEL_PROP_THREAD_EXT__BEGIN + 48,

        ///// Link metrics Enhanced-ACK Based Probing IE report
        ///** Format: SEA(t(CD)) - Unsolicited notifications only
        // *
        // * Required capability: `SPINEL_CAP_THREAD_LINK_METRICS`
        // *
        // * `S` : Short address of the Probing Subject
        // * `E` : Extended address of the Probing Subject
        // * `t(A(t(CD)))` : Struct that contains array of structs encoded as following:
        // *   `C` : Metric id
        // *   `D` : Metric value
        // *
        // *   +---------------+----+----------------+
        // *   |    Metric     | Id |  Value format  |
        // *   +---------------+----+----------------+
        // *   | LQI           |  1 | `C` (uint8_t)  |
        // *   | Link margin   |  2 | `C` (uint8_t)  |
        // *   | RSSI          |  3 | `c` (int8_t)   |
        // *   +---------------+----+----------------+
        // *
        // */
        //SPINEL_PROP_THREAD_LINK_METRICS_MGMT_ENH_ACK_IE = SPINEL_PROP_THREAD_EXT__BEGIN + 49,

        ///// Link metrics Forward Tracking Series management
        ///** Format: 6CCC - Write only
        // *
        // * Required capability: `SPINEL_CAP_THREAD_LINK_METRICS`
        // *
        // * `6` : IPv6 destination address
        // * `C` : Series id
        // * `C` : Tracked frame types encoded as bit fields in single byte, if equal to zero,
        // *       accounting is stopped and a series is removed
        // * `C` : Requested metric ids encoded as bit fields in single byte
        // *
        // *   +------------------+----+
        // *   |    Frame type    | Id |
        // *   +------------------+----+
        // *   | MLE Link Probe   |  0 |
        // *   | MAC Data         |  1 |
        // *   | MAC Data Request |  2 |
        // *   | MAC ACK          |  3 |
        // *   +------------------+----+
        // *
        // *   +---------------+----+
        // *   |    Metric     | Id |
        // *   +---------------+----+
        // *   | Received PDUs |  0 |
        // *   | LQI           |  1 |
        // *   | Link margin   |  2 |
        // *   | RSSI          |  3 |
        // *   +---------------+----+
        // *
        // * Result of configuration is reported asynchronously to the Host using the
        // * @ref SPINEL_PROP_THREAD_LINK_METRICS_MGMT_RESPONSE.
        // *
        // */
        //SPINEL_PROP_THREAD_LINK_METRICS_MGMT_FORWARD = SPINEL_PROP_THREAD_EXT__BEGIN + 50,

        ///// Link metrics management response
        ///** Format: 6C - Unsolicited notifications only
        // *
        // * Required capability: `SPINEL_CAP_THREAD_LINK_METRICS`
        // *
        // * `6` : IPv6 source address
        // * `C` : Received status
        // *
        // */
        //SPINEL_PROP_THREAD_LINK_METRICS_MGMT_RESPONSE = SPINEL_PROP_THREAD_EXT__BEGIN + 51,

        ///// Multicast Listeners Register Request
        ///** Format `t(A(6))A(t(CD))` - Write-only
        // * Required capability: `SPINEL_CAP_NET_THREAD_1_2`
        // *
        // * `t(A(6))`: Array of IPv6 multicast addresses
        // * `A(t(CD))`: Array of structs holding optional parameters as follows
        // *   `C`: Parameter id
        // *   `D`: Parameter value
        // *
        // *   +----------------------------------------------------------------+
        // *   | Id:   SPINEL_THREAD_MLR_PARAMID_TIMEOUT                        |
        // *   | Type: `L`                                                      |
        // *   | Description: Timeout in seconds. If this optional parameter is |
        // *   |   omitted, the default value of the BBR will be used.          |
        // *   | Special values:                                                |
        // *   |   0 causes given addresses to be removed                       |
        // *   |   0xFFFFFFFF is permanent and persistent registration          |
        // *   +----------------------------------------------------------------+
        // *
        // * Write to this property initiates update of Multicast Listeners Table on the primary BBR.
        // * If the write succeeded, the result of network operation will be notified later by the
        // * SPINEL_PROP_THREAD_MLR_RESPONSE property. If the write fails, no MLR.req is issued and
        // * notifiaction through the SPINEL_PROP_THREAD_MLR_RESPONSE property will not occur.
        // *
        // */
        //SPINEL_PROP_THREAD_MLR_REQUEST = SPINEL_PROP_THREAD_EXT__BEGIN + 52,

        ///// Multicast Listeners Register Response
        ///** Format `CCt(A(6))` - Unsolicited notifications only
        // * Required capability: `SPINEL_CAP_NET_THREAD_1_2`
        // *
        // * `C`: Status
        // * `C`: MlrStatus (The Multicast Listener Registration Status)
        // * `A(6)`: Array of IPv6 addresses that failed to be updated on the primary BBR
        // *
        // * This property is notified asynchronously when the NCP receives MLR.rsp following
        // * previous write to the SPINEL_PROP_THREAD_MLR_REQUEST property.
        // */
        //SPINEL_PROP_THREAD_MLR_RESPONSE = SPINEL_PROP_THREAD_EXT__BEGIN + 53,

        ///// Interface Identifier specified for Thread Domain Unicast Address.
        ///** Format: `A(C)` - Read-write
        // *
        // *   `A(C)`: Interface Identifier (8 bytes).
        // *
        // * Required capability: SPINEL_CAP_DUA
        // *
        // * If write to this property is performed without specified parameter
        // * the Interface Identifier of the Thread Domain Unicast Address will be cleared.
        // * If the DUA Interface Identifier is cleared on the NCP device,
        // * the get spinel property command will be returned successfully without specified parameter.
        // *
        // */
        //SPINEL_PROP_THREAD_DUA_ID = SPINEL_PROP_THREAD_EXT__BEGIN + 54,

        ///// Thread 1.2 Primary Backbone Router information in the Thread Network.
        ///** Format: `SSLC` - Read-Only
        // *
        // * Required capability: `SPINEL_CAP_NET_THREAD_1_2`
        // *
        // * `S`: Server.
        // * `S`: Reregistration Delay (in seconds).
        // * `L`: Multicast Listener Registration Timeout (in seconds).
        // * `C`: Sequence Number.
        // *
        // */
        //SPINEL_PROP_THREAD_BACKBONE_ROUTER_PRIMARY = SPINEL_PROP_THREAD_EXT__BEGIN + 55,

        ///// Thread 1.2 Backbone Router local state.
        ///** Format: `C` - Read-Write
        // *
        // * Required capability: `SPINEL_CAP_THREAD_BACKBONE_ROUTER`
        // *
        // * The valid values are specified by SPINEL_THREAD_BACKBONE_ROUTER_STATE_<state> enumeration.
        // * Backbone functionality will be disabled if SPINEL_THREAD_BACKBONE_ROUTER_STATE_DISABLED
        // * is writted to this property, enabled otherwise.
        // *
        // */
        //SPINEL_PROP_THREAD_BACKBONE_ROUTER_LOCAL_STATE = SPINEL_PROP_THREAD_EXT__BEGIN + 56,

        ///// Local Thread 1.2 Backbone Router configuration.
        ///** Format: SLC - Read-Write
        // *
        // * Required capability: `SPINEL_CAP_THREAD_BACKBONE_ROUTER`
        // *
        // * `S`: Reregistration Delay (in seconds).
        // * `L`: Multicast Listener Registration Timeout (in seconds).
        // * `C`: Sequence Number.
        // *
        // */
        //SPINEL_PROP_THREAD_BACKBONE_ROUTER_LOCAL_CONFIG = SPINEL_PROP_THREAD_EXT__BEGIN + 57,

        ///// Register local Thread 1.2 Backbone Router configuration.
        ///** Format: Empty (Write only).
        // *
        // * Required capability: `SPINEL_CAP_THREAD_BACKBONE_ROUTER`
        // *
        // * Writing to this property (with any value) will register local Backbone Router configuration.
        // *
        // */
        //SPINEL_PROP_THREAD_BACKBONE_ROUTER_LOCAL_REGISTER = SPINEL_PROP_THREAD_EXT__BEGIN + 58,

        ///// Thread 1.2 Backbone Router registration jitter.
        ///** Format: `C` - Read-Write
        // *
        // * Required capability: `SPINEL_CAP_THREAD_BACKBONE_ROUTER`
        // *
        // * `C`: Backbone Router registration jitter.
        // *
        // */
        //SPINEL_PROP_THREAD_BACKBONE_ROUTER_LOCAL_REGISTRATION_JITTER = SPINEL_PROP_THREAD_EXT__BEGIN + 59,

        //SPINEL_PROP_THREAD_EXT__END = 0x1600,

        #endregion

        //**********************************************************************
        //
        //      Spinel IPv6 Properties
        //
        //**********************************************************************

        protected internal IPv6Address GetIPv6LLAddr()
        {
            // SPINEL_PROP_IPV6_LL_ADDR Format: `6` - Read only
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_IPV6_LL_ADDR);

            try
            {
                return (IPv6Address)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("IP addesss format violation");
            }
        }

        protected internal IPv6Address GetIPv6MLAddr()
        {
            //SPINEL_PROP_IPV6_ML_ADDR Format: `6` - Read only
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_IPV6_ML_ADDR);

            try
            {
                return (IPv6Address)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("IP addesss format violation");
            }
        }

        protected internal OpenThreadIp6Prefix GetIPv6MLPrefix()
        {
            //SPINEL_PROP_IPV6_ML_PREFIX /** Format: `6C` - Read-write
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_IPV6_ML_PREFIX);

            try
            {
                return (OpenThreadIp6Prefix)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("IP addesss format violation");
            }
        }

        protected internal IPv6Address[] GetIPv6AddressTable()
        {
            //SPINEL_PROP_IPV6_ADDRESS_TABLE
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_IPV6_ADDRESS_TABLE);

            try
            {
                return (IPv6Address[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("IP addesss format violation");
            }
        }

        [Obsolete("IPv6 Route Table - Deprecated")]
        protected internal void GetIPv6RouteTable()
        {
            //SPINEL_PROP_IPV6_ROUTE_TABLE          
            throw new NotImplementedException();
        }

        protected internal bool GetIPv6ICMPPingOffload()
        {
            //SPINEL_PROP_IPV6_ICMP_PING_OFFLOAD Format `b` - Read-write
            throw new NotImplementedException();
        }

        protected internal bool SetIPv6ICMPPingOffload(bool AllowRespondICMPPingRequests)
        {
            //SPINEL_PROP_IPV6_ICMP_PING_OFFLOAD Format `b` - Read-write
            throw new NotImplementedException();
        }

        protected internal IPv6Address[] GetIPv6MulticastAddressTable()
        {
            //SPINEL_PROP_IPV6_MULTICAST_ADDRESS_TABLE
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_IPV6_MULTICAST_ADDRESS_TABLE);

            try
            {
                return (IPv6Address[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("IP addesss format violation");
            }
        }

        protected internal SpinelIPv6ICMPPingOffloadMode GetIPv6IcmpPingOffloadMode()
        {
            //SPINEL_PROP_IPV6_MULTICAST_ADDRESS_TABLE
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_IPV6_ICMP_PING_OFFLOAD_MODE);

            try
            {
                return (SpinelIPv6ICMPPingOffloadMode)frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("IP addesss format violation");
            }
        }


        //**********************************************************************
        //
        //      Stream Properties
        //
        //**********************************************************************

        protected internal byte[] GetStreamDebug()
        {
            ///SPINEL_PROP_STREAM_DEBUG ** Format: `dD` (stream, read only)
            throw new NotImplementedException();
        }


        protected internal void SetPropStreamNet(byte[] frame, bool waitResponse = true)
        {
            byte[] dataCombined = mEncoder.EncodeDataWithLength(frame);

            PropertySetValue(SpinelProperties.SPINEL_PROP_STREAM_NET, dataCombined, "dD", 129, waitResponse);
        }

        //    SPINEL_PROP_STREAM__BEGIN = 0x70,

        ///// Debug Stream
        ///** Format: `U` (stream, read only)
        // *
        // * This property is a streaming property, meaning that you cannot explicitly
        // * fetch the value of this property. The stream provides human-readable debugging
        // * output which may be displayed in the host logs.
        // *
        // * The location of newline characters is not assumed by the host: it is
        // * the NCP's responsibility to insert newline characters where needed,
        // * just like with any other text stream.
        // *
        // * To receive the debugging stream, you wait for `CMD_PROP_VALUE_IS`
        // * commands for this property from the NCP.
        // *
        // */
        //SPINEL_PROP_STREAM_DEBUG = SPINEL_PROP_STREAM__BEGIN + 0,

        ///// Raw Stream
        ///** Format: `dD` (stream, read only)
        // *  Required Capability: SPINEL_CAP_MAC_RAW or SPINEL_CAP_CONFIG_RADIO
        // *
        // * This stream provides the capability of sending and receiving raw 15.4 frames
        // * to and from the radio. The exact format of the frame metadata and data is
        // * dependent on the MAC and PHY being used.
        // *
        // * This property is a streaming property, meaning that you cannot explicitly
        // * fetch the value of this property. To receive traffic, you wait for
        // * `CMD_PROP_VALUE_IS` commands with this property id from the NCP.
        // *
        // * The general format of this property is:
        // *
        // *    `d` : frame data
        // *    `D` : frame meta data
        // *
        // * The frame meta data is optional. Frame metadata MAY be empty or partially
        // * specified. Partially specified metadata MUST be accepted. Default values
        // * are used for all unspecified fields.
        // *
        // * The frame metadata field consists of the following fields:
        // *
        // *   `c` : Received Signal Strength (RSSI) in dBm - default is -128
        // *   `c` : Noise floor in dBm - default is -128
        // *   `S` : Flags (see below).
        // *   `d` : PHY-specific data/struct
        // *   `d` : Vendor-specific data/struct
        // *
        // * Flags fields are defined by the following enumeration bitfields:
        // *
        // *   SPINEL_MD_FLAG_TX       = 0x0001 :  Packet was transmitted, not received.
        // *   SPINEL_MD_FLAG_BAD_FCS  = 0x0004 :  Packet was received with bad FCS
        // *   SPINEL_MD_FLAG_DUPE     = 0x0008 :  Packet seems to be a duplicate
        // *   SPINEL_MD_FLAG_RESERVED = 0xFFF2 :  Flags reserved for future use.
        // *
        // * The format of PHY-specific data for a Thread device contains the following
        // * optional fields:
        // *   `C` : 802.15.4 channel (Receive channel)
        // *   `C` : IEEE 802.15.4 LQI
        // *   `L` : The timestamp milliseconds
        // *   `S` : The timestamp microseconds, offset to mMsec
        // *
        // * Frames written to this stream with `CMD_PROP_VALUE_SET` will be sent out
        // * over the radio. This allows the caller to use the radio directly.
        // *
        // * The frame meta data for the `CMD_PROP_VALUE_SET` contains the following
        // * optional fields.  Default values are used for all unspecified fields.
        // *
        // *  `C` : Channel (for frame tx)
        // *  `C` : Maximum number of backoffs attempts before declaring CCA failure
        // *        (use Thread stack default if not specified)
        // *  `C` : Maximum number of retries allowed after a transmission failure
        // *        (use Thread stack default if not specified)
        // *  `b` : Set to true to enable CSMA-CA for this packet, false otherwise.
        // *        (default true).
        // *  `b` : Set to true to indicate it is a retransmission packet, false otherwise.
        // *        (default false).
        // *  `b` : Set to true to indicate that SubMac should skip AES processing, false otherwise.
        // *        (default false).
        // *
        // */
        //SPINEL_PROP_STREAM_RAW = SPINEL_PROP_STREAM__BEGIN + 1,

        ///// (IPv6) Network Stream
        ///** Format: `dD` (stream, read only)
        // *
        // * This stream provides the capability of sending and receiving (IPv6)
        // * data packets to and from the currently attached network. The packets
        // * are sent or received securely (encryption and authentication).
        // *
        // * This property is a streaming property, meaning that you cannot explicitly
        // * fetch the value of this property. To receive traffic, you wait for
        // * `CMD_PROP_VALUE_IS` commands with this property id from the NCP.
        // *
        // * To send network packets, you call `CMD_PROP_VALUE_SET` on this property with
        // * the value of the packet.
        // *
        // * The general format of this property is:
        // *
        // *    `d` : packet data
        // *    `D` : packet meta data
        // *
        // * The packet metadata is optional. Packet meta data MAY be empty or partially
        // * specified. Partially specified metadata MUST be accepted. Default values
        // * are used for all unspecified fields.
        // *
        // * For OpenThread the meta data is currently empty.
        // *
        // */
        //SPINEL_PROP_STREAM_NET = SPINEL_PROP_STREAM__BEGIN + 2,

        ///// (IPv6) Network Stream Insecure
        ///** Format: `dD` (stream, read only)
        // *
        // * This stream provides the capability of sending and receiving unencrypted
        // * and unauthenticated data packets to and from nearby devices for the
        // * purposes of device commissioning.
        // *
        // * This property is a streaming property, meaning that you cannot explicitly
        // * fetch the value of this property. To receive traffic, you wait for
        // * `CMD_PROP_VALUE_IS` commands with this property id from the NCP.
        // *
        // * To send network packets, you call `CMD_PROP_VALUE_SET` on this property with
        // * the value of the packet.
        // *
        // * The general format of this property is:
        // *
        // *    `d` : packet data
        // *    `D` : packet meta data
        // *
        // * The packet metadata is optional. Packet meta data MAY be empty or partially
        // * specified. Partially specified metadata MUST be accepted. Default values
        // * are used for all unspecified fields.
        // *
        // * For OpenThread the meta data is currently empty.
        // *
        // */
        //SPINEL_PROP_STREAM_NET_INSECURE = SPINEL_PROP_STREAM__BEGIN + 3,

        ///// Log Stream
        ///** Format: `UD` (stream, read only)
        // *
        // * This property is a read-only streaming property which provides
        // * formatted log string from NCP. This property provides asynchronous
        // * `CMD_PROP_VALUE_IS` updates with a new log string and includes
        // * optional meta data.
        // *
        // *   `U`: The log string
        // *   `D`: Log metadata (optional).
        // *
        // * Any data after the log string is considered metadata and is OPTIONAL.
        // * Presence of `SPINEL_CAP_OPENTHREAD_LOG_METADATA` capability
        // * indicates that OpenThread log metadata format is used as defined
        // * below:
        // *
        // *    `C`: Log level (as per definition in enumeration
        // *         `SPINEL_NCP_LOG_LEVEL_<level>`)
        // *    `i`: OpenThread Log region (as per definition in enumeration
        // *         `SPINEL_NCP_LOG_REGION_<region>).
        // *    `X`: Log timestamp = <timestamp_base> + <current_time_ms>
        // *
        // */
        //SPINEL_PROP_STREAM_LOG = SPINEL_PROP_STREAM__BEGIN + 4,

        //SPINEL_PROP_STREAM__END = 0x80,


        //**********************************************************************
        //
        //      Counter Properties
        //
        //**********************************************************************

        protected internal void SetCntrReset()
        {
            PropertySetValue(SpinelProperties.SPINEL_PROP_CNTR_RESET, 1, "C");
        }


        protected internal ushort[] GetMsgBufferCounters()
        {
            FrameData frameData = PropertyGetValue(SpinelProperties.SPINEL_PROP_MSG_BUFFER_COUNTERS);

            try
            {
                return (ushort[])frameData.Response;
            }
            catch
            {
                throw new SpinelProtocolExceptions("Buffer counters format violation");
            }
        }


        protected internal void Transact(int commandId, byte[] payload, byte tID = SpinelCommands.HEADER_DEFAULT)
        {
            byte[] packet = EncodePacket(commandId, tID, payload);
            StreamTx(packet);
        }

        protected internal void Transact(int commandId, byte tID = SpinelCommands.HEADER_DEFAULT)
        {
            Transact(commandId, null, tID);
        }

        protected internal byte[] EncodePacket(int commandId, byte tid = SpinelCommands.HEADER_DEFAULT, params byte[] payload)
        {
            byte[] tidBytes = new byte[1] { tid };
            byte[] commandBytes = mEncoder.EncodeValue(commandId);
            byte[] packet = new byte[commandBytes.Length + tidBytes.Length + (payload == null ? 0 : payload.Length)];

            if (payload != null)
            {
                packet = Utilities.CombineArrays(tidBytes, commandBytes, payload);
            }
            else
            {
                packet = Utilities.CombineArrays(tidBytes, commandBytes);
            }

            return packet;
        }

        private void PacketHandler(FrameData frameData)
        {
            uint propertyId = frameData.PropertyId;

            switch ((SpinelProperties)propertyId)
            {
                case SpinelProperties.SPINEL_PROP_LAST_STATUS:              
                case SpinelProperties.SPINEL_PROP_STREAM_NET:
                case SpinelProperties.SPINEL_PROP_STREAM_NET_INSECURE:
                case SpinelProperties.SPINEL_PROP_THREAD_UDP_FORWARD_STREAM:                      
                case SpinelProperties.SPINEL_PROP_STREAM_RAW:                                        
                case SpinelProperties.SPINEL_PROP_MAC_SCAN_STATE:    
                case SpinelProperties.SPINEL_PROP_MAC_SCAN_BEACON:                    
                case SpinelProperties.SPINEL_PROP_MAC_ENERGY_SCAN_RESULT:
                //case SpinelProperties.SPINEL_PROP_THREAD_MLR_RESPONSE:                  
                //case SpinelProperties.SPINEL_PROP_SRP_CLIENT_EVENT:                    
                //case SpinelProperties.SPINEL_PROP_THREAD_LINK_METRICS_QUERY_RESULT:                    
                //case SpinelProperties.SPINEL_PROP_THREAD_LINK_METRICS_MGMT_RESPONSE:                    
                //case SpinelProperties.SPINEL_PROP_THREAD_LINK_METRICS_MGMT_ENH_ACK_IE:
                    
                    if (OnPropertyChanged != null)
                    {                     
                        OnPropertyChanged(propertyId, frameData.Response);
                    }
                    return;                
            }

            if(frameData.TID == 0x80)
            {
                switch ((SpinelProperties)frameData.PropertyId)
                {
                    case SpinelProperties.SPINEL_PROP_IPV6_ADDRESS_TABLE:                       
                    case SpinelProperties.SPINEL_PROP_NET_ROLE:                        
                    case SpinelProperties.SPINEL_PROP_IPV6_LL_ADDR:                                          
                    case SpinelProperties.SPINEL_PROP_IPV6_ML_ADDR:                        
                    case SpinelProperties.SPINEL_PROP_NET_PARTITION_ID:                        
                    case SpinelProperties.SPINEL_PROP_THREAD_CHILD_TABLE:                       
                    case SpinelProperties.SPINEL_PROP_NET_KEY_SEQUENCE_COUNTER:                        
                    case SpinelProperties.SPINEL_PROP_THREAD_LEADER_NETWORK_DATA:                        
                    case SpinelProperties.SPINEL_PROP_IPV6_MULTICAST_ADDRESS_TABLE:                        
                    case SpinelProperties.SPINEL_PROP_PHY_CHAN:                       
                    case SpinelProperties.SPINEL_PROP_MAC_15_4_PANID:
                    case SpinelProperties.SPINEL_PROP_NET_NETWORK_NAME:                       
                    case SpinelProperties.SPINEL_PROP_NET_XPANID:                        
                    case SpinelProperties.SPINEL_PROP_NET_NETWORK_KEY:                      
                    case SpinelProperties.SPINEL_PROP_NET_PSKC:                       
                    case SpinelProperties.SPINEL_PROP_CHANNEL_MANAGER_NEW_CHANNEL:                       
                    case SpinelProperties.SPINEL_PROP_PHY_CHAN_SUPPORTED:

                        if (OnPropertyChanged != null)
                        {
                            OnPropertyChanged(propertyId, frameData.Response);
                        }

                        return;
                }
            }
            
            if (OnFrameDataReceived != null)
            {
                OnFrameDataReceived(frameData);
            }           
        }


        private void StreamDataReceived()
        {
            lock (rxLocker)
            {
                StreamRX();
            }

            receivedPacketWaitHandle.Set();

            if (isSyncFrameExpecting)
            {
                return;
            }

            while (waitingQueue.Count != 0)
            {

                FrameData frameData = waitingQueue.Dequeue() as FrameData;

                PacketHandler(frameData);                
            }

            //  receivedPacketWaitHandle.Reset();
        }

        private object PropertyChangeValue(int commandId, SpinelProperties propertyId, byte[] propertyValue, string propertyFormat = "B", byte tid = SpinelCommands.HEADER_DEFAULT, bool waitResponse = true)
        {
            FrameData responseFrame = null;
            isSyncFrameExpecting = true;
            byte[] payload = mEncoder.EncodeValue(((int)propertyId));

            if (propertyFormat != null)
            {
                payload = Utilities.CombineArrays(payload, propertyValue);
            }

            int uid = Utilities.GetUID(((int)propertyId), tid);

            lock (txLocker)
            {
                Transact(commandId, payload, tid);
            }

            if (!waitResponse)
            {
                isSyncFrameExpecting = false;
                return null;
            }

            receivedPacketWaitHandle.Reset();
#if DEBUG
            if (!receivedPacketWaitHandle.WaitOne(150000, false))
#else
            if (!receivedPacketWaitHandle.WaitOne(5000, false))
#endif
            {
                throw new SpinelProtocolExceptions("Timeout for sync packet " + commandId);
            }

            if (waitingQueue.Count > 0)
            {
                while (waitingQueue.Count != 0)
                {
                    FrameData frameData = waitingQueue.Dequeue() as FrameData;

                    if (frameData.UID == uid)
                    {
                        responseFrame = frameData;
                        isSyncFrameExpecting = false;
                    }
                    else
                    {
                        PacketHandler(frameData);                        
                    }
                }
            }
            else
            {
                throw new SpinelProtocolExceptions("No response packet for command" + commandId);
            }

            return responseFrame;
        }

        private void StreamTx(byte[] packet)
        {
            hdlcInterface.Write(packet);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StreamRX(int timeout = 0)
        {
            DateTime start = DateTime.UtcNow;

            bool dataPooled = false;

            while (true)
            {
                TimeSpan elapsed = DateTime.UtcNow - start;

                if (timeout != 0)
                {
                    if (elapsed.Seconds > timeout)
                    {
                        break;
                    }
                }

                if (stream.IsDataAvailable)
                {
                    byte[] frameData = hdlcInterface.Read();

                    FrameData frameDecoded;
                    SpinelFrameDecoder.DecodeFrame(frameData, out frameDecoded);
                    waitingQueue.Enqueue(frameDecoded);
                    dataPooled = true;
                }
                else
                {
                    //  Console.WriteLine("Serial data not available. Data pooled :" + dataPooled.ToString() );
                }

                if (!stream.IsDataAvailable && dataPooled)
                {
                    break;
                }
            }
        }

        private FrameData PropertyGetValue(SpinelProperties propertyId, byte tid = SpinelCommands.HEADER_DEFAULT)
        {
            return PropertyChangeValue(SpinelCommands.CMD_PROP_VALUE_GET, propertyId, null, null, tid) as FrameData;
        }

        private FrameData PropertySetValue(SpinelProperties propertyId, ushort propertyValue, string propertyFormat = "B", byte tid = SpinelCommands.HEADER_DEFAULT)
        {
            byte[] propertyValueArray = mEncoder.EncodeValue(propertyValue, propertyFormat);

            return PropertySetValue(propertyId, propertyValueArray, propertyFormat, tid);
        }

        private FrameData PropertySetValue(SpinelProperties propertyId, byte propertyValue, string propertyFormat = "B", byte tid = SpinelCommands.HEADER_DEFAULT)
        {
            byte[] propertyValueArray = mEncoder.EncodeValue(propertyValue, propertyFormat);

            return PropertySetValue(propertyId, propertyValueArray, propertyFormat, tid);
        }

        private FrameData PropertySetValue(SpinelProperties propertyId, sbyte propertyValue, string propertyFormat = "B", byte tid = SpinelCommands.HEADER_DEFAULT)
        {
            byte[] propertyValueArray = mEncoder.EncodeValue(propertyValue, propertyFormat);

            return PropertySetValue(propertyId, propertyValueArray, propertyFormat, tid);
        }

        private FrameData PropertySetValue(SpinelProperties propertyId, string propertyValue, string propertyFormat = "B", byte tid = SpinelCommands.HEADER_DEFAULT)
        {
            byte[] propertyValueArray = mEncoder.EncodeValue(propertyValue, propertyFormat);

            return PropertySetValue(propertyId, propertyValueArray, propertyFormat, tid);
        }

        private FrameData PropertySetValue(SpinelProperties propertyId, uint propertyValue, string propertyFormat = "L", byte tid = SpinelCommands.HEADER_DEFAULT)
        {
            byte[] propertyValueArray = mEncoder.EncodeValue(propertyValue, propertyFormat);

            return PropertySetValue(propertyId, propertyValueArray, propertyFormat, tid);
        }

        private FrameData PropertySetValue(SpinelProperties propertyId, byte[] propertyValue, string propertyFormat = "B", byte tid = SpinelCommands.HEADER_DEFAULT, bool waitResponse = true)
        {
            return PropertyChangeValue(SpinelCommands.CMD_PROP_VALUE_SET, propertyId, propertyValue, propertyFormat, tid, waitResponse) as FrameData;
        }
    }
}
