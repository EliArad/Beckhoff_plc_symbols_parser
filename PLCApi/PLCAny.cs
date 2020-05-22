using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace PLCApi
{
    public class PLCAny : IDisposable
    {
        protected AdsStream m_dataStream;
        protected BinaryReader m_binRead;
        protected BinaryWriter m_binWrite;

        protected int m_hVar;
        protected int m_size;
        PLC m_plc;
        bool disposed = false;
        string m_varName;
        IPLCNotify m_notify;       
        int hConnect;
        int m_sizeOfType = 0;
        public PLCAny(PLC plc,string Name)
        {
            m_plc = plc;
            m_hVar = plc.AdsClient.CreateVariableHandle(Name);
            m_varName = Name;
            m_dataStream = new AdsStream(31);
            m_binRead = new BinaryReader(m_dataStream, System.Text.Encoding.ASCII);
             
        }
        public PLCAny(PLC plc, string ProgName, string Name)
        {
            m_plc = plc;
            m_varName = ProgName + "." + Name;
            m_hVar = plc.AdsClient.CreateVariableHandle(m_varName);
            
        }

        string m_changedValue;

        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            if (e.NotificationHandle == hConnect)
            {
                //if(typeof(T) == typeof(bool))
                {
                    m_changedValue = m_binRead.ReadBoolean().ToString();
                    m_notify.NotifyChanges(m_changedValue);
                }
            }
        }

        // in structurethe loction is the index
        public void AddNotification(IPLCNotify notify, object text, int location = 0, int cycleTimeInMs = 100, int maxDelay = 0)
        {
            m_notify = notify;
            hConnect = m_plc.AdsClient.AddDeviceNotification(m_varName, m_dataStream, location, m_sizeOfType,  AdsTransMode.OnChange, cycleTimeInMs, maxDelay, text);
        }

        public int [] Read(int size)
        {

            int[] arr = (int[])m_plc.AdsClient.ReadAny(m_hVar, typeof(int[]), new int[] { size });
            return arr;
        }

        public string Read(out string str, int size)
        {
            str = m_plc.AdsClient.ReadAny(m_hVar, typeof(String), new int[] { size }).ToString();
            return str;
        }

        public void Read(out Byte val)
        {
            val = (Byte)(m_plc.AdsClient.ReadAny(m_hVar, typeof(Byte)));            
        }
         
        public void Read(out float val)
        {
            val = (float)(m_plc.AdsClient.ReadAny(m_hVar, typeof(float)));
        }

        public void Read(out Boolean val)
        {
            val = (Boolean)(m_plc.AdsClient.ReadAny(m_hVar, typeof(Boolean)));
        }

        public void Read(out Double val)
        {
            val = (Double)(m_plc.AdsClient.ReadAny(m_hVar, typeof(Double)));
        }

        byte[] StructToByteArray<T>(T structVal) 
        {
            int size = Marshal.SizeOf(structVal);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structVal, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        T StructFromByteArray<T>(byte[] bytes) 
        {
            int sz = Marshal.SizeOf(typeof(T));
            IntPtr buff = Marshal.AllocHGlobal(sz);
            Marshal.Copy(bytes, 0, buff, sz);
            T ret = (T)Marshal.PtrToStructure(buff, typeof(T));
            Marshal.FreeHGlobal(buff);
            return ret;
        }

      

        public void Read(out int val)
        {
            val = (int)(m_plc.AdsClient.ReadAny(m_hVar, typeof(int)));
        }

        public void Write(object[] arr)
        {
            m_plc.AdsClient.WriteAny(m_hVar, arr);            
        }
        
        public void Write(object value)
        {
            m_plc.AdsClient.WriteAny(m_hVar, value);
        }

        public void Set()
        {
            m_plc.AdsClient.WriteAny(m_hVar, true);
        }

        public void Clear()
        {
            m_plc.AdsClient.WriteAny(m_hVar, false);
        }
  
        public void Write(bool value)
        {
            m_plc.AdsClient.WriteAny(m_hVar, value);
        }
        // or this or the above 
        public void Write(byte [] value)
        {
            m_plc.AdsClient.WriteAny(m_hVar, value);
        }
        public void Write(string str, int size)
        {
            m_plc.AdsClient.WriteAny(m_hVar, str, new int[] { size });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                m_plc.AdsClient.DeleteVariableHandle(m_hVar);
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~PLCAny()
        {
            Dispose(false);
        }


    }
}
