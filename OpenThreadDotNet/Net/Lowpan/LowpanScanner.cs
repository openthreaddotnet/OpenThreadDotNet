using System.Collections;
using System.Threading;


#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
using nanoFramework.OpenThread.Spinel;
using nanoFramework.OpenThread.Core;

namespace nanoFramework.OpenThread.Net.Lowpan
{
#else
using dotNETCore.OpenThread.NCP;
using dotNETCore.OpenThread.Spinel;
using dotNETCore.OpenThread.Core;

namespace dotNETCore.OpenThread.Net.Lowpan
{
#endif
    public class LowpanScanner
    {
        private WpanApi wpanApi;
        private byte[] scanMask;
        private sbyte txPower;

        private ArrayList scanMacResult = new ArrayList();
        private ArrayList scanEnergyResult = new ArrayList();

        private AutoResetEvent scanThread = new AutoResetEvent(false);

        public byte[] ScanMask
        {
            get { return scanMask; }
            set
            {
                if (wpanApi.SetMacScanMask((byte[])value))
                {
                    scanMask = value;
                }
            }
        }

        public sbyte TxPower
        {
            get { return txPower; }
            set
            {
                if (wpanApi.SetPhyTxPower(value))
                {
                    txPower = value;
                }
            }
        }
       
        internal LowpanScanner(WpanApi WpanApi)
        {
            this.wpanApi = WpanApi;
            this.wpanApi.OnPropertyChanged += WpanApi_OnPropertyChanged;
            scanMask = this.wpanApi.GetMacScanMask();
            txPower = this.wpanApi.GetPhyTxPower();
        }

        public LowpanBeaconInfo[] ScanBeacon()
        {
            wpanApi.SetMacScanState(SpinelScanState.SPINEL_SCAN_STATE_BEACON);
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

        public LowpanChannelInfo[] ScanEnergy()
        {
            wpanApi.SetMacScanState(SpinelScanState.SPINEL_SCAN_STATE_ENERGY);
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

        private void WpanApi_OnPropertyChanged(uint PropertyId, object PropertyValue)
        {
            switch ((SpinelProperties)PropertyId)
            {               
                case SpinelProperties.SPINEL_PROP_MAC_SCAN_BEACON:

                    ArrayList scanInfo = (ArrayList)PropertyValue;
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
                    break;

                case SpinelProperties.SPINEL_PROP_MAC_ENERGY_SCAN_RESULT:

                    ArrayList energyScan = (ArrayList)PropertyValue;

                    LowpanChannelInfo lowpanChannelInfo = new LowpanChannelInfo();

                    lowpanChannelInfo.Channel = (byte)energyScan[0];
                    lowpanChannelInfo.Rssi = (sbyte)energyScan[1];
                    scanEnergyResult.Add(lowpanChannelInfo);

                    break;

                case SpinelProperties.SPINEL_PROP_MAC_SCAN_STATE:

                    SpinelScanState scanState = (SpinelScanState)(PropertyValue);

                    if (scanState == SpinelScanState.SPINEL_SCAN_STATE_IDLE)
                    {
                        scanThread.Set();
                    }

                    break;
            }            
        }
    }
}
