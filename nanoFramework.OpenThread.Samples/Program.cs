using nanoFramework.Hardware.Esp32;
using nanoFramework.OpenThread.NCP;
using nanoFramework.OpenThread.Net;
using nanoFramework.OpenThread.Net.Sockets;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace nanoFramework.OpenThread.Samples
{
    public class Program
    {       
        private static string networkname = "OpenThreadNano";
        private static string masterkey = "00112233445566778899aabbccddeeff";
        private static byte channel = 11;
        private static ushort panid = 1000;

        private static ushort port = 1234;

        private static GpioPin ledGreen;
        private static GpioController gpioController = new GpioController();

        private static LoWPAN loWPAN = new LoWPAN("COM2");

        public static void Main()
        {
            Configuration.SetPinFunction(Gpio.IO16, DeviceFunction.COM2_TX);
            Configuration.SetPinFunction(Gpio.IO17, DeviceFunction.COM2_RX);

            loWPAN.Open();

            try
            {
                loWPAN.Form(networkname, channel, masterkey, panid);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            

            Socket receiver = new Socket();
            receiver.Bind(IPAddress.Any, port);
            IPEndPoint remoteIp = null;

            while (true)
            {
                if (receiver.Poll(-1, SelectMode.SelectRead))
                {
                    byte[] data = receiver.Receive(ref remoteIp);
                    string message = Encoding.UTF8.GetString(data, 0, data.Length);
                    Debug.WriteLine("\n");
                    Debug.WriteLine(message.Length + " bytes from " + remoteIp.Address + " " + remoteIp.Port + " " + message);
                    Debug.WriteLine(">");

                    if (message.ToLower() == "ledon")
                    {
                        ledGreen.Write(PinValue.High);
                    }
                    else if (message.ToLower() == "ledoff")
                    {
                        ledGreen.Write(PinValue.Low);
                    }
                }
            }
        }
    }
}
