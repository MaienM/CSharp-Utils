using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace CSharpUtils.Structs
{
    public class StructUtils
    {
        private static byte GetBitmask(int bit)
        {
            return (byte)(1 << bit);
        }

        public static bool GetBitAsBool(byte data, int bit)
        {
            return (data & GetBitmask(bit)) != 0;
        }

        public static byte SetBitAsBool(byte data, int bit, bool value)
        {
            if (value)
            {
                data |= GetBitmask(bit);
            }
            else
            {
                data &= (byte) (~GetBitmask(bit));
            }
            return data;
        }

        /// <summary>
        /// converts byte[] to struct
        /// </summary>
        public static T RawDeserialize<T>(byte[] rawData, int position)
        {
            if (rawData == null)
            {
                return default(T);
            }

            int rawsize = Marshal.SizeOf(typeof(T));
            if (rawsize > rawData.Length - position)
            {
                throw new ArgumentException("Not enough data to fill struct. Array length from position: " +
                                            (rawData.Length - position) + ", Struct length: " + rawsize);
            }
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            T retobj = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        /// <summary>
        /// converts a struct to byte[]
        /// </summary>
        public static byte[] RawSerialize(object anything)
        {
            int rawSize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawDatas = new byte[rawSize];
            Marshal.Copy(buffer, rawDatas, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawDatas;
        }
    }
}
