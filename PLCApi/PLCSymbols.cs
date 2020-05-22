using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;

namespace PLCApi
{
    public class PLCSymbols : PLC
    {
        const string m_fileName = "plc_symbols.json";
        TcAdsSymbolInfoLoader symbolLoader;
        Dictionary<string, TcAdsSymbolInfo> m_plcSymbols = new Dictionary<string, TcAdsSymbolInfo>();
        AdsStream m_dataStream;
        List<string> m_plcSymbolsList;
        protected BinaryReader m_binRead;
        Dictionary<string, int> m_hConnect = new Dictionary<string, int>();
        StreamWriter m_sw;
        StreamWriter m_swSymbols;
        Dictionary<string, IPLCNotify> m_notify = new Dictionary<string, IPLCNotify>();

        [Serializable]
        public struct PLC_SYMBOLS
        {
            public string Name;
            public string ShortName;
            public long IndexGroup;
            public long IndexOffset;
            public int Size;
            public string Type;
            public AdsDatatypeId Datatype;
            public object Value;

        }
        Dictionary<string, PLC_SYMBOLS> m_plcSymbols2 = new Dictionary<string, PLC_SYMBOLS>();

        public PLCSymbols()
        {
            m_dataStream = new AdsStream(31);
            m_binRead = new BinaryReader(m_dataStream, System.Text.Encoding.ASCII);

            m_adsClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
        }

        public void OnNotification(object sender, AdsNotificationEventArgs e)
        { 
            if (m_notify.ContainsKey(e.UserData.ToString()) == true)
            {
                m_notify[e.UserData.ToString()].NotifyChanges(e.UserData.ToString());
            }
        }

        public void DeleteDeviceNotification(int handle)
        {
            try
            {
                m_adsClient.DeleteDeviceNotification(handle);
            }
            catch (Exception err)
            {

            }
            
        }

        // in structurethe loction is the index
        public int AddNotification(IPLCNotify notify, string varName, string text,  AdsTransMode transMode = AdsTransMode.OnChange)
        {
            if (m_hConnect.ContainsKey(varName) == true)
                return m_hConnect[varName];

            object userData = varName;
            int handle = m_adsClient.AddDeviceNotification(varName, m_dataStream, transMode, 0, 100, text);
            m_notify.Add(text, notify);
            m_hConnect.Add(varName, handle);         
            return handle;
        }

        public int AddNotification(IPLCNotify notify, string varName, string text, AdsTransMode transMode, int cycleTime, int maxDelay)
        {
            if (m_hConnect.ContainsKey(varName) == true)
                return m_hConnect[varName];

            object userData = varName;
            int handle = m_adsClient.AddDeviceNotification(varName, m_dataStream, transMode, cycleTime, maxDelay, text);
            m_notify.Add(text, notify);
            m_hConnect.Add(varName, handle);
            return handle;
        }
        public Dictionary<string, PLC_SYMBOLS> GetAllSymbols2()
        {
            return m_plcSymbols2;
        }
        public Dictionary<string, TcAdsSymbolInfo> GetAllSymbols()
        {
            return m_plcSymbols;
        }
        public int GetSymbol(string Name, out PLC_SYMBOLS symbol, out object Value)
        {
            try
            {
                symbol = m_plcSymbols2[Name];
                Value = 0xFFFFFFFFF;
                try
                {
                    TcAdsSymbolInfo s = m_plcSymbols[Name];
                    Value = m_adsClient.ReadSymbol(s).ToString();
                    return 1;
                }
                catch (AdsDatatypeNotSupportedException err)
                {
                    Value = err.Message;
                    return -1;
                }
                catch (Exception err)
                {
                    return -2;
                }
            }
            catch (Exception err)
            {
                throw (new SystemException("Symbol not found"));
            }
        }
        public int GetSymbol(string Name, out TcAdsSymbolInfo symbol, out object Value)
        {
            try
            {
                symbol = m_plcSymbols[Name];
                Value = 0xFFFFFFFFF;
                try
                {
                    TcAdsSymbolInfo s = m_plcSymbols[Name];
                    Value = m_adsClient.ReadSymbol(s).ToString();
                    return 1;
                }
                catch (AdsDatatypeNotSupportedException err)
                {
                    Value = err.Message;
                    return -1;
                }
                catch (Exception err)
                {
                    return -2;
                }
            }
            catch (Exception err)
            {
                throw (new SystemException("Symbol not found"));
            }
        }

