using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DDC
{
    internal static class DDCCI
    {

        [DllImport("Comm_RealtekUSB.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void I2CWrite(byte ucSlave, byte ucSub, short usLength, IntPtr ptr_byte);

        [DllImport("Comm_RealtekUSB.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void I2CRead(byte ucSlave, byte ucSub, short usLength, IntPtr ptr_byte);


        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern uint GetTickCount();

        public static void Waitor(uint ms)
        {
            uint start = GetTickCount();
            while (GetTickCount() - start < ms)
            {
                Application.DoEvents();
            }
        }
        private static byte BOEDCCCI_CheckSum(byte[] byte_buffer)
        {
            int i;
            byte checksum = 0;

            for (i = 0; i < byte_buffer.Length; i++)
            {
                checksum ^= byte_buffer[i];
            }

            return (byte)checksum;
        }
        public static void BOEDCCCI_Write(byte[] byte_buffer)
        {
            byte[] buffer = new byte[byte_buffer.Length + 1];

            byte_buffer.CopyTo(buffer, 0);
            buffer[byte_buffer.Length] = (byte)(BOEDCCCI_CheckSum(byte_buffer) ^ 0x6e ^ 0x51);//数组尾加入checksum

            GCHandle hObject = GCHandle.Alloc(buffer, GCHandleType.Pinned);//获取数组指针
            IntPtr ptr_byte = hObject.AddrOfPinnedObject();

            I2CWrite(0x6E, 0x51, (short)buffer.Length, ptr_byte);
        }
        public static void BOEDCCCI_Read(byte[] byte_buffer, byte[] return_buffer)
        {
            BOEDCCCI_Write(byte_buffer);

            Waitor(5);

            GCHandle hObject = GCHandle.Alloc(return_buffer, GCHandleType.Pinned);//获取数值指针
            IntPtr ptr_byte = hObject.AddrOfPinnedObject();

            I2CRead(0x6E, 0x51, (short)return_buffer.Length, ptr_byte);
        }
        public static void BOEDCCCI_Read(byte[] return_buffer)//函数重载
        {
            GCHandle hObject = GCHandle.Alloc(return_buffer, GCHandleType.Pinned);//获取数值指针
            IntPtr ptr_byte = hObject.AddrOfPinnedObject();

            I2CRead(0x6E, 0x51, (short)return_buffer.Length, ptr_byte);
        }
    }
}
