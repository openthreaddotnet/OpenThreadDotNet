using System;
using System.IO.Ports;

#if (NANOFRAMEWORK_1_0)

namespace nanoFramework.OpenThread.NCP
{ 
#else

namespace dotNETCore.OpenThread.NCP
{
#endif
    internal class SerialStream : IStream
    {
        private SerialPort serialPort;
        private string portName;

        public event SerialDataReceivedEventHandler SerialDataReceived;

        public SerialStream() { }

        public SerialStream(string portName)
        {
            this.portName = portName;
        }

        public bool IsDataAvailable
        {
            get { return serialPort.BytesToRead > 0 ? true:false; }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
            try
            {
                serialPort.Open();                              
            }
            catch(Exception ex)
            {
                throw ex;
            }           
        }

        public byte[] Read()
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] data)
        {
            serialPort.Write(data, 0, data.Length);
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialDataReceived();
        }

        public byte ReadByte()
        {
            //  return Convert.ToByte(serialPort.ReadByte());
            return (byte)serialPort.ReadByte();       
        }
    }
}
