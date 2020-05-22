using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCApi
{
    public class PLCString : PLCVar
    {

        public PLCString(PLC plc, string ProgramName, string VarName) : base(plc, ProgramName, VarName)
        {
        }

        public PLCString(PLC plc,string Name) : base(plc, Name)
        {
            
        }

        public override void Read(out string text)
        {

            //length of the stream = length of string in sps + 1
            m_binRead = new BinaryReader(m_dataStream, System.Text.Encoding.ASCII);
            int length = m_plc.AdsClient.Read(m_hVar, m_dataStream);
            text = new string(m_binRead.ReadChars(length));
            //necessary if you want to compare the string to other strings
            //text = text.Substring(0,text.IndexOf('\0'));
        }
        public override void Write(string text)
        {
            //length of the stream = length of string + 1
            BinaryWriter writer = new BinaryWriter(m_dataStream, System.Text.Encoding.ASCII);
            writer.Write(text.ToCharArray());
            //add terminating zero
            writer.Write('\0');
            m_plc.AdsClient.Write(m_hVar, m_dataStream);
        }
    }
}
