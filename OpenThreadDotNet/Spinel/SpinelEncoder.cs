﻿using System;
using System.Text;

#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.NCP;
namespace nanoFramework.OpenThread.Spinel
{ 
#else
using dotNETCore.OpenThread.NCP;
namespace dotNETCore.OpenThread.Spinel
{
#endif  
    public class SpinelEncoder
    {
        public byte[] EncodeValue(string PropertyValue, string PropertyFormat = "B")
        {
            if (PropertyFormat == "U")
            {
                return EncodeUtf8(PropertyValue);
            }
            //else if (PropertyFormat == "D")
            //{
            //    return EncodeData(PropertyValue);
            //}
            else
            {
                return BitConverter.GetBytes(int.Parse(PropertyValue));
            }
        }

        public byte[] EncodeValue(byte[] PropertyValue, string PropertyFormat = "D")
        {
            return EncodeData(PropertyValue);
        }

        public byte[] EncodeValue(byte PropertyValue, string PropertyFormat = "C")
        {
            return EncodeInt8(PropertyValue);
        }

        public byte[] EncodeValue(sbyte PropertyValue, string PropertyFormat = "c")
        {
            return EncodeUInt8(PropertyValue);
        }

        public byte[] EncodeValue(ushort PropertyValue, string PropertyFormat = "S")
        {
            return EncodeInt16(PropertyValue);
        }

        public byte[] EncodeValue(uint PropertyValue, string PropertyFormat = "L")
        {
            return EncodeInt32(PropertyValue);            
        }

        public byte[] EncodeValue(int PropertyValue, string PropertyFormat = "i")
        {
            return EncodeUintPacked(PropertyValue);
        }

        private byte[] EncodeUtf8(string PropertyValue)
        {
#if NETCORE
            PropertyValue += '\0';
            Encoding utf8 = Encoding.UTF8;
            
            return utf8.GetBytes(PropertyValue);
#else
            var oldArray = Encoding.UTF8.GetBytes(PropertyValue);
            byte[] newArray = new byte[oldArray.Length + 1];
            oldArray.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = 0x00;
            return newArray;
#endif
        }

        private byte[] EncodeData(byte[] PropertyValue)
        {
            //return Utils.HexToBytes(PropertyValue);
            return PropertyValue;
        }

        public byte[] EncodeDataWithLength(byte[] PropertyValue)
        {
            byte[] DataLen = BitConverter.GetBytes((short)PropertyValue.Length);           
            return Utilities.CombineArrays(DataLen, PropertyValue);          
        }

        private byte[] EncodeInt8(byte PropertyValue)
        {
            byte[] byteArray = new byte[1];

            byteArray[0] = BitConverter.GetBytes(PropertyValue)[0];

            return byteArray;
        }

        private byte[] EncodeUInt8(sbyte PropertyValue)
        {
            byte[] byteArray = new byte[1];

            byteArray[0] = BitConverter.GetBytes(PropertyValue)[0];

            return byteArray;
        }

        private byte[] EncodeInt16(ushort PropertyValue)
        {
          //  byte[] byteArray = new byte[2];

            return BitConverter.GetBytes(PropertyValue);
        }

        private byte[] EncodeInt32(uint PropertyValue)
        {
           // byte[] byteArray = new byte[4];

            return BitConverter.GetBytes(PropertyValue);
        }

        private byte[] EncodeUintPacked(int ValueToEncode)
        {
            int encoded_size = Spinel_packed_uint_size(ValueToEncode);

            byte[] tempByte = new byte[encoded_size];

            int index;

            for (index = 0; index != encoded_size - 1; index++)
            {
                tempByte[index] = (byte)((ValueToEncode & 0x7F) | 0x80);
                ValueToEncode = ValueToEncode >> 7;
            }

            tempByte[index] = (byte)(ValueToEncode & 0x7F);

            return tempByte;
        }

        private int Spinel_packed_uint_size(int value)
        {
            int ret;

            if (value < (1 << 7))
            {
                ret = 1;
            }
            else if (value < (1 << 14))
            {
                ret = 2;
            }
            else if (value < (1 << 21))
            {
                ret = 3;
            }
            else if (value < (1 << 28))
            {
                ret = 4;
            }
            else
            {
                ret = 5;
            }

            return ret;
        }
    }
}
