using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace PLCApi
{
    public class PLCVar
    {

        protected AdsStream m_dataStream;
        protected BinaryReader m_binRead;
        protected BinaryWriter m_binWrite;

        protected int m_hVar;
        protected int m_size;
        protected PLC m_plc;

        public PLCVar(PLC plc, string ProgramName, string VarName)
        {
            m_plc = plc;
            string name = string.Format("{0}.{1}", ProgramName, VarName);
            m_hVar = plc.AdsClient.CreateVariableHandle(name);
        }

        public PLCVar(PLC plc, string Name)
        {
            m_plc = plc;
            m_hVar = plc.AdsClient.CreateVariableHandle(Name);
        }
        public virtual void VarSize(int arraySize, int arrayTypeSize)
        {
            m_size = arraySize * arrayTypeSize;
        }
        public virtual void VarSize(int size)
        {
            m_size = size;
            m_dataStream = new AdsStream(m_size);
        }

        public virtual void Write<T>(T t) where T : struct
        {

        }

        public virtual T Read<T>(T t) where T : struct
        {
            throw (new NotImplementedException());
        }

        public virtual void Read(out string text)
        {
            throw (new NotImplementedException());
        }
        public virtual void ReadIn16(out Int16[] arr, int index = 0)
        {
            throw (new NotImplementedException());
        }
        public virtual void Write(string text)
        {
            throw (new NotImplementedException());
        }
    }
}
