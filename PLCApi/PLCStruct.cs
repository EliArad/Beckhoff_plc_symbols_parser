using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


// PLC  INT   = C# short
// PLC DINT    = C# int
// PLC SINT   = c# byte
// PLC LREAL  = c# double
// PLC REAL =   c# float

namespace PLCApi
{
    public class PLCStruct : PLCArray
    {

        public PLCStruct(PLC plc, string ProgramName, string VarName) : base(plc, ProgramName, VarName)
        {
        }

        public PLCStruct(PLC plc, string Name) : base(plc, Name)
        {
            m_hVar = m_plc.AdsClient.CreateVariableHandle(Name);
        }
         
        byte[] StructToByteArray<T>(T structVal) where T : struct
        {
            int size = Marshal.SizeOf(structVal);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structVal, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        T StructFromByteArray<T>(byte[] bytes) where T : struct
        {
            int sz = Marshal.SizeOf(typeof(T));
            IntPtr buff = Marshal.AllocHGlobal(sz);
            Marshal.Copy(bytes, 0, buff, sz);
            T ret = (T)Marshal.PtrToStructure(buff, typeof(T));
            Marshal.FreeHGlobal(buff);
            return ret;
        }

        public override void Write<T>(T t)
        {
            byte [] b = StructToByteArray(t);

            m_binWrite.Write(b);
            //Write complete stream in the PLC
            m_plc.AdsClient.Write(m_hVar, m_dataStream);
        }

        public override T Read<T>(T t)
        {
            byte[] b = new byte[Marshal.SizeOf(t)];
            //Write complete stream in the PLC
            m_plc.AdsClient.Read(m_hVar, m_dataStream);
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = m_binRead.ReadByte();
            }
            T t1 = StructFromByteArray<T>(b);
            return t1;
        }
    }
}