        public bool ReadSymbol(string str)
        {
            ITcAdsSymbol symbol = m_plcSymbols[str];
            return Convert.ToBoolean(m_adsClient.ReadSymbol(symbol));
        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out object Value)
        {
            try
            {
                Value = 0;
                ITcAdsSymbol symbol = m_adsClient.ReadSymbolInfo(symbolName);
                if (symbol == null)
                {
                    throw (new SystemException("Unable to read Symbol Info " + symbolName));
                }

                try
                {
                    Value = m_adsClient.ReadSymbol(symbol);
                }
                catch (AdsDatatypeNotSupportedException err)
                {
                    Value = 0;
                }
                catch (Exception err)
                {
                    throw (new SystemException("Unable to read Symbol Info. " + symbolName + " " + err.Message));
                }
                return symbol;
            }
            catch (Exception err)
            {
                throw (new SystemException("Unable to read Symbol Info. " + symbolName + " " + err.Message));
            }
        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out int Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToInt32(v);
            return r;

        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out bool Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToBoolean(v);
            return r;

        }

        string CreateSymbolRoot(TcAdsSymbolInfo symbol, bool begin)
        {
            if (begin == true)
            {

                string[] parts = symbol.Name.Split('.');
                if (parts[0] == string.Empty)
                {
                    parts[0] = "_" + parts[1];
                }
                m_sw.WriteLine("public struct {0}", parts[0]);
                m_sw.WriteLine("{");
                if (CreateSymbolNode(symbol) == false)
                    return string.Empty;

                return parts[0];
            }
            else
            {
                m_sw.WriteLine("}");
            }
            return string.Empty;
        }

