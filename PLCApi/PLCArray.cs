using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.IO; 


namespace PLCApi
{
    public class PLCArray: PLCVar
    {
       
        int m_arraySize;

        public PLCArray(PLC plc, string ProgramName, string VarName) : base(plc, ProgramName, VarName)
        {
        }
             
        public PLCArray(PLC plc, string VarName) : base(plc, VarName)
        {
            m_hVar = plc.AdsClient.CreateVariableHandle(VarName);
        }

        public override void VarSize(int arraySize, int arrayTypeSize)
        {

            m_arraySize = arraySize;
            m_size = arraySize * arrayTypeSize;
            m_dataStream = new AdsStream(m_size);
            m_binRead = new BinaryReader(m_dataStream);
            m_binWrite = new BinaryWriter(m_dataStream);

        }
        
        public override void ReadIn16(out Int16[] arr, int index = 0)
        {          
              
            arr = new Int16[m_size];

            m_dataStream.Position = index;
            m_plc.AdsClient.Read(m_hVar, m_dataStream);
         
            for (int i = 0; i < m_arraySize; i++)
            {
                 arr[i] = m_binRead.ReadInt16();
            }         
        }
    }
}