        bool CreateSymbolNode(TcAdsSymbolInfo symbol)
        {
            string[] parts = symbol.Name.Split('.');


            if (parts[1].Contains("[") == true)
            {
                parts[1] = parts[1].Replace('[', '_');
                parts[1] = parts[1].Replace(']', ' ');
                parts[1] = parts[1].TrimEnd();
            }


            if (symbol.Datatype == AdsDatatypeId.ADST_REAL80)
                m_sw.WriteLine("public static double {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_REAL64)
                m_sw.WriteLine("public static double {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_INT32)
                m_sw.WriteLine("public static int {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_INT16)
                m_sw.WriteLine("public static short {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_STRING)
                m_sw.WriteLine("public static string {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_INT8)
                m_sw.WriteLine("public static char {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_UINT8)
                m_sw.WriteLine("public static byte {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_REAL64)
                m_sw.WriteLine("public static double {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_REAL32)
                m_sw.WriteLine("public static float {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_BIT)
                m_sw.WriteLine("public static bool {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_UINT32)
                m_sw.WriteLine("public static uint {0}", parts[1]);
            else
            if (symbol.Datatype == AdsDatatypeId.ADST_UINT16)
                m_sw.WriteLine("public static ushort {0}", parts[1]);
            else
            {
                //throw (new SystemException("Un handled type:" + symbol.Datatype.ToString()));
                return false;

            }

            m_sw.WriteLine("{");
            m_sw.WriteLine("get");
            m_sw.WriteLine("{");
            m_sw.WriteLine("symbol = m_plcSymbols[\"{0}\"];", symbol.Name);

            if (symbol.Datatype == AdsDatatypeId.ADST_STRING)
            {
                m_sw.WriteLine("\treturn Client.ReadSymbol(symbol).ToString();");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_UINT16)
            {
                m_sw.WriteLine("\treturn Convert.ToUInt16(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_REAL64)
            {
                m_sw.WriteLine("\treturn Convert.ToDouble(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_REAL80)
            {
                m_sw.WriteLine("\treturn Convert.ToDouble(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_INT16)
            {
                m_sw.WriteLine("\treturn Convert.ToInt16(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_INT32)
            {
                m_sw.WriteLine("\treturn Convert.ToInt32(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_UINT8)
            {
                m_sw.WriteLine("\treturn Convert.ToByte(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_INT8)
            {
                m_sw.WriteLine("\treturn Convert.ToChar(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_UINT32)
            {
                m_sw.WriteLine("\treturn Convert.ToUInt32(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_BIT)
            {
                m_sw.WriteLine("\treturn Convert.ToBoolean(Client.ReadSymbol(symbol));");
            }
            else if (symbol.Datatype == AdsDatatypeId.ADST_BIGTYPE)
            {
                m_sw.WriteLine("\treturn Convert.ADST_UINT32(Client.ReadSymbol(symbol));");
            }
            else
            {
                //throw (new SystemException("Un handled type:" + symbol.Datatype.ToString()));
                return false;
            }


            m_sw.WriteLine("}");
            m_sw.WriteLine("set");
            m_sw.WriteLine("{");
            m_sw.WriteLine("symbol = m_plcSymbols[\"{0}\"];", symbol.Name);
            m_sw.WriteLine("Client.WriteSymbol(symbol, value.ToString());");
            m_sw.WriteLine("}");
            m_sw.WriteLine("}");
            return true;

        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out uint Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToUInt32(v);
            return r;
        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out ushort Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToUInt16(v);
            return r;
        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out short Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToInt16(v);
            return r;
        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out byte Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToByte(v);
            return r;
        }
        public ITcAdsSymbol ReadSymbol(string symbolName, out double Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToDouble(v);
            return r;
        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out float Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = Convert.ToSingle(v);
            return r;
        }

        public ITcAdsSymbol ReadSymbol(string symbolName, out string Value)
        {
            object v;
            ITcAdsSymbol r = ReadSymbol(symbolName, out v);
            Value = v as string;
            return r;

        }

        public ITcAdsSymbol FindSymbol(string symbolName)
        {

            try
            {
                ITcAdsSymbol symbol = symbolLoader.FindSymbol(symbolName);
                if (symbol == null)
                {
                    return null;
                }
                return symbol;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public void WriteSymbol(string symbolName, object value)
        {
            try
            {
                ITcAdsSymbol symbol;
                symbol = m_plcSymbols[symbolName];
                m_adsClient.WriteSymbol(symbol, value.ToString());
            }
            catch (Exception err)
            {
                throw (new SystemException("Unable to write Value. " + err.Message));
            }
        }

        Dictionary<string, bool> m_wasCreated = new Dictionary<string, bool>();

    
        private void CreateNewNode2(TcAdsSymbolInfo symbol)
        {
            //if (symbol.Type.ToString().Contains("ARRAY") == true)
              //  return;

            m_swSymbols.WriteLine("\"{0},{1}\",", symbol.Name, symbol.Type);          
            m_plcSymbolsList.Add(symbol.Name + "," + symbol.Type);
            //node.Tag = symbol;
            TcAdsSymbolInfo subSymbol = symbol.FirstSubSymbol;
            while (subSymbol != null)
            {
                CreateNewNode2(subSymbol);
                subSymbol = subSymbol.NextSymbol;
            }
        }
        public void CreateSymbolTypesFile3(string PREFIX, string fileName = "")
        {
            try
            {
                if (fileName == string.Empty)
                    fileName = "plc_symbols.txt";
                m_swSymbols = new StreamWriter(fileName);
                 

                m_plcSymbolsList = new List<string>();
                if (symbolLoader == null)
                    symbolLoader = m_adsClient.CreateSymbolInfoLoader();
            }
            catch (Exception err)
            {
                if (m_swSymbols != null)
                    m_swSymbols.Close();
                throw (new SystemException(err.Message));
            }
             
            TcAdsSymbolInfo symbol = symbolLoader.GetFirstSymbol(true);
            while (symbol != null)
            {
                CreateNewNode2(symbol);
                symbol = symbol.NextSymbol;
            }

            if (m_swSymbols != null)
                m_swSymbols.Close();

             
        }

        public void CreateSymbolTypesFile1(string PREFIX, string fileName = "")
        {

            try
            {
                m_wasCreated.Clear();
                m_sw = new StreamWriter(fileName == "" ? "plc_symbol_types.cs" : fileName);

                m_sw.WriteLine("/////////////  AUTO GENERATED {0}  ////////", DateTime.Now);
                m_sw.WriteLine();

                m_sw.WriteLine("using System;");
                m_sw.WriteLine("namespace PLCApi");
                m_sw.WriteLine("{");
                m_sw.WriteLine("public partial class PLCTypes : PLCSymbols");
                m_sw.WriteLine("{");
                m_sw.WriteLine("public struct {0}", PREFIX);
                m_sw.WriteLine("{");
                bool firstTime = true;

                foreach (TcAdsSymbolInfo symbol in symbolLoader)
                {
                    if (symbol.Type.ToString().Contains("ARRAY") == false)
                    {
                        string[] parts = symbol.Name.Split('.');
                        if (m_wasCreated.ContainsKey(parts[0]) == false)
                        {
                            if (firstTime == false)
                            {
                                m_sw.WriteLine("\t}");
                            }
                            string s = CreateSymbolRoot(symbol, true);
                            if (s != string.Empty)
                                m_wasCreated.Add(s, true);
                            firstTime = false;
                        }
                        else
                        {
                            CreateSymbolNode(symbol);
                        }
                    }
                }
                m_sw.WriteLine("}");
                m_sw.WriteLine("}");
                m_sw.WriteLine("}");
                m_sw.WriteLine("}");
                m_sw.Close();
            }
            catch (Exception err)
            {
                if (m_sw != null)
                    m_sw.Close();
                throw (new SystemException(err.Message));
            }

        }

        public void CreateSymbolTypesFile(string PREFIX , string fileName)
        {
            m_wasCreated.Clear();
            m_sw = new StreamWriter(fileName == "" ? "plc_symbol_types.cs" : fileName);

            m_sw.WriteLine("/////////////  AUTO GENERATED {0}  ////////" , DateTime.Now);
            m_sw.WriteLine();

            m_sw.WriteLine("using System;");
            m_sw.WriteLine("namespace PLCApi");
            m_sw.WriteLine("{");
            m_sw.WriteLine("public partial class PLCTypes : PLCSymbols");
            m_sw.WriteLine("{");
            m_sw.WriteLine("public struct {0}", PREFIX);
            m_sw.WriteLine("{");
            bool firstTime = true;

            TcAdsSymbolInfo symbol = symbolLoader.GetFirstSymbol(true);
            while (symbol != null)
            {
                if (
                    (symbol.Type.ToString().Contains("ARRAY") == false) &&
                    symbol.Datatype != AdsDatatypeId.ADST_BIGTYPE)
                {
                    string[] parts = symbol.Name.Split('.');
                    if (m_wasCreated.ContainsKey(parts[0]) == false)
                    {
                        if (firstTime == false)
                        {
                            m_sw.WriteLine("\t}");
                        }
                        m_wasCreated.Add(CreateSymbolRoot(symbol, true), true);
                        firstTime = false;
                    }
                    else
                    {
                        CreateSymbolNode(symbol);
                    }
                }
                symbol = symbol.NextSymbol;
            }

            m_sw.WriteLine("}");
            m_sw.WriteLine("}");
            m_sw.WriteLine("}");
            m_sw.WriteLine("}");
            m_sw.Close();         
        }

        public void LoadSymbols()
        {

            try
            {
                symbolLoader = m_adsClient.CreateSymbolInfoLoader();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

            m_plcSymbols.Clear();
            m_plcSymbols2.Clear();

            TcAdsSymbolInfo symbol = symbolLoader.GetFirstSymbol(true);
            while (symbol != null)
            {
                string symbolName = symbol.Name.TrimStart('.');

                m_plcSymbols.Add(symbolName, symbol);

                PLC_SYMBOLS p = new PLC_SYMBOLS();
                p.Name = symbolName;
                p.IndexGroup = symbol.IndexGroup;
                p.IndexOffset = symbol.IndexOffset;
                p.IndexOffset = symbol.IndexOffset;
                p.Size = symbol.Size;
                p.Type = symbol.Type;
                p.Datatype = symbol.Datatype;
                string ShortName = symbol.ShortName.TrimStart('.');
                p.ShortName = ShortName;

                try
                {
                    p.Value = m_adsClient.ReadSymbol(symbol).ToString();
                    m_plcSymbols2.Add(p.Name, p);
                }
                catch (AdsDatatypeNotSupportedException err)
                {
                    p.Value = err.Message;
                }
                catch (Exception err)
                {
                    //MessageBox.Show("Unable to read Symbol Info. " + err.Message);
                }
                symbol = symbol.NextSymbol;
            }
        }


        public void LoadSymbols2()
        {

            try
            {
                symbolLoader = m_adsClient.CreateSymbolInfoLoader();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

            m_plcSymbols.Clear();
            m_plcSymbols2.Clear();

            foreach (TcAdsSymbolInfo symbol in symbolLoader)
            {

                m_plcSymbols.Add(symbol.Name, symbol);

                PLC_SYMBOLS p = new PLC_SYMBOLS();
                p.Name = symbol.Name;
                p.IndexGroup = symbol.IndexGroup;
                p.IndexOffset = symbol.IndexOffset;
                p.IndexOffset = symbol.IndexOffset;
                p.Size = symbol.Size;
                p.Type = symbol.Type;
                p.Datatype = symbol.Datatype;
                p.ShortName = symbol.ShortName;

                try
                {
                    p.Value = m_adsClient.ReadSymbol(symbol).ToString();
                    m_plcSymbols2.Add(p.Name, p);
                }
                catch (AdsDatatypeNotSupportedException err)
                {
                    p.Value = err.Message;
                }
                catch (Exception err)
                {
                    //MessageBox.Show("Unable to read Symbol Info. " + err.Message);
                }             
            }
        }

        private void CreateNewNode4(TcAdsSymbolInfo symbol)
        {

            


            m_plcSymbols.Add(symbol.Name, symbol);

            PLC_SYMBOLS p = new PLC_SYMBOLS();
            p.Name = symbol.Name;
            p.IndexGroup = symbol.IndexGroup;
            p.IndexOffset = symbol.IndexOffset;
            p.IndexOffset = symbol.IndexOffset;
            p.Size = symbol.Size;
            p.Type = symbol.Type;
            p.Datatype = symbol.Datatype;
            string ShortName = symbol.ShortName.TrimStart('.');
            p.ShortName = ShortName;

            try
            {
                //p.Value = m_adsClient.ReadSymbol(symbol).ToString();
                m_plcSymbols2.Add(p.Name, p);
            }
            catch (AdsDatatypeNotSupportedException err)
            {
                p.Value = err.Message;
            }
            catch (Exception err)
            {
                //MessageBox.Show("Unable to read Symbol Info. " + err.Message);
            }

            //TreeNode node = new TreeNode(symbol.Name);
            //node.Tag = symbol;
            TcAdsSymbolInfo subSymbol = symbol.FirstSubSymbol;
            while (subSymbol != null)
            {
                string symbolName = subSymbol.Name.TrimStart('.');
                if (symbol.Name.Contains("GVL_") == true)
                {
                    CreateNewNode4(subSymbol);
                }
                subSymbol = subSymbol.NextSymbol;
            }
        }

        public void LoadSymbols3()
        {

            try
            {
                symbolLoader = m_adsClient.CreateSymbolInfoLoader();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

            m_plcSymbols.Clear();
            m_plcSymbols2.Clear();

            TcAdsSymbolInfo symbol = symbolLoader.GetFirstSymbol(true);
            while (symbol != null)
            {
                if (symbol.Name.Contains("GVL_") == true)
                {
                    CreateNewNode4(symbol);
                }
                symbol = symbol.NextSymbol;
            }          
        }

        string Save()
        {
            try
            {
                using (StreamWriter file = File.CreateText(m_fileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, m_plcSymbols2);
                }
                return "ok";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
         
        public string DumpSymbols()
        {
            return Save();
        }
    }
}
